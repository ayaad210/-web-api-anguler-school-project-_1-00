using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Models
{
    public class AdminRegisterVModel
    {

       
            [Required]
            [EmailAddress]
        
            public string Email { get; set; }

           // [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            
            public string Password { get; set; }
           public string Name { get;  set; }
        public int Age { get; set; }

    }
}
