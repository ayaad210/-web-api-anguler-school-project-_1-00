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
    public class GroupsController : ControllerBase
    {


        private readonly ApplicationDbContext db;

        public GroupsController(ApplicationDbContext _Db)
        {
            db = _Db;
        }
        [HttpGet]
        [Route("GetGroups")]
        [Authorize(Roles = "admin,teachers")]

        public async Task<JsonResult> GetGroupsAsync()
        {

            var t = await Task.Factory.StartNew(() =>
            {
                return db.Groups.ToList();
            });

            return new JsonResult(t);
        }

        // GET: Groups/Details/5
        [HttpGet()]
        [Route("GetGroupByid")]
        [Authorize(Roles = "admin,teachers")]

        public async Task<JsonResult> GetGroupByidAsync(int? GroupId)
        {
            if (GroupId == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
            var t = await Task.Factory.StartNew(() =>
            {


                Group Group = db.Groups.Find(GroupId);

                return Group;
            });


            return new JsonResult(t);

        }

        [HttpGet()]
        [Route("GetGroupBySemesterid")]
        [Authorize(Roles = "admin,teachers")]

        public async Task<JsonResult> GetGroupBySemesteridAsync(int? semesterid)
        {
            if (semesterid == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
            var t = await Task.Factory.StartNew(() =>
            {


                List<Group> Groups = db.Groups.Where(g => g.SemesterId == semesterid).ToList();

                return Groups;
            });


            return new JsonResult(t);

        }


        [HttpPost]
        [Route("CreateGroup")]
        [Authorize(Roles = "admin")]

        public async Task<ActionResult> CreateGroupAsync(Group Group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
            await db.Groups.AddAsync(Group);

            if ((await db.SaveChangesAsync()) > 0)
            {
                return Ok(new JsonResult(Helber.ResponseStringsHelber.AddedSucessFully));
            }


            return BadRequest(Helber.ResponseStringsHelber.SavingError);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        [Route("UpdateGroup")]
        public async Task<ActionResult> UpdateGroupAsync(Group Group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
            var grp = await db.Groups.FindAsync(Group.id);

            if (grp == null)
            {
                return BadRequest(Helber.ResponseStringsHelber.NotExist);

            }
            grp.Name = Group.Name;
            
            
            db.Entry(grp).State = EntityState.Modified;

            if ((await db.SaveChangesAsync()) > 0)
            {
                return Ok(new JsonResult ( Helber.ResponseStringsHelber.UpdatedSucessFully));
            }


            return BadRequest(Helber.ResponseStringsHelber.SavingError);
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("DeleteGroup")]
        public async Task<ActionResult> DeleteGroupAsync(int Groupid)
        {
            Group Group = db.Groups.Find(Groupid);
            if (Group.STasks.Count <= 0)
            {
                db.Groups.Remove(Group);
                if ((await db.SaveChangesAsync()) > 0)
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
