
using System;
using Common.Repositories;
using Common.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace API
{
    /*https://app.getpostman.com/join-team?invite_code=83cc885762ff0941b9517a127419aa52a658153eea2aae410e61d3df45c22ac0&target_code=c18be4d2606301a8fad63a1820dc7c69*/
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            //Configure JWT Authentication
            var secretKey = builder.Configuration["JwtSettings:SecretKey"] 
                ?? throw new InvalidOperationException("SECRET KEY IS MISSING");
            var key = Encoding.UTF8.GetBytes(secretKey);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; //Set to true if using HTTPS in production
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false, //Change this if you want to validate issuer
                        ValidateAudience = false //Change this if you want to validate audience
                    };
                });

            builder.Services.AddAuthorization();


            var app = builder.Build();



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

        
            app.UseHttpsRedirection();


            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate(); // Apply pending migrations
            }

            app.Run();
        }
    }
}
