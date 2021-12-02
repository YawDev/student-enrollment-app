using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentEnrollment.App.Services;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Entities;

namespace StudentEnrollment.App.Controllers
{
    public class AdminController : BaseController
    {
      
        private readonly ILogger<AccountsController> _logger;
        private readonly IApiService _apiService;       

        public AdminController(ILogger<AccountsController> logger,IApiService ApiService)
        {
          
            _logger = logger;
            _apiService = ApiService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Register(AddAdminDto addAdminDto)
        {
            var response =  _apiService.PostObjectResponse("api/admin/register", addAdminDto);

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index","Home");
            
            return View(addAdminDto);
        }
    }
}