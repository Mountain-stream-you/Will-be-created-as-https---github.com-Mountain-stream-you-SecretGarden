using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SecretGarden.Bos;
using SecretGarden.Repositories;

namespace SecretGarden
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;

        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment _webHostEnvironment;
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<BoProvider>();
            services.AddScoped<PeopleRepo>(); 
            if (!_webHostEnvironment.IsProduction())
                services.AddDirectoryBrowser();

            services.AddAutoMapper(typeof(Startup));
            //设置跨域
            services.AddCors(options => 
            {
                options.AddPolicy(
                    MyAllowSpecificOrigins,
                    builder=>
                    {
                        builder.WithOrigins("https://baidu.con").SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .AllowAnyMethod();
                    }
                    );
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //压缩响应
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.AddDbContextPool<SecretGardenContext>(options=> {
                options.UseLazyLoadingProxies(); //延迟加载
                options.UseMySql(Configuration.GetConnectionString("SecretGarden"));
                options.EnableSensitiveDataLogging(); //增加参数输出
            },64);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SecretGarden API", Version = "v1" });
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();

                services.AddDistributedRedisCache(options => {
                    options.Configuration = Configuration["Redis:Configuration"]; //默认端口号为 6379,如果不是指写上 127.0.0.1:端口号
                    options.InstanceName = Configuration["Redis:InstanceName"];
                });
            });
        }

       

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if(env.IsProduction())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //启用wwwroot可访问
            app.UseStaticFiles();

            app.UseCors(MyAllowSpecificOrigins);

            if (!env.IsProduction())
            {
                //判断Log目录是否存在,没有则创建
                var logPath = Path.Combine(AppContext.BaseDirectory, "log");
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                var fileProvider = new PhysicalFileProvider(logPath);
                var requestPath = new PathString("/log");
                var provider = new FileExtensionContentTypeProvider();
                provider.Mappings[".log"] = "application/x-msdownload";
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = fileProvider,
                    RequestPath = requestPath,
                    ContentTypeProvider = provider,
                });
                app.UseDirectoryBrowser(new DirectoryBrowserOptions()
                {
                    FileProvider = fileProvider,
                    RequestPath = requestPath,
                });
                app.UseDirectoryBrowser();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecretGarden API");
                });
            }
            else
            {
                //生产环境使用Https
                app.UseHttpsRedirection();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication() ;//授权

            app.UseRouting();

            app.UseAuthorization();  //认证

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
