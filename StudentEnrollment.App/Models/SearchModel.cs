using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StudentEnrollment.Core.Dtos;

namespace StudentEnrollment.App.Models
{
    public class SearchModel
    {
        public string Keywords { get; set; }
        
        

    }

    public class SearchResultsModel
    {
        public string keyword { get; set; } 
        public List<CourseDto> results { get; set; }
        
        
    }
}