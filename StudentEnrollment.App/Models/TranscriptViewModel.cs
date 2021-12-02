using System;
using System.Collections.Generic;
using StudentEnrollment.Core.Dtos;

namespace StudentEnrollment.App.Models
{
    public class TranscriptViewModel
    {
        public Guid StudentId { get; set; }
        
        
        public List<EnrollmentDto> Enrollments { get; set; }  
    }
}