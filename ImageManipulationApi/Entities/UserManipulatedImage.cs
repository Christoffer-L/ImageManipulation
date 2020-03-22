using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageManipulationApi.Entities
{
    public class UserManipulatedImage
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public byte[] EncryptedImage { get; set; }
    }
}
