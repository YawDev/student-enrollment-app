using System;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentEnrollment.App.Services;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Services;
using StudentEnrollment.Entities;
using StudentEnrollment.Store.Enums;

namespace StudentEnrollment.App.Controllers
{
    public class AdminController : BaseController
    {
      
        private readonly ILogger<AccountsController> _logger;
        private readonly IApiService _apiService;     
        private readonly IUserAuthService _userAuthService;       
  

        public AdminController(ILogger<AccountsController> logger,IApiService ApiService,IUserAuthService userAuthService)
        {
          
            _logger = logger;
            _apiService = ApiService;
            _userAuthService = userAuthService;
        }

        [HttpGet]
        public IActionResult Details(string Id)
        {
            if(_userAuthService.IsSignedIn(User))
            {
                var currentUserId = _userAuthService.GetUserid(User);
                if(currentUserId != Id || !_userAuthService.HasProperPermission(User, Permissions.AdminPermissions))
                        return RedirectToAction("NotAuthorized","Accounts");

                var response =  _apiService.GetResponse($"api/admin-portal/{Id}");
                if(response.IsSuccessStatusCode)
                {
                    var adminDto = _apiService.GetDeserializedObject<AdminDetailsDto>(response);
                    return View(adminDto);
                }
                return RedirectToAction("Notfound","Home");
            }
            return RedirectToAction("Login","Accounts");
        }

        [HttpGet]
        public IActionResult Register()
        {
             if(_userAuthService.IsSignedIn(User))
                return RedirectToAction("Index", "Departments");
            return View();
        }

        [HttpPost]
        public IActionResult Register(AddAdminDto addAdminDto)
        {
            try
            {
                var response =  _apiService.PostObjectResponse("api/admin/register", addAdminDto);

                if(response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    ViewBag.Message = "Something went wrong while creating account."; return View(addAdminDto); 
                }

                if(response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = _apiService.GetApiResultMessage(response);
                    ViewBag.Message = message; return View(addAdminDto); 
                } 

                if(response.StatusCode == HttpStatusCode.Created)
                {
                    ViewBag.Message = "Sign up has been successful"; return View(addAdminDto); 
                }        
            
                return View(addAdminDto);
            }
            catch(Exception ex)
            {
                 _logger.LogError(ex.Message);
                return RedirectToAction("ServerError","Home");
            }
        }
    }
}