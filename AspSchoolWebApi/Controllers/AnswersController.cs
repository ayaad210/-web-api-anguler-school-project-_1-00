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
    public class AnswersController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public AnswersController(ApplicationDbContext _Db)
        {
            db = _Db;
        }

        [HttpGet]
        [Route("getAnswerByStudentAndTask")]
        [Authorize(Roles = "students,teachers")]

        public async Task<JsonResult> getAnswerByStudentAndTaskAsync([FromQuery] int studentid,[FromQuery] int taskid)
        {

            var res = await Task.Factory.StartNew(() =>
            {
                return db.Answers.Where(s => s.StudentId==studentid&&s.STaskid==taskid).Include(s=>s.STask).Include(s=>s.Student.user).FirstOrDefault();
            }
                );
            return new JsonResult(res);

        }


        [HttpGet]
        [Route("GetAnswersByTaskId")]
        [Authorize(Roles = "students,teachers")]
        public async Task<JsonResult> GetAnswersBytaskidAsync(int taskid)
        {

            var res = await Task.Factory.StartNew(() =>
            {
                return db.Answers.Where(s => s.STaskid == taskid).Include(s => s.STask).Include(s => s.Student.user).ToList();

            }
                );
            return new JsonResult(res);


        }

        // GET api/<AnswerController>/5
        [HttpGet]
        [Route("GetAnswerBYId/{Answerid?}")]
        [Authorize(Roles = "students,teachers")]

        public async Task<JsonResult> GetAnswerBYIdAsync([FromRoute] int Answerid)
        {

            var res = await Task.Factory.StartNew(() =>
            {
                return db.Answers.Where(s => s.Id == Answerid);
            }
                );
            return new JsonResult(res);

        }

        // POST api/<AnswerController>
        [HttpPost]
        [Authorize(Roles = "students")]
        [Route("AddAnswer")]

        public async Task<ActionResult> AddAnswerAsync([FromBody] Answer Answer)
        {

            await db.Answers.AddAsync(Answer);
            var t = await db.SaveChangesAsync();

            if (t > 0)
            {
                return Ok(new JsonResult( Helber.ResponseStringsHelber.AddedSucessFully));
            }
            return BadRequest(Helber.ResponseStringsHelber.SavingError);


        }

        // PUT api/<AnswerController>/5
        [HttpPut]
        [Route("UpdateAnswer")]
        [Authorize(Roles = "students,teachers")]

        public async Task<ActionResult> UpdateAnswerAsync([FromBody] Answer Answer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
          Answer _answer= await db.Answers.FindAsync(Answer.Id);
            _answer.AnswerContent = Answer.AnswerContent;
            _answer.Degree = Answer.Degree;
            _answer.Datetime = Answer.Datetime;

            db.Entry(_answer).State = EntityState.Modified;
            var t = await db.SaveChangesAsync();
            if (t > 0)
            {
                return Ok(new JsonResult( Helber.ResponseStringsHelber.UpdatedSucessFully));
            }
            return BadRequest(Helber.ResponseStringsHelber.SavingError);



        }


        // DELETE api/<AnswerController>/5
        [HttpDelete]
        [Authorize(Roles = "teachers")]
        [Route("DeleteAnswer/{id?}")]
        public async Task<ActionResult> DeleteAnswerAsync( [FromRoute] int id)
        {
            var se = await db.Answers.FindAsync(id);

            db.Answers.Remove(se);
            if ((await db.SaveChangesAsync()) > 0)
            {
                return Ok(new JsonResult(Helber.ResponseStringsHelber.DeletedSucessFully));

            }
            return BadRequest(Helber.ResponseStringsHelber.SavingError);

        }


        


    
 

    

}
}
