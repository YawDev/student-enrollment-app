using System.Net;
using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ControllerHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudentEnrollment.App.Models;
using StudentEnrollment.App.Services;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Services;
using static StudentEnrollment.App.Models.TimeSelectField;
using StudentEnrollment.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using StudentEnrollment.Entities;
using StudentEnrollment.Core.Services;

namespace StudentEnrollment.App.Controllers
{
    public class CoursesController : BaseController
    {
        private readonly ILogger<CoursesController> _logger;
        private readonly IApiService _apiService;
        private readonly IUploadService _uploadService;
        private readonly TimeSelectField _TimeSelector;
        private readonly IUserAuthService _userAuthService;


        public CoursesController(ILogger<CoursesController> logger, IApiService ApiService,
       IUserAuthService userAuthService, IUploadService uploadService)
        {
            _logger = logger;
            _apiService = ApiService;
            _TimeSelector = new TimeSelectField();
            _uploadService = uploadService;
            _userAuthService = userAuthService;
        }

        public IActionResult UploadCourses()
        {
            if (!_userAuthService.IsSignedIn(User))
                return RedirectToAction("Login", "Accounts");

            return View(new UploadCoursesViewModel());
        }

        [HttpPost]
        public IActionResult UploadCourses(UploadCoursesViewModel uploadCoursesViewModel)
        {
            try
            {
                var userid = _userAuthService.GetUserid(User);
                var formfile = uploadCoursesViewModel.FormFile;


                if (uploadCoursesViewModel.FormFile?.Length > 0)
                {
                    _uploadService.ValidateFile(formfile);
                    _uploadService.SaveFile();
                    var fileContent = _uploadService.GetFileContent();
                    var courseDtos = _uploadService.GetMappedDtos();
                    var fileDto = new FileDto(){mappedData = courseDtos, Content = fileContent, fileName=formfile.FileName};

                    var response = _apiService.PostObjectResponse($"api/upload/courses/{userid}", fileDto);

                    if (response.StatusCode != HttpStatusCode.Created)
                        ViewBag.Message = "Something went wrong when trying to upload file.";
                    else
                        ViewBag.Message = "File successfully uploaded. Check status of upload.";

                    return View(uploadCoursesViewModel);
                }
                ViewBag.Message = "File is missing.";
                return View(uploadCoursesViewModel);
            }
            catch (DomainException ex)
            {
                ViewBag.Message = ex.Message;
                _logger.LogError(ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Unable Able to Upload File.";
                _logger.LogError(ex.Message);
                return View(uploadCoursesViewModel);
            }
        }

        [HttpGet]
        public IActionResult UploadCourseLogs()
        {
            var userid = _userAuthService.GetUserid(User);
            if (_userAuthService.IsSignedIn(User))
            {
                var response = _apiService.GetResponse($"api/upload/logs/{userid}");
                if (response.IsSuccessStatusCode)
                {
                    var UploadLogs = _apiService.GetDeserializedObject<List<UploadCoursesLogDto>>(response);
                    return View(new UploadStatusViewModel { uploads = UploadLogs });
                }
            }
            return RedirectToAction("Login", "Accounts");
        }

        [HttpGet]
        public IActionResult LogErrors(Guid id)
        {
            var userid = _userAuthService.GetUserid(User);
            if (_userAuthService.IsSignedIn(User))
            {
                var response = _apiService.GetResponse($"api/upload/logs/errors/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var errors = _apiService.GetDeserializedObject<List<UploadCourseErrorDto>>(response);
                    return View(new UploadErrorsViewModel { Errors = errors });
                }
            }
            return RedirectToAction("Login", "Accounts");
        }




        public IActionResult Delete(Guid id)
        {
            try
            {
                var response = _apiService.GetResponse($"api/courses/{id}/details");
                if (response.IsSuccessStatusCode)
                {
                    var course = _apiService.GetDeserializedObject<CourseDto>(response);
                    return View(course);
                }
                return RedirectToAction("Notfound", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError", "Home");
            }
        }


        [HttpPost]
        public IActionResult Delete(CourseDto courseDto)
        {
            try
            {
                var id = courseDto.Id;
                var response = _apiService.DeleteResponse($"api/courses/delete/{id}");

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index", "Departments");

                return RedirectToAction("Notfound", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError", "Home");
            }
        }


        public IActionResult Add(Guid Id)
        {
            var response = _apiService.GetResponse($"api/departments/details/{Id}");
            if (response.IsSuccessStatusCode)
            {
                var Department = _apiService.GetDeserializedObject<DepartmentDto>(response);
                var ViewModel = CreateViewModel(Department);
                return View(ViewModel);
            }
            return RedirectToAction("Notfound", "Home");
        }


        [HttpPost]
        public IActionResult Add(AddCourseViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = _apiService.PostObjectResponse("api/courses/add", ViewModel.Dto);

                    if (response.IsSuccessStatusCode)
                        return RedirectToAction("Index", "Departments");

                    ViewBag.Message = _apiService.GetApiResultMessage(response);
                    return View(ViewModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError", "Home");
                }
            }
            return View(ViewModel);
        }


        public AddCourseViewModel CreateViewModel(DepartmentDto departmentDto)
        {
            return new AddCourseViewModel()
            {
                Dto = new SaveCourseDto() { Department = departmentDto.Title },
                InstructorsListItems = departmentDto.Instructors,
                Start = _TimeSelector.StartTime,
                End = _TimeSelector.EndTime
            };
        }


    }
}
