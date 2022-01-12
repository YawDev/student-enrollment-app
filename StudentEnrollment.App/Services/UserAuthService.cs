using System.Net.Mail;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Services;
using StudentEnrollment.Entities;
using StudentEnrollment.Store.Enums;

namespace StudentEnrollment.App.Services
{
    public class UserAuthService : IUserAuthService
    {
        private readonly UserManager<RequestUser> _userManager;
        private readonly SignInManager<RequestUser> _signInManager; 
        private readonly IEmailService _emailService;   
        private readonly IApiService _apiService;       

        public UserAuthService(UserManager<RequestUser> userManager,SignInManager<RequestUser> signInManager, IApiService apiService,
        IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _apiService = apiService;
            _emailService = emailService;
        }

        public bool AuthorizeUser(string url, Permissions Type, Guid urlParameter,ClaimsPrincipal user)
        {
            if(Type is Permissions.StudentPermissions)
                return VerifyStudentAccess(url, urlParameter, user);

            if(Type is Permissions.InstructorPermissions)
                return true;
            if(Type is Permissions.AdminPermissions)
                return true;

            return false;
        }

        private bool VerifyStudentAccess(string url, Guid idParameter, ClaimsPrincipal user)
        {
            var currentUser = _userManager.GetUserAsync(user);
            var response =  _apiService.GetResponse($"{url}/{currentUser.Result.Id}");
            if(response.IsSuccessStatusCode)
            {
                var studentDetails = _apiService.GetDeserializedObject<StudentDetailsDto>(response);
                if(studentDetails.Id != idParameter)
                    return false;
                
                return true;
            }
            return false;
        }


        public bool HasProperPermission(ClaimsPrincipal user, Permissions Type)
        {
            var Permissions = _userManager.GetUserAsync(user).Result.Permission;

            return Permissions == Type;
        }

        public bool IsSignedIn(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }

        

        public void SignOut()
        {
            _signInManager.SignOutAsync();
        }

        public string GetUserid(ClaimsPrincipal user)
        {
            return _userManager.GetUserId(user);
        }

        public async Task SignInAsync(HttpContext httpContext, RequestUser user)
        {
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            await httpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
        }

        public bool VerifyCurrentUser(string userIdParameter, ClaimsPrincipal user)
        {
           var signedInUserId = GetUserid(user);
           return signedInUserId == userIdParameter;
        }

        public async Task<RequestUser> FindUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
            
        }

        public async Task<string> GeneratePasswordResetTokenAsync(RequestUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public Task SendPasswordResetLink(EmailTemplate template)
        {
            var message = _emailService.CreateEmailMessage(template);
            return Task.Run(() => _emailService.SendMessage(message));

        }

        public  Task<IdentityResult> ResetPassword(RequestUser user, string token, string Password)
        {
            return Task.Run(() => _userManager.ResetPasswordAsync(user, token, Password));
        }
    }
}