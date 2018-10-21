﻿using CustomerApi.ErrorHandling;
using CustomerApi.Filters;
using CustomerApi.Models;
using CustomerApi.Repositories;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace CustomerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt => opt.Filters.Add<HandleExceptionsFilterAttribute>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Customer API", Version = "v1" });
            });

            services.Configure<ApiBehaviorOptions>(x => x.InvalidModelStateResponseFactory = ErrorHandler.FromInvalidModel);
            RegisterDependencies(services);
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton(new LiteDatabase(Configuration.GetConnectionString("db")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            ConfigureSwagger(app);

            app.UseHttpsRedirection();
            app.UseMvc();

            ConfigureDatabaseSchema();
        }

        private void ConfigureDatabaseSchema()
        {
            BsonMapper.Global.Entity<Customer>().Id(x => x.Id);
        }

        private static void ConfigureSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API V1"); });
            app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
        }
    }
}
