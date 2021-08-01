using AspSchoolWebApi.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Authintication
{
    [Serializable]

    public class ApplicationUser:IdentityUser
    {
        public ApplicationUser()
        {
            this.Books = new HashSet<Book>();
            
        }

        public string Name { get; set; }
        public int Age { get; set; }
        public string PhotoFileName { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
