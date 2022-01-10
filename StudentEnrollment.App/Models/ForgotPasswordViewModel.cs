using System.ComponentModel.DataAnnotations;

namespace StudentEnrollment.App.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}