using AspSchoolWebApi.Authintication;
using AspSchoolWebApi.Authintication.Interfaces;
using AspSchoolWebApi.Email;
using AspSchoolWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
 using Microsoft.Extensions.Hosting;
using System.IO;

namespace AspSchoolWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: AccountController

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly IEmailSender _emailsender;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext db;
        private readonly IHostEnvironment _env;

        public AccountController(SignInManager<ApplicationUser> signManager, UserManager<ApplicationUser> userManager,ITokenGenerator tokenGenerator, IEmailSender emailsender, RoleManager<IdentityRole> roleManager,ApplicationDbContext applicationDbContext, IHostEnvironment ENV)
        {
            _signManager = signManager;
            _userManager = userManager;
            _emailsender = emailsender;
            _tokenGenerator = tokenGenerator;
            _roleManager = roleManager;
            db = applicationDbContext;
            _env = ENV;
        }
 





        // POST: AccountController/Create
        [HttpPost("[action]")]

       // [ValidateAntiForgeryToken]
       [Route("AdminRegister")]
        [AllowAnonymous]

        public async Task<object> AdminRegisterAsync(AdminRegisterVModel model)
        {
            if (ModelState.IsValid)
            {
                #region save image
                string photofileName = "anonymous.jpg";
                try
                {
                    var httprequest = Request.Form;//من هناك هبعت الصورة فشكل فورم داتا
                    var postedFile = httprequest.Files[0];
                    string newfileName = postedFile.FileName;
                    var physicalpath = _env.ContentRootPath + "/photos/" + newfileName + DateTime.Now.Second+DateTime.Now.Millisecond;
                    using (var stream = new FileStream(physicalpath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }
                    photofileName = newfileName;

                }
                catch (Exception)
                {
                    photofileName = "anonymous.jpg";


                }
                #endregion
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email,Name=model.Name ,Age=model.Age,PhotoFileName= photofileName,EmailConfirmed=true};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                 

                    await _userManager.AddToRoleAsync(user, "Admin");


                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { UserId = user.Id, Code = code }, protocol: HttpContext.Request.Scheme);

                    //await _emailsender.SendEmailAsync(user.Email, "Techhowdy.com - Confirm Your Email", "Please confirm your e-mail by clicking this link: <a href=\"" + callbackUrl + "\">click here</a>");



                    return Ok(new JsonResult( "Registerd Sucessfuly") );
                }
                

                else
                {
                    return BadRequest("error  in register" );

                }
            }
            else
            {
                return BadRequest(  "invaled date" );

            }


        }

        



        [AllowAnonymous]
        [HttpGet("[action]")]//عشان هيضغط ع لينك كانة هيفتحها بس
        [Route("ConfirmEmail")]

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);

            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new JsonResult("ERROR");
            }

            if (user.EmailConfirmed)
            {
                return Ok("actually EmailConfirmed");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {

                return Ok( "EmailConfirmed");

            }
            else
            {
                List<string> errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.ToString());
                }
                return new JsonResult(errors);
            }


        }

       [HttpPost("[action]")]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginVModel model)
        {
            // Get the User from Database
            if (!await _roleManager.RoleExistsAsync("Parents"))
            {
                IdentityRole newRole = new IdentityRole("Parents");
                await _roleManager.CreateAsync(newRole);
            }

            var user = await _userManager.FindByNameAsync(model.Email);
            
         
            if (user != null && (await _signManager.CheckPasswordSignInAsync(user, model.Password,true)).Succeeded)
            {

                // THen Check If Email Is confirmed
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "User Has not Confirmed Email.");

                    return Unauthorized( "We sent you an Confirmation Email. Please Confirm Your Registration With ayaad.com To Log in." );
                }

                var roles = await _userManager.GetRolesAsync(user);
                string personid = "";
              
                switch (roles.FirstOrDefault())
                {
                    case "admin": 
                        {personid = user.Id; break; }
                    case "teachers":
                        { personid = db.Teachers.Where(t => t.user.Id == user.Id).FirstOrDefault().id.ToString(); break; }
                    case "students":
                        { personid= db.Students.Where(s => s.user.Id == user.Id).FirstOrDefault().id.ToString(); break; }
                    case "Parents":
                        { personid =db.Parents.Where(p => p.User.Id == user.Id).FirstOrDefault().Id.ToString(); break; }

                    default:break;
                }
                if (personid==""||personid==null)
                {
                    return Unauthorized("We sent you an Confirmation Email. Please Confirm Your Registration With ayaad.com To Log in.");

                }

                var claims = new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                       // new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString()),
                        new Claim("PersonId",personid),

                     };

            string token=    _tokenGenerator.GenereteToken(claims.ToList());

                return Ok(new {token=token });

            }
            else
            {
                return Unauthorized("EMAIL OR PASWWORD not correct");
            }
        }



        [HttpPost("[action]")]
        [AllowAnonymous]
        [Route("ExternalLogin")]
        public async Task<IActionResult> ExternalLoginAsync( ExternalLoginVModel model) 
        {
            //  var res=  await    _signManager.ExternalLoginSignInAsync(model.provider, model.providertoken, false);

            // _userManager.RemoveAuthenticationTokenAsync()
            ApplicationUser user = db.Users.Where(u => u.Email == model.Email).FirstOrDefault();

         bool res=   db.UserTokens.SingleOrDefault(ut => ut.LoginProvider == model.provider &&ut.UserId==user.Id)==null?false:true ;
            if (res&&user!=null)
            {
       //    string id=     db.UserTokens.Where(ut => ut.Name == model.nameidentifire && ut.LoginProvider == model.provider).FirstOrDefault().UserId;

              //  ApplicationUser user = await db.Users.FindAsync(id);

                var roles = await _userManager.GetRolesAsync(user);
                string personid = "";

                switch (roles.FirstOrDefault())
                {
                    case "admin":
                        { personid = user.Id; break; }
                    case "teachers":
                        { personid = db.Teachers.Where(t => t.user.Id == user.Id).FirstOrDefault().id.ToString(); break; }
                    case "students":
                        { personid = db.Students.Where(s => s.user.Id == user.Id).FirstOrDefault().id.ToString(); break; }
                    case "Parents":
                        { personid = db.Parents.Where(p => p.User.Id == user.Id).FirstOrDefault().Id.ToString(); break; }

                    default: break;
                }
                if (personid == "" || personid == null)
                {
                    return Unauthorized("We sent you an Confirmation Email. Please Confirm Your Registration With ayaad.com To Log in.");

                }

                var claims = new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                       // new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString()),
                        new Claim("PersonId",personid),

                     };

                string token = _tokenGenerator.GenereteToken(claims.ToList());

                return Ok(new { token = token });

            }
            else
            {
                return Unauthorized("EMAIL OR PASWWORD not correct");
            }
        }

        



    }
}
