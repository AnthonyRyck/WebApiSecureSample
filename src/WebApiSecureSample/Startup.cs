using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiSecureSample.Areas.Identity;
using WebApiSecureSample.Data;
using WebApiSecureSample.SecureApi;

namespace WebApiSecureSample
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{	
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));

			services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddRazorPages();
			services.AddServerSideBlazor();
			
			services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
			services.AddDatabaseDeveloperPageExceptionFilter();
			services.AddSingleton<WeatherForecastService>();

			// Pour JWT auth
			var key = Configuration["KeyPourJwt"];

			services.AddAuthentication(x =>
			{
				// Si ajout de cette ligne, plus possible de s'authentifier sur le site.
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(x =>
				{
					x.RequireHttpsMetadata = false;
					x.SaveToken = true;
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
						ValidateIssuer = false,
						ValidateAudience = false
					};
				});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("RequestAdmin", policy =>
					   policy.RequireRole("Admin"));

				options.AddPolicy("ManagerRequest", policy =>
					   policy.RequireRole("Manager"));
			});

			services.AddScoped<IJwtAuthenticationManager, JwtAuthenticationManager>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}
