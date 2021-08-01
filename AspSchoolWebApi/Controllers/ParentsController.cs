using AspSchoolWebApi.Authintication;
using AspSchoolWebApi.Helbers;
using AspSchoolWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ParentsController : ControllerBase
    {


        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly IHostEnvironment _env;

        public ParentsController(SignInManager<ApplicationUser> signManager, UserManager<ApplicationUser> userManager, ApplicationDbContext _Db, IHostEnvironment env)
        {
            _userManager = userManager;
            _signManager = signManager;
               db = _Db;
            _env = env;
        }
        [HttpGet]
        [Route("GetParents")]
        [Authorize(Roles = "admin,teacher")]

        public async Task<JsonResult> GetParentsAsync()
        {

            var t = await Task.Factory.StartNew(() =>
            {
                var Parents = db.Parents.Include(t => t.User).ToList();

                return Parents;
            });

            return new JsonResult(t);
        }

        // GET: Parents/Details/5
        [HttpGet()]
        [Route("GetParentByid/{ParentId?}")]
        [Authorize(Roles = "admin,Parents,teachers")]

        public async Task<JsonResult> GetParentByidAsync(int? ParentId)
        {
            if (ParentId == null)
            {
                return new JsonResult(Helber.ResponseStringsHelber.NullParams);
            }
            var t = await Task.Factory.StartNew(() =>
            {


                Parent parent = db.Parents.Where(t => t. Id== ParentId).Include(t => t.User).FirstOrDefault();

                return parent;
            });


            return new JsonResult(t);

        }

        [HttpPost]
       // [Authorize(Roles = "admin")]
        [Route("CreateParent")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateParentAsync(ParentVModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
            if (model.provider != null&&model.provider!=string.Empty)
            {
                var user = new ApplicationUser { Name = model.Name, Age = model.Age, UserName = model.Email, Email = model.Email, PhotoFileName = model.PhotoFileName , EmailConfirmed = true };

               var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
           //  var x=    await    _userManager.AddLoginAsync(user, new UserLoginInfo(model.provider, model.providertoken, model.provider)) ;
                   await db.UserTokens.AddAsync(new IdentityUserToken<string> { LoginProvider = model.provider, UserId = user.Id, Name = model.identifier, Value = model.providertoken });
              
                    if (await db.SaveChangesAsync()>0)
                    {
                            await _userManager.AddToRoleAsync(user, "Parents");

                            Parent Parent = new Models.Parent();
                            Parent.User = user;
                            db.Parents.Add(Parent);

                            if ((await db.SaveChangesAsync()) > 0)
                            {
                                return Ok(new JsonResult(Helber.ResponseStringsHelber.AddedSucessFully));
                            }


                            return BadRequest(Helber.ResponseStringsHelber.SavingError);
                        



                      ///  return Ok(new JsonResult(Helber.ResponseStringsHelber.AddedSucessFully));

                    }


                }
                
                
                    return BadRequest(Helber.ResponseStringsHelber.SavingError);

                

            }
            else
            {
                #region register by email passowrd
                var user = new ApplicationUser { Name = model.Name, Age = model.Age, UserName = model.Email, Email = model.Email, PhotoFileName = model.PhotoFileName, EmailConfirmed = true  };

                var check = await _userManager.CreateAsync(user, model.Password);

                if (check.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Parents");

                    Parent Parent = new Models.Parent();
                    Parent.User = user;
                    db.Parents.Add(Parent);

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
                #endregion
            }
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


        [ValidateAntiForgeryToken]
        [HttpPut]
        [Authorize(Roles = "admin,parents")]
        [Route("UpdateParent")]
        public async Task<ActionResult> UpdateParentAsync(ParentVModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Helber.ResponseStringsHelber.ValidationError);
            }
            var per =  db.Parents.Include(p => p.User).FirstOrDefault(p => p.Id == model.id);

            if (per == null)
            {
                return BadRequest(Helber.ResponseStringsHelber.NotExist);

            }
            //new object[] { (object)(new { id = tech.user.Id }) }
            ApplicationUser user =  per.User;  //await db.Users.FindAsync(per.User.Id);
            if (user == null)
            {
                return BadRequest(Helber.ResponseStringsHelber.NotExist);

            }
            user.Name = model.Name;
            user.Age = model.Age;
            user.UserName = model.Email;
            user.Email = model.Email;
            user.PhotoFileName = model.PhotoFileName;
            var res = await _userManager.UpdateAsync(user);
            if (res.Succeeded)
            {


                db.Entry(per).State = EntityState.Modified;

                if ((await db.SaveChangesAsync()) > 0)
                {
                    return Ok( new JsonResult( Helber.ResponseStringsHelber.UpdatedSucessFully));
                }
            }



            return BadRequest(Helber.ResponseStringsHelber.SavingError);
        }
        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("DeleteParent")]
        public async Task<ActionResult> DeleteParentAsunc(int Parentid)
        {
            Parent Parent = db.Parents.Include(p => p.User).FirstOrDefault(p => p.Id == Parentid);

            if (Parent.Children.Count <= 0)
            {
              IdentityResult res= await _userManager.DeleteAsync(Parent.User);
                if (res.Succeeded)
                {
                    db.Parents.Remove(Parent);
                    if ((await db.SaveChangesAsync()) > 0)
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

        [HttpGet]
        [Route("GetParentChildren")]
        [Authorize(Roles = "admin,Parents")]

        public async Task<JsonResult> GetParentChildrenAsync(int id)
        {
            var res = await Task.Factory.StartNew(() =>
             {
                 Parent per = db.Parents.Include(p => p.Children).Where(p => p.Id == id).FirstOrDefault();
                 return per.Children.ToList();
             });

            return new JsonResult(res);
        }

        public class perantsstudent
        {
            public int Parentid { get; set; }
            public int StudentId { get; set; }
        }

        [HttpPost]
        [Route("InsertStudentToParent")]
        [Authorize(Roles="admin")]
        public async Task<ActionResult> InsertStudentToParentAsync(perantsstudent model)
          {

            try
            {
                if (model.Parentid==0||model.StudentId==0)
                {
                      return BadRequest(Helber.ResponseStringsHelber.NullParams);

                }
                Parent per = db.Parents.Include(p => p.Children).Where(p => p.Id == model.Parentid).FirstOrDefault();

                
                  Student st = db.Students.Include(p => p.user).Where(p => p.id == model.StudentId).FirstOrDefault();

                  per.Children.Add(st);




                if (await db.SaveChangesAsync()>0)
                {
                    return Ok(new JsonResult( Helber.ResponseStringsHelber.AddedSucessFully));
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
        [Route("DeleteStudentToParent")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteStudentToParentAsync(int Parentid, int StudentId)
        {
            try { 
            if (Parentid == 0 || StudentId == 0)
            {
                return BadRequest(Helber.ResponseStringsHelber.NullParams);

            }
            Parent per = db.Parents.Include(p => p.Children).Where(p => p.Id == Parentid).FirstOrDefault();


            Student st = db.Students.Include(p => p.user).Where(p => p.id == StudentId).FirstOrDefault();
            per.Children.Remove(st);
                if (await db.SaveChangesAsync() > 0)
                {
                    return Ok(  new JsonResult( Helber.ResponseStringsHelber.DeletedSucessFully));
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
