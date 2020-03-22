using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageManipulationApi.Entities.HelperClasses
{
    public class BringPostResponse
    {
        public string result { get; set; }
        public bool valid { get; set; }
        public string postalCodeType { get; set; }
    }
}
