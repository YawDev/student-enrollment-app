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
using StudentEnrollment.Entities;
using StudentEnrollment.Services;

namespace StudentEnrollment.App.Controllers
{
    public class AccountsController : BaseController
    {

        private readonly UserManager<RequestUser> _userManager;
        private readonly SignInManager<RequestUser> _signInManager;    
        private readonly ILogger<AccountsController> _logger;
        private readonly IApiService _apiService;       

        public AccountsController(UserManager<RequestUser> userManager, SignInManager<RequestUser> signInManager, ILogger<AccountsController> logger,
        IApiService ApiService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _apiService = ApiService;
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

         public IActionResult Register()
        {
            var currentUser = _signInManager.IsSignedIn(User);

            if (!currentUser)
                return View();

            return RedirectToAction("Index", "Home");
        }

         public IActionResult Login()
        {
            var currentUser = _signInManager.IsSignedIn(User);
            
            if (!currentUser)
                return View();

            
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public IActionResult Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response =  _apiService.PostObjectResponse("api/login", loginDto);
                   
                    if(response.StatusCode == HttpStatusCode.InternalServerError) 
                        throw new Exception(response.ReasonPhrase);

                    if(response.StatusCode == HttpStatusCode.BadRequest) 
                        throw new DomainException(ErrorMessages.InvalidLogin);

                    if(response.IsSuccessStatusCode)
                    {
                        response =  _apiService.GetResponse($"api/users/{loginDto.UserName}");
                        if(response.IsSuccessStatusCode)
                        {
                            var user =  _apiService.GetDeserializedObject<RequestUser>(response);
                            SignUserIn(user);
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
                    return RedirectToAction("ServerError","Home");
                }
            }
            return View(loginDto);
        }


        public IActionResult Logout()
        {
            try
            {
                _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError","Home");
            }
        }
        protected async void SignUserIn(RequestUser user)
        {
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
        }

        
        public IActionResult Account()
        {
            try
            {
                if (_signInManager.IsSignedIn(User))
                {
                    var id = _userManager.GetUserId(User);
                    var response  =  _apiService.GetResponse($"api/student-account/{id}");

                    if(response.IsSuccessStatusCode)
                    {
                        var detailsDto = _apiService.GetDeserializedObject<StudentDetailsDto>(response);
                        return RedirectToAction("Details","Students", detailsDto);
                    }
                    return RedirectToAction("Details","Instructors", new {id = id});
                }
                return RedirectToAction("Index", "Home");}
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError","Home");
            }
        }

        public IActionResult ViewTranscript(Guid Id)
        {
            try
            {
                if (_signInManager.IsSignedIn(User))
                {
                        if(!AuthorizeStudent(Id))
                            return RedirectToAction("NotAuthorized", "Accounts");

                        return RedirectToAction("Transcript", new { id = Id });
                    
                }
                return RedirectToAction("Login", "Accounts");}
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError","Home");
            }
        }

        public IActionResult Transcript(Guid Id)
        {
            if(_signInManager.IsSignedIn(User))
            {
                if(!AuthorizeStudent(Id))
                    return RedirectToAction("NotAuthorized", "Accounts");

                var response  =   _apiService.GetResponse($"api/my-enrollments/{Id}");
                if(response.IsSuccessStatusCode)
                {
                    var enrollmentDtos =  _apiService.GetDeserializedObject<List<EnrollmentDto>>(response);
                    return View( new TranscriptViewModel{Enrollments=enrollmentDtos});
                }
                return RedirectToAction("Notfound","Home");
            }   
            return RedirectToAction("Login", "Accounts");

        }

        [HttpGet]
        public IActionResult Enroll(Guid Id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                if(!AuthorizeStudent(Id))
                    return RedirectToAction("NotAuthorized", "Accounts");

                return View(new EnrollViewModel{Id = Id, Dto = new EnrollCourseDto()});
            }
            
            return RedirectToAction("Login", "Accounts");
        }

        [HttpPost]
        public IActionResult Enroll(EnrollViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    if (_signInManager.IsSignedIn(User))
                    {
                        var id = viewModel.Id;
                        if(!AuthorizeStudent(id)) 
                            return RedirectToAction("NotAuthorized", "Accounts");

                        var response  =  _apiService.PostObjectResponse($"api/courses-enroll/{id}", viewModel.Dto);

                        if(response.IsSuccessStatusCode)
                            return RedirectToAction("Transcript", new {id = viewModel.Id});
                    
                        ViewBag.Message = _apiService.GetApiResultMessage(response);
                        return View(viewModel);
                    }
                return RedirectToAction("Index", "Home");
                }
            
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError","Home");
            }
        }
        return View(viewModel);
        }

        public bool AuthorizeStudent(Guid urlParameter)
        {
            var currentUser = _userManager.GetUserAsync(User);
            if(currentUser.Result.Permission != Store.Enums.Permissions.StudentPermissions)
                return false;

            var response =  _apiService.GetResponse($"api/student-account/{currentUser.Result.Id}");
            if(response.IsSuccessStatusCode)
            {
                var currentUserStudentDetails = _apiService.GetDeserializedObject<StudentDetailsDto>(response);
                if(currentUserStudentDetails.Id != urlParameter)
                    return false;
            }
            return true;
        }

        

        

    }
}