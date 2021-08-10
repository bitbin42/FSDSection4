using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MMStoreServer {
  public class Startup {

    public Startup(IConfiguration configuration) {
    Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
          {options.TokenValidationParameters = new TokenValidationParameters {ValidateIssuer = true, ValidateAudience = true,
              ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = Configuration["Jwt:Issuer"],
              ValidAudience = Configuration["Jwt:Issuer"], 
              IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))};}); 
    services.AddControllers().AddJsonOptions(options => {options.JsonSerializerOptions.IgnoreNullValues = true;
                                                        options.JsonSerializerOptions.MaxDepth=0;});
    services.AddCors(c => {c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());});
    services.Add(new ServiceDescriptor(typeof(MMStoreServer.Repositories.IMMStoreRepository), typeof(MMStoreServer.Repositories.MMStoreRepository), ServiceLifetime.Scoped));
    services.AddDbContext<MMStoreServer.Repositories.MMStoreDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MMStoreDB")));
    //services.AddSwaggerGen(c => {
    //  c.SwaggerDoc("v1", new OpenApiInfo { Title = "MMStoreServer", Version = "v1" });
    //  });
	 services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
	 services.AddTransient<Support.Helper, Support.Helper>();

    //// to allow the client code to be tested; this should be removed for production
    //services.AddCors(options => { options.AddPolicy("LocalHostPolicy",
    //    builder => { builder.WithOrigins("http://localhost:3000", "http://localhost:5000"); });});
   }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
    if (env.IsDevelopment()) {
      app.UseDeveloperExceptionPage();
      //app.UseSwagger();
      //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MMStoreServer v1"));
    }

    app.UseRouting();
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());    
    
    //app.UseCors("LocalHostPolicy"); // allow localhost - must call beteeen UseRouting and UseEndpoints
      
    app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}
