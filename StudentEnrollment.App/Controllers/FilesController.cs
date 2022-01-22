using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentEnrollment.App.Services;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Services;
using StudentEnrollment.Store.Enums;

namespace StudentEnrollment.App.Controllers
{
    public class FilesController : BaseController
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IApiService _apiService;
        private readonly IUserAuthService _userAuthService;
       
        public FilesController(ILogger<FilesController> logger, IApiService apiService,
        IUserAuthService userAuthService)
        {
            _logger = logger;
            _apiService = apiService;
            _userAuthService = userAuthService;
        }

        [HttpGet]
        public IActionResult Index(string id)
        {
            if(_userAuthService.IsSignedIn(User))
            {
                var currentUserId = _userAuthService.GetUserid(User);
                if(currentUserId != id || !_userAuthService.HasProperPermission(User, Permissions.AdminPermissions))
                        return RedirectToAction("NotAuthorized","Accounts");

                var response =  _apiService.GetResponse($"api/files/{id}");
                if(response.IsSuccessStatusCode)
                {
                    var results = _apiService.GetDeserializedObject<List<FileQueryResultDto>>(response);
                    return View(results);
                }
                return RedirectToAction("Notfound","Home");
            }
            return RedirectToAction("Login","Accounts");
        }


        [HttpGet]
        public IActionResult ViewFile(Guid id)
        {
            if(_userAuthService.IsSignedIn(User))
            {
                var currentUserId = _userAuthService.GetUserid(User);
                if(!_userAuthService.HasProperPermission(User, Permissions.AdminPermissions))
                        return RedirectToAction("NotAuthorized","Accounts");

                var response =  _apiService.GetResponse($"api/filecontent/{id}");
                if(response.IsSuccessStatusCode)
                {
                    var fileDto = _apiService.GetDeserializedObject<FileContentsDto>(response);
                    return View(fileDto);
                }
                return RedirectToAction("Notfound","Home");
            }
            return RedirectToAction("Login","Accounts");
        }


        public IActionResult Delete(Guid id)
        {
            try
            {
                if(_userAuthService.IsSignedIn(User))
                {
                    if(!_userAuthService.HasProperPermission(User, Permissions.AdminPermissions)) 
                        return RedirectToAction("NotAuthorized", "Accounts");

                        var response =  _apiService.GetResponse($"api/filecontent/{id}");
                        if(response.IsSuccessStatusCode)
                        {
                            var file = _apiService.GetDeserializedObject<FileContentsDto>(response);
                            return View(file);
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
        public IActionResult ConfirmDelete(FileContentsDto file)
        {
            try
            {
                if(_userAuthService.IsSignedIn(User))
                {
                    if(!_userAuthService.HasProperPermission(User, Permissions.AdminPermissions)) 
                        return RedirectToAction("NotAuthorized", "Accounts");

                        var response =  _apiService.DeleteResponse($"api/uploads/delete/{file.Id}");
                        if(response.IsSuccessStatusCode)
                            return RedirectToAction("Index",new{id = file.RequestUserId});

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

    }
}