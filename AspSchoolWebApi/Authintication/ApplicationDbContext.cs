
using AspSchoolWebApi.Authintication;
using AspSchoolWebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AspSchoolWebApi.Authintication
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public virtual DbSet<Parent> Parents { get; set; }

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<STask> Tasks { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
