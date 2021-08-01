using AspSchoolWebApi.Authintication;
using AspSchoolWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspSchoolWebApi.Helbers;
using Microsoft.Extensions.Hosting;
using System.IO;


namespace AspSchoolWebApi.Controllers
{[ApiController]
    [Route("[controller]")]

    public class StudentController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly ApplicationDbContext db;
     private readonly   IHostEnvironment _env;
        public StudentController(SignInManager<ApplicationUser> signManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _db, IHostEnvironment env)
        {
            _signManager = signManager;
            _userManager = userManager;
            db = _db;
            _env = env;

        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("GetAllStudent")]
        public async Task<JsonResult> GetAllStudentAsync()
        {

            var m = await Task.Factory.StartNew(() => { return db.Students.Include(s => s.user).ToList(); });



            return new JsonResult(m);
        }





        [Authorize]
        [HttpGet]
        [Route("GetStudentBySemesterid/{semesterid?}")]
        public async Task<JsonResult> GetStudentBySemesteridAsync([FromRoute] int? semesterid)
        {

            var m = await Task.Factory.StartNew(() => { return db.Students.Include(s => s.user).Where(s=>s.CurrentSemesterId== semesterid).ToList(); });



            return new JsonResult(m);
        }

        [Authorize(Roles = "Teachers,admin,parents")]

        [HttpGet]
        [Route("GetGroupStudents/{GroupId?}")]
        public async Task<JsonResult> GetGroupStudentsAsync([FromRoute] int GroupId)
        {

            var m = await Task.Factory.StartNew(() => { return db.Students.Where(s => s.Groups.Where(g=>g.id==GroupId).Count()>0).Include(s => s.user).ToList(); });



            return new JsonResult(m);
        }

        [Authorize(Roles = "admin,Parents")]

        [Route("GetParenStudents/{ParentId?}")]
            [HttpGet]
        public async Task<JsonResult> GetParenStudentsAsync(int ParentId)
        {

            var m = await Task.Factory.StartNew(() => { return db.Students.Where(s => s.Parent.Id == ParentId).Include(s => s.user).ToList(); });



            return new JsonResult(m);
        }

        [Route("GetStudentprogressbygroupid")]
        [Authorize(Roles = "Teachers,admin,Parents,students")]
        [HttpGet]
        public async Task<JsonResult> GetStudentprogressbygroupidAsync([FromQuery] int StudentId)
        {
            var t = await Task.Factory.StartNew(() =>
            {


                //   db.Students
                //.Include("Group.stask.teacher")
                //.Include("Employee.Employee_Country");

                #region test

                //var test = db.Students.Include(s => s.Groups).Join(db.Groups, s => s.Groups.LastOrDefault(), g => g, (s, g) => new { s, g }).Join(db.Tasks, re => re.g.id, t => t.Groupid, (re, t) => new { re.s, re.g, t }).Join(db.Teachers.Include(tech => tech.user), res => res.t.Teacherid, tech => tech.id, (res, tech) => new { res.s, res.g, res.t, tech }).Join(db.Users, res3 => res3.tech.user, u => u, (res3, u) => new { res3.s, res3.g, res3.t, res3.tech, u }).Join(db.Subjects, res4 => res4.tech.Subjectid, sub => sub.id, (res4, sub) => new { student = res4.s, studentgroup = res4.g, studntgrouptask = res4.t, teacheroftask = res4.tech, techeruser = res4.u, techersubjct = sub }).Where(final => final.student.id == StudentId); ;


                //var test2 = test.GroupBy(x => x.techersubjct);



                //List<StudentProgressForsubjectModel> prolist = new List<Models.StudentProgressForsubjectModel>();

                //foreach (var item in test2)
                //{
                //    decimal sumoftotal = 0;
                //    decimal sumofdegree = 0;

                //    StudentProgressForsubjectModel pro = new StudentProgressForsubjectModel();
                //    pro.submodel = new List<studentprogressmodel>();
                //    pro.Subject = item.Key;

                //    foreach (var x in item)
                //    {
                //        studentprogressmodel model = new studentprogressmodel();


                //        Answer ans = db.Answers.Where(a => a.STaskid == x.studntgrouptask.id && a.StudentId == StudentId).FirstOrDefault();
                //        model.task = (x.studntgrouptask);
                //        model.ans = ans;
                //        if (ans != null && ans.Degree != null)
                //        {
                //            sumoftotal +=  x.studntgrouptask.Total;
                //            decimal z = 0;
                //            decimal.TryParse(ans.Degree.ToString(), out z;
                //            sumofdegree += z;
                //        }
                //        pro.totalofsubject = sumoftotal;
                //        pro.sumofdegrees = sumofdegree;

                //        pro.submodel.Add(model);


                //    }



                //    prolist.Add(pro);
                //}
                #endregion


                var student = (db.Students.Include(ss => ss.Groups).ThenInclude(g => g.STasks).ThenInclude(s=>s.Teacher).ThenInclude(t=>  t.Subject    )). Where(ss => ss.id == StudentId).FirstOrDefault();

                var studengroups = student.Groups.LastOrDefault();

              // var tasksoflaasgroup = db.Groups.Where(g => g.id == lastgruop.id).Include(g => g.STasks).FirstOrDefault(); 
                ///Student s = s1.STasks.Select(t =>new { t.Teacher.user ,t.Teacher.Subject,t.Total,t.Answers,t.Type})).
              
               // List<STask> tasks = s.Groups.FirstOrDefault().STasks.ToList();


                var subjecttasks = studengroups.STasks.GroupBy(t => t.Teacher.Subject);
                List<StudentProgressForsubjectModel> prolist = new List<Models.StudentProgressForsubjectModel>();

                foreach (var item in subjecttasks)
                {
                    decimal sumoftotal = 0;
                    decimal sumofdegree = 0;

                    StudentProgressForsubjectModel pro = new StudentProgressForsubjectModel();
                    pro.submodel = new List<studentprogressmodel>();
                    pro.Subject = item.Key;
                    foreach (var task in item)
                    {
                        studentprogressmodel model = new studentprogressmodel();
                        

                        Answer ans = db.Answers.Where(a=>a.STaskid==task.id&& a.StudentId == StudentId).FirstOrDefault();
                        model.task=(task);
                        model.ans = ans;
                        if (ans!=null&&ans.Degree!=null)
                        {
                            sumoftotal += task.Total;
                            decimal x = 0;
                            decimal.TryParse(ans.Degree.ToString(), out x);
                            sumofdegree += x;
                        }
                        pro.totalofsubject = sumoftotal;
                        pro.sumofdegrees = sumofdegree;

                        pro.submodel.Add(model);


                    }
                    

                   
                    prolist.Add(pro);
                }
                return prolist;


            });


            return new JsonResult(t);
        }


       

       
        [HttpPost]       
[AllowAnonymous]     
        [Route("CreateStudent")]
        public async Task<ActionResult> CreateStudentAsync(StudentVModel model)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var user = new ApplicationUser { Name = model.Name, Age = model.Age, UserName = model.Email, Email = model.Email, PhotoFileName = model.PhotoFileName , EmailConfirmed = true };

                        var check = await _userManager.CreateAsync(user, model.Password);

                        if (check.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "students");
                            Student student = new Student();
                            student.Points = 0;
                            student.user = user;



                            db.Students.Add(student);
                            var res = await db.SaveChangesAsync();
                            if (res > 0)
                            {
                                return Ok(new JsonResult(Helber.ResponseStringsHelber.AddedSucessFully));
                            }
                            else
                            {
                                return BadRequest(Helber.ResponseStringsHelber.SavingError);
                            }


                        }
                        else
                        {
                            return BadRequest(Helber.ResponseStringsHelber.SavingError);

                        }
                    }
                    catch (Exception)
                    {

                        transaction.Rollback();
                        return BadRequest(Helber.ResponseStringsHelber.SavingError);
                    }


                }

            }
            else
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);

            }
        }
      
        [HttpGet]
        [Authorize(Roles = "admin,teachers")]
        [Route("GetStudentById/{StudentId?}")]
        public async Task<JsonResult> GetStudentByIdAsync([FromRoute] int? StudentId)
        {
            if (StudentId == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }



         

       var tres=await     Task.Factory.StartNew(() => {
           Student student = db.Students.Include(ss => ss.user).Include(s => s.Groups).Where(ss => ss.id == StudentId).FirstOrDefault();
           // List<SemestersSubject>  subs=  student.Group.Semester.s


           if (student == null)
           {
               return null;
           }
           ApplicationUser user = db.Users.Find(student.user.Id);
           StudentVModel model = new StudentVModel();
           model.Points = student.Points;
           model.Name = user.Name;
           model.StudentID = student.id;
           model.userid = user.Id;
           model.Password = user.PasswordHash;
           model.ConfirmPassword = user.PasswordHash;
           model.Email = user.Email;
           model.Age = user.Age;

           model.LastGroup = student.Groups.Last();

           return model;
       });
            if (tres==null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NotExist);
            }

            return new JsonResult(tres);
        }
        [HttpPost]
        [Route("saveImage/{old?}")]
        [Authorize]
        public async Task<ActionResult> saveImageAsync([FromRoute] string old)
        {
            #region save image



            string photofileName = await Task.Factory.StartNew(() =>
            {
                try
                {


                    var httprequest = Request.Form;//من هناك هبعت الصورة فشكل فورم داتا
                    var postedFile = httprequest.Files[0];

                    try
                    {
                        if (old != postedFile.FileName && old != "anonymous.jpg")//تاكيد ملوش لازمة
                        {

                            //مسح القدسم
                            FileInfo f = new FileInfo(_env.ContentRootPath + "/photos/" + old);
                            f.Delete();
                        }
                        else
                        {
                            return old;
                        }

                    }
                    catch (Exception)
                    {


                    }

                    string newfileName = postedFile.FileName;
                    var physicalpath = _env.ContentRootPath + "/photos/" + newfileName;
                    using (var stream = new FileStream(physicalpath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }

                    return newfileName;
                }
                catch (Exception)
                {
                    return "anonymous.jpg";


                }

            });




            return Ok(new JsonResult(photofileName));
            #endregion
        }

        [Authorize(Roles = "admin,students")]
        [HttpPut]
        [Route("UpdateStudent")]

        public async Task<ActionResult> UpdateStudentAsync(StudentVModel model)
        {
            if (ModelState.IsValid)
            {


                Student st = db.Students.Include(s => s.user).FirstOrDefault(s => s.id == model.StudentID);
                ApplicationUser user = st.user;
                user.Name = model.Name;
                user.Age = model.Age;
                user.PhotoFileName = model.PhotoFileName;
                st.CurrentSemesterId = model.CurrentSemmesterID;
                st.Points = model.Points;

                var res = await _userManager.UpdateAsync(user);

                if (res.Succeeded)
                {
                    db.Entry(st).State = EntityState.Modified;
                    st.user = user;
                    int x = await db.SaveChangesAsync();

                    if (x > 0)
                    {
                        return Ok(new JsonResult(Helber.ResponseStringsHelber.UpdatedSucessFully));
                    }
                }
                return BadRequest(Helber.ResponseStringsHelber.SavingError);










            }
            else
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);

            }

        }


        

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("DeleteStudent/{StudentId?}")]
        
        public async Task <ActionResult> DeleteStudentAsync([FromRoute] int? StudentId)
        {
            if (StudentId == null)
            {
                return BadRequest(Helber.ResponseStringsHelber.NullParams);
            }

            Student student = db.Students.Include(ss => ss.user).Include(ss => ss.Groups).FirstOrDefault();
            ApplicationUser user = student.user;
            

            db.Students.Remove(student);
           if ((await db.SaveChangesAsync()) > 0)
            {
               var res= await _userManager.DeleteAsync(user);

                if (res.Succeeded)
                {

                    return Ok(new JsonResult( Helber.ResponseStringsHelber.DeletedSucessFully));
                }

            }
           
                return BadRequest(Helber.ResponseStringsHelber.SavingError);

            






        }

        public class groupsstudent
        {
            public int studentid { get; set; }
            public int groupid { get; set; }
        }

        [HttpPost]
        [Route("InsertStudentTogroup")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> InsertStudentTogroupAsync(groupsstudent model)
        {

            try
            {
                if (model.studentid == 0 || model.groupid == 0)
                {
                    return BadRequest(Helber.ResponseStringsHelber.NullParams);
                }
                Group per = db.Groups.Include(p => p.Students).Where(p => p.id == model.groupid).FirstOrDefault();


                Student st = db.Students.Include(p => p.user).Where(p => p.id == model.studentid).FirstOrDefault();

                per.Students.Add(st);




                if (await db.SaveChangesAsync() > 0)
                {
                    return Ok(new JsonResult(Helber.ResponseStringsHelber.AddedSucessFully));
                }

                else
                {
                    return BadRequest(Helber.ResponseStringsHelber.SavingError);

                }

            }
            catch (Exception)
            {

                return BadRequest(Helber.ResponseStringsHelber.SavingError);
            }
        }

        [HttpDelete]
        [Route("DeleteStudentfromgroup")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteStudentfromgroupAsync(int groupid, int StudentId)
        {
            try
            {
                if (groupid == 0 || StudentId == 0)
                {
                    return BadRequest(Helber.ResponseStringsHelber.NullParams);

                }
                Group per = db.Groups.Include(p => p.Students).Where(p => p.id == groupid).FirstOrDefault();


                Student st = db.Students.Include(p => p.user).Where(p => p.id == StudentId).FirstOrDefault();
                per.Students.Remove(st);
                if (await db.SaveChangesAsync() > 0)
                {
                    return Ok(new JsonResult(Helber.ResponseStringsHelber.DeletedSucessFully));
                }

                else
                {
                    return BadRequest(Helber.ResponseStringsHelber.SavingError);

                }

            }
            catch (Exception)
            {

                return BadRequest(Helber.ResponseStringsHelber.SavingError);

            }

        }


    }
}
