using System;
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
using StudentEnrollment.Entities;
using StudentEnrollment.Services;

namespace StudentEnrollment.App.Controllers
{
    public class InstructorsController : BaseController
    {
        private readonly ILogger<InstructorsController> _logger;
        private readonly IApiService _apiService;
        private readonly UserManager<RequestUser> _userManager;
        private readonly SignInManager<RequestUser> _signInManager;
        GradeSelectField gradeSelectField;    



        public InstructorsController(ILogger<InstructorsController> logger, IApiService apiService,
        UserManager<RequestUser> userManager, SignInManager<RequestUser> signInManager)
        {
            _logger = logger;
            _apiService = apiService;
            _signInManager = signInManager;
            _userManager=userManager;
            gradeSelectField = new GradeSelectField();
        }
        [HttpGet]
        public IActionResult Register()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Register(AddInstructorDto addInstructorDto)
        {
            var response =  _apiService.PostObjectResponse("api/instructors/register",addInstructorDto);

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index","Home");
            
            return View(addInstructorDto);
        }

        public IActionResult Details(string Id)
        {
            if(_signInManager.IsSignedIn(User))
            {
                var currentUserId = _userManager.GetUserAsync(User).Result.Id;
                if(currentUserId != Id || !Authorize())
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
            if(_signInManager.IsSignedIn(User))
            {
                if(!Authorize()) return RedirectToAction("NotAuthorized","Accounts");
                
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

        public bool Authorize()
        {
                var Permissions = _userManager.GetUserAsync(User).Result.Permission;
                if(Permissions != Store.Enums.Permissions.InstructorPermissions)
                    return false;


                return true;
        }

        

     

    }
}