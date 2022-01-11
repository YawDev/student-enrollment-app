using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using ControllerHelpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudentEnrollment.App.Models;
using StudentEnrollment.App.Services;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Exceptions;
using StudentEnrollment.Core.Services;
using StudentEnrollment.Entities;
using StudentEnrollment.Services;
using StudentEnrollment.Store.Enums;

namespace StudentEnrollment.App.Controllers
{
    public class AccountsController : BaseController
    {

        private readonly ILogger<AccountsController> _logger;
        private readonly IApiService _apiService;
        private readonly IUserAuthService _userAuthService;


        public AccountsController(IUserAuthService userAuthService, ILogger<AccountsController> logger,
        IApiService ApiService)
        {

            _logger = logger;
            _apiService = ApiService;
            _userAuthService = userAuthService;
        }

        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel() { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View(model);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = Task.Run(() => _userAuthService.FindUserByEmail(model.Email));
                    if (user.Result != null)
                    {
                        var token = await _userAuthService.GeneratePasswordResetTokenAsync(user.Result);
                        var callback = Url.Action("ResetPassword", "Accounts", new { token, email = user.Result.Email }, Request.Scheme);
                        var message = new EmailTemplate(new string[] { user.Result.Email }, "Reset password Link", callback);
                        await _userAuthService.SendPasswordResetLink(message);
                        return RedirectToAction("ForgotPasswordConfirmation");
                    }
                    ViewBag.Message = "User with email not found";
                    return View(model);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError", "Home");
            }
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            if (!_userAuthService.IsSignedIn(User))
                return View();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (!_userAuthService.IsSignedIn(User))
                return View();



            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = _apiService.PostObjectResponse("api/login", loginDto);

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                        throw new Exception(response.ReasonPhrase);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        throw new DomainException(ErrorMessages.InvalidLogin);

                    if (response.IsSuccessStatusCode)
                    {
                        response = _apiService.GetResponse($"api/users/{loginDto.UserName}");
                        if (response.IsSuccessStatusCode)
                        {
                            var user = _apiService.GetDeserializedObject<RequestUser>(response);
                            await Task.Run(() => _userAuthService.SignInAsync(HttpContext, user));
                            return RedirectToAction("Index", "Departments");
                        }
                    }
                    return View(loginDto);

                }
                catch (DomainException ex)
                {
                    _logger.LogError(ex.Message);
                    ModelState.AddModelError("Error", ex.Message); ViewBag.Message = ex.Message;
                    return View(loginDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View(loginDto);
        }


        public IActionResult Logout()
        {
            try
            {
                _userAuthService.SignOut();
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError", "Home");
            }
        }



        public IActionResult Account()
        {
            try
            {
                if (_userAuthService.IsSignedIn(User))
                {
                    var id = _userAuthService.GetUserid(User);
                    var response = _apiService.GetResponse($"api/student-account/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var detailsDto = _apiService.GetDeserializedObject<StudentDetailsDto>(response);
                        return RedirectToAction("Details", "Students", detailsDto);
                    }

                    if (_userAuthService.HasProperPermission(User, Permissions.InstructorPermissions))
                        return RedirectToAction("Details", "Instructors", new { id = id });

                    if (_userAuthService.HasProperPermission(User, Permissions.AdminPermissions))
                        return RedirectToAction("Details", "Admin", new { id = id });
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError", "Home");
            }
        }

        public IActionResult ViewTranscript(Guid Id)
        {
            try
            {
                if (_userAuthService.IsSignedIn(User))
                {
                    if (!_userAuthService.AuthorizeUser("api/student-account", Permissions.StudentPermissions, Id, User))
                        return RedirectToAction("NotAuthorized", "Accounts");

                    return RedirectToAction("Transcript", new { id = Id });

                }
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError", "Home");
            }
        }

        public IActionResult Transcript(Guid Id)
        {
            if (_userAuthService.IsSignedIn(User))
            {
                if (!_userAuthService.AuthorizeUser("api/student-account", Permissions.StudentPermissions, Id, User))
                    return RedirectToAction("NotAuthorized", "Accounts");

                var response = _apiService.GetResponse($"api/my-enrollments/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    var enrollmentDtos = _apiService.GetDeserializedObject<List<EnrollmentDto>>(response);
                    return View(new TranscriptViewModel { Enrollments = enrollmentDtos });
                }
                return RedirectToAction("Notfound", "Home");
            }
            return RedirectToAction("Login", "Accounts");

        }

        [HttpGet]
        public IActionResult Enroll(Guid Id)
        {
            if (_userAuthService.IsSignedIn(User))
            {
                if (!_userAuthService.AuthorizeUser("api/student-account", Permissions.StudentPermissions, Id, User))
                    return RedirectToAction("NotAuthorized", "Accounts");

                return View(new EnrollViewModel { Id = Id, Dto = new EnrollCourseDto() });
            }

            return RedirectToAction("Login", "Accounts");
        }

        [HttpPost]
        public IActionResult Enroll(EnrollViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_userAuthService.IsSignedIn(User))
                    {
                        var id = viewModel.Id;
                        if (!_userAuthService.AuthorizeUser("api/student-account", Permissions.StudentPermissions, id, User))
                            return RedirectToAction("NotAuthorized", "Accounts");

                        var response = _apiService.PostObjectResponse($"api/courses-enroll/{id}", viewModel.Dto);

                        if (response.IsSuccessStatusCode)
                            return RedirectToAction("Transcript", new { id = viewModel.Id });

                        ViewBag.Message = _apiService.GetApiResultMessage(response);
                        return View(viewModel);
                    }
                    return RedirectToAction("Index", "Home");
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View(viewModel);
        }
    }
}