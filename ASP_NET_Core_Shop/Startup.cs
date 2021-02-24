using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP_NET_Core_Shop.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using ASP_NET_Core_Shop.Models.Repositories;

namespace ASP_NET_Core_Shop
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
			services.AddControllersWithViews();

			//�|������
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
					.AddCookie(options => {
						//�p�G�w�n�J���v������ �|�ɦV"/Shop/AccessDeny"������
						options.AccessDeniedPath = "/Shop/AccessDeny";
						//�p�G���n�J �|�ɦV�n�J����
						options.LoginPath = "/Shop/Login";
						options.Cookie.HttpOnly = true;
					});

			//�s����Ʈw
			services.AddDbContext<ShopDBContext>(options =>
			options.UseSqlServer(Configuration.GetConnectionString("DBConnectionString")));

			services.AddScoped<IRepository, Repository>();
			services.AddMvc();
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
			}
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();   //�n�J����

			app.UseAuthorization();  //���v

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
