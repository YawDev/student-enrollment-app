using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using StudentEnrollment.App.Mappers;
using StudentEnrollment.Core.Dtos;
using StudentEnrollment.Core.Exceptions;
using StudentEnrollment.Entities;

namespace StudentEnrollment.App.Services
{
    public class UploadService : IUploadService
    {
        private string filepath;
        private IFormFile file;

        public void ValidateFile(IFormFile formFile)
        {
            file = formFile;

            var extension = Path.GetExtension(file.FileName);
            if(extension != ".csv")
                throw new DomainException("File must be of .csv");

            filepath = Path.Combine(Directory.GetCurrentDirectory(), file.FileName);
        }
        public List<UploadCourseDto> GetMappedDtos()
        {
           StreamReader fileReader = new StreamReader(filepath);
           using(var csv = new CsvReader(fileReader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CourseMap>();
                var records = csv.GetRecords<UploadCourseDto>();
                var dtos  = records.ToList();
                return dtos;
            }
        }

        

        public void SaveFile()
        {
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                file.CopyToAsync(stream);
            }
        }

        
    }
}