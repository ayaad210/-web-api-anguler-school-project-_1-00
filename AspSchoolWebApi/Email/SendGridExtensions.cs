using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspSchoolWebApi.Email
{
    public static class SendGridExtensions
    {
        public static IServiceCollection AddSendGridEmailSender(this IServiceCollection services) 
        {
            services.AddTransient<IEmailSender, EmailSender>();

            return services;
         }
    }
}
