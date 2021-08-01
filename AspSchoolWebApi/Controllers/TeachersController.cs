using AspSchoolWebApi.Authintication;
using AspSchoolWebApi.Helbers;
using AspSchoolWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TeachersController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly IHostEnvironment _env;

        public TeachersController(SignInManager<ApplicationUser> signManager, UserManager<ApplicationUser> userManager,ApplicationDbContext _Db, IHostEnvironment env)
        {
            db = _Db;
            _signManager = signManager;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet]
        [Route("GetTeachers")]
        [Authorize(Roles = "admin")]

        public async Task<JsonResult> GetTeachersAsync()
        {

            var t = await Task.Factory.StartNew(() =>
            {
                var Teachers = db.Teachers.Include(t => t.user).Include(t => t.Subject).ToList();

                return Teachers;
            });

            return new JsonResult(t);
        }

        // GET: Teachers/Details/5
        [HttpGet()]
        [Route("GetTeacherByid")]
        [Authorize(Roles = "admin,teachers")]

        public async Task<JsonResult> GetTeacherByidAsync(int? TeacherId)
        {
            if (TeacherId == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
            var t = await Task.Factory.StartNew(() =>
            {


                Teacher Teacher = db.Teachers.Where(t=>t.id==TeacherId).Include(t=>t.user).Include(t=>t.Subject).FirstOrDefault();

                return Teacher;
            });


            return new JsonResult(t);

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("CreateTeacher")]
        public async Task<ActionResult> CreateTeacherAsync([FromBody] TeacherVModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }



            var user = new ApplicationUser { Name = model.Name, Age = model.Age, UserName = model.Email, Email = model.Email, PhotoFileName = model.PhotoFileName, EmailConfirmed = true };

            var check = await _userManager.CreateAsync(user, model.Password);

            if (check.Succeeded)
            {

                await _userManager.AddToRoleAsync(user, "teachers");

                Teacher teacher = new Models.Teacher();
                teacher.user = user;
                teacher.Subjectid = model.Subjectid;
                db.Teachers.Add(teacher);

                if ((await db.SaveChangesAsync()) > 0)
                {
                    return Ok(new JsonResult(Helber.ResponseStringsHelber.AddedSucessFully));
                }


                return BadRequest(Helber.ResponseStringsHelber.SavingError);
            }
            else
            {
                return BadRequest(Helber.ResponseStringsHelber.SavingError);

            }


        }

        [HttpPost]
        [Route("saveImage/{old?}")]
        [Authorize]

        public async Task<ActionResult>  saveImageAsync([FromRoute] string old)
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
                        if (old != postedFile.FileName&&old!="anonymous.jpg")//تاكيد ملوش لازمة
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



       [HttpPut]
        [Authorize(Roles = "admin,teachers")]
        [Route("UpdateTeacher")]
        public async Task<ActionResult> UpdateTeacherAsync(TeacherVModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
            var tech =  db.Teachers.Include(t=>t.user).FirstOrDefault(t=>t.id==model.id);

            if (tech == null)
            {
                return BadRequest(Helber.ResponseStringsHelber.NotExist);

            }
            //new object[] { (object)(new { id = tech.user.Id }) }

            ApplicationUser user =await db.Users.FindAsync(tech.user.Id);
            if (user==null)
            {
                return BadRequest(Helber.ResponseStringsHelber.NotExist);

            }
            
            user.PhotoFileName = model.PhotoFileName;
            user.Name = model.Name;
            user.Age=model.Age;
           user.UserName = model.Email;
            user.Email = model.Email;
           var res=  await _userManager.UpdateAsync(user);
            
            if (res.Succeeded)
            {
                tech.Subjectid = model.Subjectid;


                db.Entry(tech).State = EntityState.Modified;

                if ((await db.SaveChangesAsync()) > 0)
                {
                    return Ok(new JsonResult( Helber.ResponseStringsHelber.UpdatedSucessFully));
                }
            }
            


            return BadRequest(Helber.ResponseStringsHelber.SavingError);
        }
        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("DeleteTeatcher")]
        public async Task<ActionResult> DeleteTeatcherAsunc(int Teacherid)
        {
            Teacher Teacher = db.Teachers.Where(t => t.id == Teacherid).Include(t => t.user).FirstOrDefault();


            if (Teacher.STasks.Count <= 0)
            {

                try
                {
                    if (Teacher.user.PhoneNumber!="anonymous.jpg")
                    {
                        FileInfo f = new FileInfo(_env.ContentRootPath + "/photos/" + Teacher.user.PhotoFileName);
                        f.Delete();
                    }
                    
                }
                catch (Exception)
                {


                }

                db.Teachers.Remove(Teacher);
             int res=   await db.SaveChangesAsync();

                if ( res> 0)
                {
                    var t = await _userManager.DeleteAsync(Teacher.user);

                    if (t.Succeeded)
                    {
                        return Ok(new JsonResult(Helber.ResponseStringsHelber.DeletedSucessFully));

                    }

                }
                return BadRequest(Helber.ResponseStringsHelber.SavingError);


            }
            else
            {
                return BadRequest(Helber.ResponseStringsHelber.CanNotDeleted);

            }
        }

    }
}
