using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using OrdersService.Clients;
using OrdersService.Handlers;
using OrdersService.Messages;
using OrdersService.Repositories;
using Rebus.Persistence.FileSystem;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.FileSystem;

namespace OrdersService
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrdersService", Version = "v1" });
            });

            services.AddSingleton<OrdersRepository>();
            services.AddSingleton(new LiteDatabase(Configuration.GetConnectionString("db")));
            services.AddHttpClient<AccountServiceClient>(cfg => cfg.BaseAddress = new Uri(Configuration["Clients:AccountService"]));

            services.AddRebus(x => x
                .Transport(t => t.UseFileSystem(Configuration["Rebus:QueueDirectory"], "orders-queue"))
                .Subscriptions(t => t.UseJsonFile(Configuration["Rebus:SubscriptionFile"]))
                .Routing(r => r.TypeBased().MapAssemblyDerivedFrom<OrderCreatedEvent>("orders-queue")));
            services.AddRebusHandler<OrderStatusHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrdersService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.ApplicationServices.UseRebus();
        }
    }
}
