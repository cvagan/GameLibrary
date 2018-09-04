using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Helpers
{
    public class ImageValidation
    {
        public string ErrorMessage { get; set; }
        public decimal FileSize { get; set; }

        public string FileCheck(IFormFile file)
        {
            var supportedTypes = new[] { "jpg", "jpeg", "gif", "png" };
            var fileExtension = System.IO.Path.GetExtension(file.FileName).Substring(1);

            if (!supportedTypes.Contains(fileExtension))
            {
                ErrorMessage = "The file must be an image";
                return ErrorMessage;
            }
            else if (file.Length > (FileSize * 1024))
            {
                ErrorMessage = $"Filesize cannot exceed {FileSize}Kb";
                return ErrorMessage;
            }

            return "ok";
        }
    }
}
