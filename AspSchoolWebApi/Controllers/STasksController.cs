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
using AspSchoolWebApi.Models;

namespace AspSchoolWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class STasksController : ControllerBase
    {

        private readonly ApplicationDbContext db;

        public STasksController(ApplicationDbContext _Db)
        {
            db = _Db;
        }
        [HttpGet]
        [Route("GetStasks")]
        [Authorize(Roles = "admin")]

        public async Task<JsonResult> GetStasksAsync()
        {

            var t = await Task.Factory.StartNew(() =>
            {
                return db.Tasks.ToList();
            });

            return new JsonResult(t);
        }

        // GET: Stasks/Details/5
        [HttpGet]
        [Route("GetStaskByid")]
        [Authorize]//(Roles = "students,teachers")]

        public async Task<JsonResult> GetStaskByidAsync(int? StaskId)
        {
            if (StaskId == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
            var t = await Task.Factory.StartNew(() =>
            {


                STask Stask = db.Tasks.Find(StaskId);

                return Stask;
            });


            return new JsonResult(t);

        }

        [HttpGet()]
        [Route("GetStaskByGroupid")]
        [Authorize(Roles = "teachers,students")]
        public async Task<JsonResult> GetStaskByGroupidAsync(int? GroupId,int personid,string role)
        {
            if (GroupId == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
            

            var t = await Task.Factory.StartNew(() =>
            {
                List<STask> stasks = null;
                switch (role)
                {


                    case "students":
                        {
                            //  int   Sgroupid  =(await db.Students.FindAsync(personid)).Groups.LastOrDefault().id;
                            stasks = db.Tasks.Where(t => t.Groupid == GroupId).ToList(); break;

                        }
                    case "teachers":
                        {
                            stasks = db.Tasks.Where(t => t.Teacherid == personid && t.Groupid == GroupId).ToList(); break;


                        }
                    default:
                        break;
                }
                return stasks;
            });

            if (t==null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NotExist);
            }

            
           


            return new JsonResult(t);

        }


        [HttpGet()]
        [Route("GetStaskByGroupAndSubjectId")]
        [Authorize(Roles = "students,teachers")]
        public async Task<JsonResult> GetStaskByGroupAndSubjectIdAsync([FromQuery] int? groupid, [FromQuery] int? subjectid, [FromQuery] string? personid, [FromQuery] string? role)
        {
            if (groupid == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
         

            var t = await Task.Factory.StartNew(() =>
            {

                List<STask> stasks = null;
                if (role == "students")
                {
                 Student s=   db.Students.Include(s=>s.Groups).FirstOrDefault(s=>s.id==int.Parse(personid));

                    //stasks = db.Tasks.Where(t => t.Groupid == groupid && t.Teacher.Subject.id == subjectid).ToList();
                    stasks = db.Tasks.Where(t => t.Groupid == s.Groups.LastOrDefault().id && t.Teacher.Subject.id == subjectid).ToList();

                }
                else
                    if ( role=="teachers")
                {
                    stasks = db.Tasks.Where(t => t.Groupid == groupid && t.Teacherid.ToString()==personid).ToList();
                }

                //  int   Sgroupid  =(await db.Students.FindAsync(personid)).Groups.LastOrDefault().id;


                return stasks;
            });

            if (t == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NotExist);
            }

            return new JsonResult(t);

        }



        [HttpPost]
        [Route("CreateStask")]
        [Authorize(Roles = "teachers")]

        public async Task<ActionResult> CreateStaskAsync(STask Stask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
            await db.Tasks.AddAsync(Stask);

            if ((await db.SaveChangesAsync()) > 0)
            {
                return Ok(Helber.ResponseStringsHelber.AddedSucessFully);
            }


            return BadRequest(Helber.ResponseStringsHelber.SavingError);
        }

        [HttpPut]
        [Authorize(Roles = "teachers")]
        [Route("UpdateStask")]
        public async Task<ActionResult> UpdateStaskAsync( STask stask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
            var tsk = await db.Tasks.FindAsync(stask.id);
            if (tsk == null)
            {
                return BadRequest(Helber.ResponseStringsHelber.NotExist);

            }
            tsk.Name = stask.Name;
            tsk.Notes= stask.Notes;
            tsk.Total = tsk.Total;
            tsk.Content = stask.Content;
            db.Entry(tsk).State = EntityState.Modified;

            if ((await db.SaveChangesAsync()) > 0)
            {
                return Ok(new JsonResult( Helber.ResponseStringsHelber.UpdatedSucessFully));
            }


            return BadRequest(Helber.ResponseStringsHelber.SavingError);
        }
        [HttpDelete]
        [Authorize(Roles = "teachers")]
        [Route("DeleteStask")]
        public async Task<ActionResult> DeleteStaskAsync(int Staskid)
        {
            STask Stask = db.Tasks.Find(Staskid);
            if (Stask.Answers.Count <= 0)
            {
                db.Tasks.Remove(Stask);
                if ((await db.SaveChangesAsync()) > 0)
                {
                    return Ok(Helber.ResponseStringsHelber.DeletedSucessFully);

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
