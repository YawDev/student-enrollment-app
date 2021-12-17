using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentEnrollment.App.Models;
using StudentEnrollment.App.Services;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Services;
using StudentEnrollment.Entities;
using StudentEnrollment.Services;

namespace StudentEnrollment.App.Controllers
{
    public class StudentsController : BaseController
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly IApiService _apiService;
        private readonly IUserAuthService _userAuthService;

        public StudentsController(ILogger<StudentsController> logger, IApiService ApiService,
        IUserAuthService userAuthService)
        {
            _logger = logger;
            _apiService = ApiService;
           _userAuthService = userAuthService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(AddStudentDto addStudentDto)
        {
            var ApiUrl = "api/students/register";
            var response =  _apiService.PostObjectResponse(ApiUrl, addStudentDto);

           if(response.StatusCode == HttpStatusCode.InternalServerError)
            {
                ViewBag.Message = "Something went wrong while creating account."; return View(addStudentDto); 
            }

            if(response.StatusCode == HttpStatusCode.BadRequest)
            {
                var message = _apiService.GetApiResultMessage(response);
                ViewBag.Message = message; return View(addStudentDto); 
            } 

            if(response.StatusCode == HttpStatusCode.Created)
            {
                ViewBag.Message = "Sign up has been successful"; return View(addStudentDto); 
            }   
            
            return View(addStudentDto);
        }


        public IActionResult Details(StudentDetailsDto studentDetailsDto)
        {
                if(!_userAuthService.IsSignedIn(User))  
                    return RedirectToAction("Login","Accounts");

                if(!_userAuthService.VerifyCurrentUser(studentDetailsDto.UserId, User)) 
                    return RedirectToAction("NotAuthorized", "Accounts");

                _apiService.PostResponse($"api/sync/student-details/{studentDetailsDto.Id}");

                var SyncLogResponse =  _apiService.GetResponse($"api/sync-logs/{studentDetailsDto.Id}");
                if(SyncLogResponse.IsSuccessStatusCode)
                    studentDetailsDto.StudentSyncLog = _apiService.GetDeserializedObject<StudentSyncLogDto>(SyncLogResponse);

            return View(studentDetailsDto);
        }

        public IActionResult Sync()
        {
            var userId = _userAuthService.GetUserid(User);
            
            var GetStudentResponse = _apiService.GetResponse($"api/student-account/{userId}");
            if(GetStudentResponse.IsSuccessStatusCode) 
            {
                var student = _apiService.GetDeserializedObject<StudentDetailsDto>(GetStudentResponse);

                var SyncLogResponse =  _apiService.PostResponse($"api/sync/student-details/{student.Id}");
                    if(SyncLogResponse.IsSuccessStatusCode)

                return RedirectToAction("Details","Students", student);
            }

            return RedirectToAction("NotFound","Home");
        }
        
    }
}