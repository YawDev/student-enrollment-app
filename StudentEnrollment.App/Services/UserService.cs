using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.App.Models;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Exceptions;
using StudentEnrollment.Entities;
using StudentEnrollment.Services;

namespace StudentEnrollment.App.Services
{
    public class UserService : IUserService 
    {
        private readonly SignInManager<RequestUser> _signInManager;
        private readonly UserManager<RequestUser> _userManager;


        public UserService(SignInManager<RequestUser> signInManager,UserManager<RequestUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        public void Login(LoginDto loginDto)
        {
            var user =  _userManager.FindByNameAsync(loginDto.UserName).Result;
            var result = _userManager.CheckPasswordAsync(user, loginDto.Password).Result;

            if (!result)
                throw new DomainException("Invalid Login");
        }

        public void ResetPassword()
        {
            throw new System.NotImplementedException();
        }
    }
}