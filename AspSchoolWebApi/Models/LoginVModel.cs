﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Models
{
    public class LoginVModel
    {

        
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        
    }

    public class ExternalLoginVModel
    {


        [Required]
        [EmailAddress]
        public string Email { get; set; }

      //  [Required]
       // [DataType(DataType.Password)]
        //[Display(Name = "Password")]
       // public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        [Required]

        public string providertoken { get; set; }
        public string nameidentifire { get; set; }
        public string provider { get; set; }



    }

}
