using AspSchoolWebApi.Authintication;
using AspSchoolWebApi.Helbers;
using AspSchoolWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {

        private readonly ApplicationDbContext db;

        public SubjectsController(ApplicationDbContext _Db)
        {
            db = _Db;
        }

         [HttpGet("[action]")]
        [Authorize]
        [Route("GetSubjectsBystudentid/{studentid?}")]

        public async Task<JsonResult> GetSubjectsBystudentidAsync(int studentid)
        {

            var t = await Task.Factory.StartNew(() =>
            {
                Student st = db.Students.Find(studentid);
                return db.Semesters.Include(s => s.SemestersSubjects).FirstOrDefault(s => s.Id == st.CurrentSemesterId).SemestersSubjects;
            });

            return new JsonResult(t);
        }





    

        [HttpGet("[action]")]
        [AllowAnonymous]
        [Route("GetSubjects")]


        public async Task<JsonResult>  GetSubjectsAsync()
        {

            var t = await Task.Factory.StartNew(() =>
             {
                 return db.Subjects.ToList();
             });

            return new JsonResult(t);
        }

        // GET: Subjects/Details/5
        [HttpGet]
        [Route("GetSubjectByid")]
        [Authorize(Roles = "admin,teachers")]

        public async Task<JsonResult> GetSubjectByidAsync(int? SubjectId)
        {
            if (SubjectId == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
            var t =await Task.Factory.StartNew(() =>
              {


                  Subject subject = db.Subjects.Find(SubjectId);
                 
                  return subject;
              });


            return new JsonResult (t);

        }

        [HttpGet]
        [Route("GetSubjectBygroupid")]
        [Authorize(Roles = "admin,teachers")]

        public async Task<JsonResult> GetSubjectBygroupidAsync(int? groupid)
        {
            if (groupid == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
            var t = await Task.Factory.StartNew(() =>
            {


                Group group = db.Groups.Where(g=>g.id==groupid).Include(g=>g.Semester.SemestersSubjects).FirstOrDefault();
                
                return group.Semester.SemestersSubjects;
            });


            return new JsonResult(t);

        }
        [HttpPost("[action]")]

        [Authorize(Roles = "admin")]
        [Route("CreateSubject")]
        public async Task<ActionResult> CreateSubjectAsync(Subject subject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest( Helber.ResponseStringsHelber.ValidationError);
            }
              await  db.Subjects.AddAsync(subject);
            
            if ((await db.SaveChangesAsync())>0)
            {
                return Ok(new JsonResult(Helber.ResponseStringsHelber.AddedSucessFully));
            }
           

            return BadRequest(Helber.ResponseStringsHelber.SavingError);
        }

       
        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("UpdateSubject")]
        public async Task<ActionResult> UpdateSubjectAsync(Subject subject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
            var sub = await db.Subjects.FindAsync(subject.id);
            if (sub==null)
            {
                return BadRequest( Helber.ResponseStringsHelber.NotExist);

            }
            db.Entry(sub).State = EntityState.Modified;

            if ((await db.SaveChangesAsync()) > 0)
            {
                return Ok(new JsonResult( Helber.ResponseStringsHelber.UpdatedSucessFully));
            }


            return BadRequest(Helber.ResponseStringsHelber.SavingError) ;
        }
        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("DeleteSubject")]
        public async Task<ActionResult> DeleteSubjectAsync(int Subjectid)
        {
            Subject subject = db.Subjects.Find(Subjectid);
            if (subject.Teachers.Count <= 0)
            {
                db.Subjects.Remove(subject);
                if ((await db.SaveChangesAsync())>0)
                {
                    return Ok(new JsonResult( Helber.ResponseStringsHelber.DeletedSucessFully));

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
