using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;
using ControllerHelpers;
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
    public class DepartmentsController : BaseController
    {
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IApiService _apiService;
        private readonly SignInManager<RequestUser> _signInManager;
        private readonly UserManager<RequestUser> _userManager;

       
        public DepartmentsController(ILogger<DepartmentsController> logger, IApiService apiService,
        SignInManager<RequestUser> signInManager, UserManager<RequestUser> userManager)
        {
            _logger = logger;
            _apiService = apiService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var currentUser = _signInManager.IsSignedIn(User);
                if(currentUser)
                {
                    var response =  _apiService.GetResponse("api/departments");
                    if(response.IsSuccessStatusCode)
                    {
                        var departments =  _apiService.GetDeserializedObject<List<DepartmentDto>>(response);
                        return View(new DepartmentsViewModel(){Departments=departments, AddDepartmentDto= new AddDepartmentDto()});
                    }
                    var result = _apiService.GetApiResultMessage(response);
                    return View(new DepartmentsViewModel());
                }
                return RedirectToAction("Login","Accounts");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("ServerError","Home");
            }
            
        }

        [HttpGet]

        public IActionResult Add()
        {
            var currentUser = _signInManager.IsSignedIn(User);
            if(currentUser)
            {
               var isAuthorized = Authorize();
               if(!isAuthorized)
                    return RedirectToAction("NotAuthorized", "Accounts");

                return View();
            }
            return RedirectToAction("Login","Accounts");
        }

        [HttpPost]
        public IActionResult Add(AddDepartmentDto departmentDto)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var response =  _apiService.PostObjectResponse("api/departments/add", departmentDto);

                    if(response.IsSuccessStatusCode)
                       return RedirectToAction("Index");
                    
                    ViewBag.Message = _apiService.GetApiResultMessage(response);
                    
                    return View(departmentDto);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError","Home");
                }
            }
            return View("Add", departmentDto);
        }

        public IActionResult Delete(Guid id)
        {
            try
            {
                if(_signInManager.IsSignedIn(User))
                {
                    if(!Authorize()) 
                        return RedirectToAction("NotAuthorized", "Accounts");

                        var response =  _apiService.GetResponse($"api/departments/details/{id}");
                        if(response.IsSuccessStatusCode)
                        {
                            var department = _apiService.GetDeserializedObject<DepartmentDetailsDto>(response);
                            return View(department);
                        }
                    return RedirectToAction("Notfound","Home");
                    }
                    return RedirectToAction("NotAuthorized", "Accounts");
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError","Home");
                }
        }


        [HttpPost]
        public IActionResult Delete(DepartmentDetailsDto departmentDto)
        {
            try
            {
                var ApiUrl = $"api/departments/delete/{departmentDto.Id}";
                var response = _apiService.DeleteResponse(ApiUrl);

                if(response.IsSuccessStatusCode)
                    return RedirectToAction("Index","Departments");
                
                return RedirectToAction("Notfound","Home");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError","Home");
            }
        }

    
        public IActionResult Details(Guid id)
        {
            try
                {
                    if(_signInManager.IsSignedIn(User))
                    {
                        var response =  _apiService.GetResponse($"api/departments/details/{id}");

                        if(response.IsSuccessStatusCode)
                        {
                            var department = _apiService.GetDeserializedObject<DepartmentDetailsDto>(response);
                            return View(department);
                        }
                    return RedirectToAction("Notfound","Home");
                    }
                    return RedirectToAction("Login","Accounts");
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return RedirectToAction("ServerError","Home");
                }
        }

       

        public bool Authorize()
        {
                var Permissions = _userManager.GetUserAsync(User).Result.Permission;
                if(Permissions != Store.Enums.Permissions.AdminPermissions)
                    return false;

                return true;
        }
    }
}
