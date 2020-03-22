using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageManipulationApi.Entities.HelperClasses
{
    public class UserImageInfo
    {
        public IFormFile UserFile { get; set; }
        public string PostalCodeAreaName { get; set; }
    }
}
