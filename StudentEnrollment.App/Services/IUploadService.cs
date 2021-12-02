using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using StudentEnrollment.Core.Dtos;

namespace StudentEnrollment.App.Services
{
    public interface IUploadService
    {
         void ValidateFile(IFormFile file);

         void SaveFile();
         List<UploadCourseDto> GetMappedDtos();
    }
}