using AspSchoolWebApi.Authintication;
using AspSchoolWebApi.Authintication.Interfaces;
using AspSchoolWebApi.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspSchoolWebApi
{
    public class Startup
    {
     //   private readonly RoleManager<IdentityRole> _roleManager;
        public Startup(IConfiguration configuration )
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();
            services.AddAuthorization();
            services.AddCors(c =>
        c.AddPolicy("crosp", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())

        

         ); ;

           services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(Configuration.GetConnectionString("sqlcon")));
         
            services.AddIdentity<ApplicationUser, IdentityRole>().
                AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
            services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

            services.AddAuthentication(optons =>
            {
                optons.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                optons.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(options =>
            {//ÂÊ «·œÌﬂÊœ— ”Ê«¡ ÂÌœ— «Ê ﬂÊﬂÏ „‘ „Õ «Ã «” ÕŒœ„ «·œÌﬂÊœ—
                SymmetricSecurityKey _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:mysecret"]));
                string va = Configuration["JWT:ValidAudience"]; 
                string vi = Configuration["JWT:ValidIssuer"]; 

                options.SaveToken = true;
                options.RequireHttpsMetadata = false;


                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = va,
                    ValidIssuer = vi,
                    IssuerSigningKey = _key,
                    ClockSkew = TimeSpan.Zero,//„ –Êœ‘ Êﬁ  ›Êﬁ ⁄„—…
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RoleClaimType=ClaimTypes.Role
                    
                    
                    



                };
                //options.Events = new JwtBearerEvents
                //{
                //    //OnMessageReceived = context =>
                //    //{

                //    ////  var token = context.Request.Cookies["token"]; //·Ê ⁄«Ì“ «” ﬁ»· «· ÊﬂÌ‰ „‰ «ﬂÊﬂÏ „‘ ÂÌœ—
                //    // //  context.Token = token;
                //    //    return Task.CompletedTask;




                //    //}
                //};



            });

            services.AddTransient<ITokenGenerator,TokenGenerator>();
            services.AddHttpContextAccessor();
            services.AddSendGridEmailSender();
            services.AddRouting();
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseAuthorization();
         
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}" );

    //            endpoints.MapControllerRoute(
    //name: "imagesave",
    //pattern: "{Teachers}/{saveimage}/{old?}");

            }



            
            );
            string x = Path.Combine(Directory.GetCurrentDirectory(),"photos");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "photos")),
                RequestPath = "/photos",

            });

        }
    }
}
