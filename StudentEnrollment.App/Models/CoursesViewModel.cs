using System;
using System.Collections.Generic;
using StudentEnrollment.Core.Dtos;

namespace StudentEnrollment.App.Models
{
    public class CoursesViewModel
    {
        public List<CourseDto> Courses { get; set; }
    }

    public class FilesViewModel
    {
        public string Id {get;set;}
        public List<FileQueryResultDto> Files { get; set; }
        
    }
}