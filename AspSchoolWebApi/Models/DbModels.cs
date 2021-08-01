using AspSchoolWebApi.Authintication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using  AspSchoolWebApi.Models;

namespace AspSchoolWebApi.Models
{
  
        public partial class Contact
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Email { get; set; }

            public string Message { get; set; }
        }
        public partial class Group
        {
            public Group()
            {
                this.Students = new HashSet<Student>();
                this.STasks = new HashSet<STask>();
            }

            public int id { get; set; }
            public string Name { get; set; }
            public Semester Semester { get; set; }
        public int SemesterId { get; set; }



        public virtual ICollection<Student> Students { get; set; }
            public virtual ICollection<STask> STasks { get; set; }
            
        } //
        public partial class Student
        {
            public Student()
            {
                this.Answers = new HashSet<Answer>();
                this.Groups = new HashSet<Group>();
            }
            public ApplicationUser user { get; set; }
       
            public int id { get; set; }
            //    public int Userid { get; set; }
            public int Points { get; set; }
         //   public virtual Group CurrentGroup { get; set; }
            public Parent Parent { get; set; }
        
        public int? ParentId { get; set; }
        public int CurrentSemesterId { get; set; }
        public virtual ICollection<Group> Groups { get; set; }


            public virtual ICollection<Answer> Answers { get; set; }
        } //

        public partial class Parent
        {
            public Parent()
            {
                this.Children = new HashSet<Student>();
            }
            public int Id { get; set; }
            public ApplicationUser User { get; set; }
            public virtual ICollection<Student> Children { get; set; }
        }
    [Serializable]

    public partial class Subject
        {
            public Subject()
            {
                this.Teachers = new HashSet<Teacher>();
                this.Semesters = new HashSet<Semester>();
            }
            
            public int id { get; set; }
            public string Name { get; set; }
            public virtual ICollection<Semester> Semesters { get; set; }



            public virtual ICollection<Teacher> Teachers { get; set; }
        }
        public partial class STask
        {
            public STask()
            {
                this.Answers = new HashSet<Answer>();
            }

            public int id { get; set; }
            public string Name { get; set; }

            public string Content { get; set; }
            public string Notes { get; set; }
            public string Type { get; set; }
            public decimal Total { get; set; }

            public virtual ICollection<Answer> Answers { get; set; }
            public virtual Group Group { get; set; }
            public int Groupid { get; set; }


            public virtual Teacher Teacher { get; set; }
            public int Teacherid { get; set; }

        }
    [Serializable]

    public partial class Teacher
        {

            public Teacher()
            {
                this.STasks = new HashSet<STask>();
            }
            public ApplicationUser user { get; set; }

            public int id { get; set; }
            //    public int Userid { get; set; }

            public virtual Subject Subject { get; set; }

            public int Subjectid { get; set; }

            public virtual ICollection<STask> STasks { get; set; }
        }
        //public partial class Semester
        //{
        //    public Semester()
        //    {
        //        this.Groups = new HashSet<Group>();
        //        this.SemestersSubjects = new HashSet<SemestersSubject>();
        //    }

        //    public int id { get; set; }
        //    public string Name { get; set; }

        //    public virtual ICollection<Group> Groups { get; set; }
        //    public virtual ICollection<SemestersSubject> SemestersSubjects { get; set; }
        //}
        //public partial class SemestersSubject
        //{
        //    public int id { get; set; }
        //    public string ClassYear { get; set; }

        //    public virtual Semester Semester { get; set; }
        //    public int semesterid { get; set; }

        //    public virtual Subject Subject { get; set; }
        //    public int Subjectid { get; set; }

        //}
        public class Semester
        {
            public Semester()
            {
                this.Groups = new HashSet<Group>();
                this.SemestersSubjects = new HashSet<Subject>();
            }
        public int Id { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
            public virtual ICollection<Subject> SemestersSubjects { get; set; }

            

            [Required]    
            public string SemesterName { get; set; }
            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

        }
        public partial class Answer
        {
            public int Id { get; set; }
            public string AnswerContent { get; set; }
            public Nullable<decimal> Degree { get; set; }
            public System.DateTime Datetime { get; set; }

            public virtual Student Student { get; set; }
            public int StudentId { get; set; }

            public virtual STask STask { get; set; }
            public int STaskid { get; set; }

        }

        public partial class Book
        {
            public int Id { get; set; }
            public ApplicationUser User { get; set; }

            public string BookName { get; set; }
            public string Filepath { get; set; }
            public string Imagepath { get; set; }

            public string Description { get; set; }
            public int Points { get; set; }
        public  int NumberOfDownloads { get; set; }
        public System.DateTime UploadDate { get; set; }
        }

    }


