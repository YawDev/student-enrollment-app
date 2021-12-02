using System.Collections.Generic;
using StudentEnrollment.Core.Dtos;

namespace StudentEnrollment.App.Models
{
    public class DepartmentsViewModel
    {
        public List<DepartmentDto> Departments { get; set; }
        public AddDepartmentDto AddDepartmentDto { get; set; }
        public UpdateDepartmentDto UpdateDepartmentDto { get; set; }
    
    }
}