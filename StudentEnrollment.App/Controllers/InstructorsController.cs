using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudentEnrollment.App.Models;
using StudentEnrollment.App.Services;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Services;
using StudentEnrollment.Entities;
using StudentEnrollment.Services;
using StudentEnrollment.Store.Enums;

namespace StudentEnrollment.App.Controllers
{
    public class InstructorsController : BaseController
    {
        private readonly ILogger<InstructorsController> _logger;
        private readonly IApiService _apiService;
        private readonly IUserAuthService _userAuthService;
        GradeSelectField gradeSelectField;    

        public InstructorsController(ILogger<InstructorsController> logger, IApiService apiService, 
        IUserAuthService userAuthService)
        {
            _logger = logger;
            _apiService = apiService;
            _userAuthService = userAuthService;
            gradeSelectField = new GradeSelectField();
        }
        [HttpGet]
        public IActionResult Register()
        {
             if(_userAuthService.IsSignedIn(User))
                return RedirectToAction("Index", "Departments");
            return View();
        }

        [HttpPost]
        public IActionResult Register(InstructorSignUpDto addInstructorDto)
        {
            try{
                
                var response =  _apiService.PostObjectResponse("api/instructors/register",addInstructorDto);

                if(response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    ViewBag.Message = "Something went wrong while creating account."; return View(addInstructorDto); 
                }

                if(response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = _apiService.GetApiResultMessage(response);
                    ViewBag.Message = message; return View(addInstructorDto); 
                } 

                if(response.StatusCode == HttpStatusCode.Created)
                {
                    ViewBag.Message = "Sign up has been successful"; return View(addInstructorDto); 
                }   
            
                return View(addInstructorDto);
             }
            catch(Exception ex)
            {
                 _logger.LogError(ex.Message);
                return RedirectToAction("ServerError","Home");
            }
        }

        public IActionResult Details(string Id)
        {
            if(_userAuthService.IsSignedIn(User))
            {
                var currentUserId = _userAuthService.GetUserid(User);
                if(currentUserId != Id || !_userAuthService.HasProperPermission(User, Permissions.InstructorPermissions))
                        return RedirectToAction("NotAuthorized","Accounts");

                var response =  _apiService.GetResponse($"api/instructor-account/{Id}");
                if(response.IsSuccessStatusCode)
                {
                    var instructorDetailsDto = _apiService.GetDeserializedObject<InstructorDetailDto>(response);
                    return View(instructorDetailsDto);
                }
                return RedirectToAction("Notfound","Home");
            }
            return RedirectToAction("Login","Accounts");

        }

        public IActionResult CourseDetails(Guid Id)
        {
            var response =  _apiService.GetResponse($"api/instructor/my-course/{Id}");

            if(response.IsSuccessStatusCode)
            {
                var courseDto = _apiService.GetDeserializedObject<CourseByInstructorDto>(response);
                return View(courseDto);
            }
                return RedirectToAction("Notfound","Home");
        }
        
        [HttpGet]
        public IActionResult SubmitGrade(Guid Id)
        {
            if(_userAuthService.IsSignedIn(User))
            {
                if(!_userAuthService.HasProperPermission(User, Permissions.InstructorPermissions)) 
                    return RedirectToAction("NotAuthorized","Accounts");
                
                var response =  _apiService.GetResponse($"api/enrollment/{Id}");
                if(response.IsSuccessStatusCode)
                {
                    var enrollmentDto =  _apiService.GetDeserializedObject<EnrollmentDto>(response);
                    var ViewModel = CreateViewModel(new SubmitGradeViewModel(){LetterGradeSelect=gradeSelectField.Options}, enrollmentDto);
                   
                    return View(ViewModel);
                }
                return View(new SubmitGradeViewModel(){Dto = new SubmitGradeDto(), LetterGradeSelect=gradeSelectField.Options});
            }
            return RedirectToAction("Login", "Accounts");
           
        }

        [HttpPost]
        public IActionResult SubmitGrade(SubmitGradeViewModel submitGradeViewModel)
        {
            var response =  _apiService.PostObjectResponse($"api/submit-grades", submitGradeViewModel.Dto);
            
            submitGradeViewModel.courseId = new Guid(submitGradeViewModel.courseIdParameter);
            submitGradeViewModel.LetterGradeSelect = gradeSelectField.Options;

            if(response.IsSuccessStatusCode)
            {
                ViewBag.Message = "Grade Successfully Saved.";
                return View(submitGradeViewModel);
            }
            return RedirectToAction("Notfound","Home");
        }

        

        public SubmitGradeViewModel CreateViewModel(SubmitGradeViewModel submitGradeViewModel, EnrollmentDto enrollmentDto)
        {
            submitGradeViewModel.Dto = new SubmitGradeDto();
            submitGradeViewModel.courseIdParameter = enrollmentDto.Course?.Id.ToString();
            submitGradeViewModel.Dto.enrollmentId = enrollmentDto.Id.ToString();
            submitGradeViewModel.Dto.LetterGrade = enrollmentDto.LetterGrade;
            submitGradeViewModel.Dto.GradePoint = enrollmentDto.GradePoint;
            submitGradeViewModel.courseId = enrollmentDto.Course.Id;
            return submitGradeViewModel;
        }

     
        

     

    }
}