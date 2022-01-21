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
    }
}