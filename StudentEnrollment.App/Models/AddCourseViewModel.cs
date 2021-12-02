using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using StudentEnrollment.Core.Dtos;

namespace StudentEnrollment.App.Models
{
   
    public class AddCourseViewModel
    {
        public AddCourseDto Dto { get; set; }

        public List<InstructorDto> InstructorsListItems { get; set; }

        public List<string> Start { get; set; }

        public List<string> End { get; set; }
    }

    public class EnrollViewModel
    {
        public EnrollCourseDto Dto { get; set; }

        public Guid Id {get;set;}
    }

    public class SubmitGradeViewModel
    {
        public Guid courseId { get; set; }
        public string courseIdParameter { get; set; }
        public SubmitGradeDto Dto { get; set; }
        public Guid Id {get;set;}
        public List<string> LetterGradeSelect { get; set; }
        
        
        
    }

    public class UploadCoursesViewModel
    {
        public IFormFile  FormFile { get; set; }
        
        
    }

    public class UploadStatusViewModel
    {
        public List<UploadCoursesLogDto> uploads { get; set; }
        
    }

    public class UploadErrorsViewModel
    {
        public List<UploadCourseErrorDto> Errors { get; set; }
        
    }

        
        
        
    
}