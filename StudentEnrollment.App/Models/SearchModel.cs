using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudentEnrollment.Core.Dtos;

namespace StudentEnrollment.App.Models
{
    public class SearchModel
    {
        public string Department { get; set; }
        public string Section { get; set; }
        public string Instructor { get; set; }
        public string CourseName { get; set; }
        public string Abbreviation { get; set; }

    }

    public class SearchResultsModel
    {
        public List<CourseDto> results { get; set; }
        
        
    }
}