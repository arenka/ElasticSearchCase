using AutoMapper;
using ElasticSearchCase.Business.Abstract;
using ElasticSearchCase.Business.Concrete;
using ElasticSearchCase.Business.ElasticSearchOptions.Abstract;
using ElasticSearchCase.Business.ElasticSearchOptions.Concrete;
using ElasticSearchCase.Business.Mappings;
using ElasticSearchCase.DataAccess.Abstract;
using ElasticSearchCase.DataAccess.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchCase.WebUI
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
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductDal, EfProductDal>();
            services.AddDbContext<ProjectContext>(options =>
                                                  options.UseMySql(Configuration.GetConnectionString("testDb"), ServerVersion.AutoDetect(Configuration.GetConnectionString("testDb"))));
            services.AddScoped<IElasticSearchService, ElasticSearchManager>();
            services.AddScoped<IElasticSearchConfiguration, ElasticSearchConfigration>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            //IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mappingConfig.CreateMapper());
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
