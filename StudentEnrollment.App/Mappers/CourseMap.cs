
using CsvHelper.Configuration;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Exceptions;

namespace StudentEnrollment.App.Mappers
{
    public class CourseMap : ClassMap<UploadCourseDto>
    {
        public CourseMap()
        {
            Map(m => m.Name).Name("Name").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.Title).Name("Title").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.Abbreviation).Name("Abbreviation").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.Capacity).Name("Capacity").Validate(x => int.TryParse(x.Field, out int result));
            Map(m => m.Credits).Name("Credits").Validate(x => decimal.TryParse(x.Field, out decimal result));
            Map(m => m.CourseNumber).Name("CourseNumber").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.Department).Name("Department").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.Section).Name("Section").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.StartTime).Name("StartTime").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.EndTime).Name("EndTime").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.InstructorFirstName).Name("InstructorFirstName").Validate(field => !string.IsNullOrEmpty(field.Field));
            Map(m => m.InstructorLastName).Name("InstructorLastName").Validate(field => !string.IsNullOrEmpty(field.Field));
        }
    }
}