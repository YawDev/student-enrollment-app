using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StudentEnrollment.App.Controllers;
using StudentEnrollment.App.Models;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Services;

namespace ControllerHelpers
{

    public class ApiToView : BaseController
    {

        public IActionResult GetRequestView<Obj>(Obj objectViewModel)
        {
            
            return View(objectViewModel);
        }
    }
}