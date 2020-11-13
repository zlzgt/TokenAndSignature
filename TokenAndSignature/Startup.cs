using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TokenAndSignature.Attributes;

namespace TokenAndSignature
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ��ӻ���MemoryCache
            services.AddMemoryCache();
            //��ӿ�����
            services.AddControllers();

            // ȫ�ֹ�����ע��
            services.AddControllersWithViews(option =>
            {
                option.Filters.Add<ApiSecurityFilterAttribute>();
                option.Filters.Add<ApiExceptionFilterAttribute>();  // ���ȫ���쳣������Ϣ
            });

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });


            // ʹ��Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "TokenAndSignature WebaPI", Version = "1.0" });
                string str = typeof(Startup).Assembly.GetName().Name;
                // include document file
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Startup).Assembly.GetName().Name}.xml"), true);
            });




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // ���Asp.Net Core 3.1 ���޷���ȡHttpContext.Request.Body������
            app.Use(next => new RequestDelegate(
            async context =>
           {
              context.Request.EnableBuffering();
              await next(context);
           }
           ));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "My Demo API (V 1.0)");
            });




           //������Main��֧


            app.UseRouting();

            //�������
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            // ���Է�֧��ʹ��


           //?????????????????????????????????????????sfdssdfsdfsdfsdfsd

        }
    }
}
