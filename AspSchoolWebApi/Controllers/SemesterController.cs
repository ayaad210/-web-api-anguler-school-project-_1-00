using AspSchoolWebApi.Authintication;
using AspSchoolWebApi.Helbers;
using AspSchoolWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspSchoolWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public SemesterController(ApplicationDbContext _Db)
        { 
            db = _Db;
        }
        // GET: api/<SemesterController>
        [HttpGet("[action]")]
        [Route("GetSemetsrs")]
        [Authorize]
        public async Task<JsonResult> GetSemetsrsAsync()
        {

            var res = await Task.Factory.StartNew(() =>
             {
                 return db.Semesters.ToList();
             }
                );
            return new JsonResult(res);

            
        }

        // GET api/<SemesterController>/5
        [HttpGet("{Semesterid}")]
        [Route("GetSemesterBYId")]
        [Authorize]
        public async Task<JsonResult> GetSemesterBYIdAsync([FromRoute]int Semesterid)
        {

            var res = await Task.Factory.StartNew(() =>
            {
                return db.Semesters.Where(s => s.Id == Semesterid);
            }
                );
            return new JsonResult(res);
            
        }

        // POST api/<SemesterController>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("AddSemester")]
        public async Task<ActionResult> AddSemesterAsync([FromBody] Semester semester )
        {
            
                await db.Semesters.AddAsync(semester);
       var t=        await db.SaveChangesAsync();

            if (t>0)
            {
                return Ok(new JsonResult( Helber.ResponseStringsHelber.AddedSucessFully));
            }
            return BadRequest(Helber.ResponseStringsHelber.SavingError);


        }

        // PUT api/<SemesterController>/5
        [HttpPut]
        [Route("UpdateSemester")]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult> UpdateSemesterAsync( [FromBody] Semester semester)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
        Semester s=   await db.Semesters.FindAsync(semester.Id);
            s.EndDate = semester.EndDate;
            s.StartDate = semester.StartDate;
            s.SemesterName = semester.SemesterName;
            
            db.Entry(s).State = EntityState.Modified;
        var t=  await  db.SaveChangesAsync();
            if (t>0)
            {
                return Ok(new JsonResult(Helber.ResponseStringsHelber.UpdatedSucessFully));
            }
            return BadRequest(Helber.ResponseStringsHelber.SavingError);



        }


        // DELETE api/<SemesterController>/5
        [HttpDelete("[action]")]
        [Authorize(Roles = "admin")]
        [Route("DeleteSemester")]
        public async Task<ActionResult> DeleteSemesterAsync( int semesterid)
        {
            var se = await db.Semesters.FindAsync(semesterid);///تعتديل            
            db.Semesters.Remove(se);

            if ((await db.SaveChangesAsync())>0)
            {
                return Ok(new JsonResult(Helber.ResponseStringsHelber.DeletedSucessFully));

            }
            return BadRequest(Helber.ResponseStringsHelber.SavingError);

        }


        //[HttpPost("{semesterId}")]
        //[Route("AddSubjectsToSemester")]
        //[Authorize("Admin")]
        //public async Task<ActionResult> AddSubjectsTosemesterAsync([FromRoute] int semesterId, [FromBody]List<Subject> subjectslist)
        //{
        //    if (semesterId == 0|| subjectslist==null)
        //    {
        //        return BadRequest(Helber.ResponseStringsHelber.NullParams);
        //    }

        //   Semester s=   await db.Semesters.FindAsync(semesterId);

        //    foreach (var item in subjectslist)
        //    {
        //        s.SemestersSubjects.Add(item);


        //    }


        //    var t = await db.SaveChangesAsync();
        //    if (t > 0)
        //    {
        //        return Ok(Helber.ResponseStringsHelber.AddedSucessFully);
        //    }
        //    return BadRequest(Helber.ResponseStringsHelber.SavingError);



        //}

        //[HttpDelete("{semesterId}")]
        //[Authorize("Admin")]
        //[Route("DeleteSubjectsTosemester")]
        //public async Task<ActionResult> DeleteSubjectsTosemesterAsync([FromRoute] int semesterId,[FromBody] List<Subject> subjectslist)
        //{
        //    if (semesterId == 0 || subjectslist == null)
        //    {
        //        return BadRequest(Helber.ResponseStringsHelber.NullParams);
        //    }

        //    Semester s = await db.Semesters.FindAsync(semesterId);

        //    foreach (var item in subjectslist)
        //    {
        //        s.SemestersSubjects.Remove(item);


        //    }


        //    var t = await db.SaveChangesAsync();
        //    if (t > 0)
        //    {
        //        return Ok(Helber.ResponseStringsHelber.DeletedSucessFully);
        //    }
        //    return BadRequest(Helber.ResponseStringsHelber.SavingError);



   // }

    public class sujectsemester
    {
        public int semesterid { get; set; }
        public int subjectid { get; set; }
    }

    [HttpPost("[action]")]
    [Route("AddSubjectToSemester")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> AddSubjectTosemesterAsync(sujectsemester param)
    {
        if (param.semesterid == 0 || param.subjectid == 0)
        {
            return BadRequest(Helber.ResponseStringsHelber.NullParams);
        }

        Semester s = await db.Semesters.FindAsync(param.semesterid);
        Subject sub = await db.Subjects.FindAsync(param.subjectid);
        if (sub != null)
        {
            s.SemestersSubjects.Add(sub);

        }





        var t = await db.SaveChangesAsync();
        if (t > 0)
        {
            return Ok(Helber.ResponseStringsHelber.AddedSucessFully);
        }
        return BadRequest(Helber.ResponseStringsHelber.SavingError);



    }

    [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("DeleteSubjectTosemester")]
        public async Task<ActionResult> DeleteSubjectTosemesterAsync( int semesterId, int Subjectid)
        {
            if (semesterId == 0 ||Subjectid==0)
            {
                return BadRequest(Helber.ResponseStringsHelber.NullParams);
            }

            Semester s = await db.Semesters.FindAsync(semesterId);
            Subject sub = await db.Subjects.FindAsync(Subjectid);
            db.Entry(s).Collection("SemestersSubjects").Load();
                if (sub != null)
            {
                s.SemestersSubjects.Remove(sub);

            






            var t = await db.SaveChangesAsync();
            if (t > 0)
            {
                return Ok(Helber.ResponseStringsHelber.DeletedSucessFully);
            }
            }
            return BadRequest(Helber.ResponseStringsHelber.SavingError);



        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("GetSemesterSubjects")]
        public async Task<JsonResult> GetSemesterSubjectsAsync(int semesterid)
        {

            var res = await Task.Factory.StartNew(() =>
            {
         //       try
             //   {
            List<Subject> list=          db.Semesters.Where(s => s.Id == semesterid).Include(s => s.SemestersSubjects).FirstOrDefault().SemestersSubjects.ToList();
             
                return list.Select( su=>new { su.id, su.Name } );
               // }
                //catch (Exception)
                //{

                //    return new JsonResult(Helber.ResponseStringsHelber.NotExist);
               }
          
                );

            return new JsonResult(  res);


        }
    }
}
