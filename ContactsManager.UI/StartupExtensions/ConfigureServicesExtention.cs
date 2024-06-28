using ContactsManager.Core.Domain.IdentityEntities;
using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;	

namespace CRUDExample.StartupExtensions
{
	public static class ConfigureServicesExtention
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddControllersWithViews(options =>
			{
				//Globle filters ,applicable to all the action methods of all controllers
				//In this way actually cannot supply parameter value to Action filter
				//options.Filters.Add<ResponseHeaderActionFilter>();

				//instead we can create a object of our filter class and supply that as argument to Add method directly

				//options.Filters.Add(new ResponseHeaderActionFilter("My-_key-From-Global", "My-_value-From-Global" ,2) ); 
				options.Filters.Add(new ResponseHeaderFilterFactoryAttribute("My-_key-From-Global", "My-_value-From-Global", 2));

				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
			});

			//add services into ioc container
			services.AddScoped<ICountriesGetterService, CountriesGetterService>();
			services.AddScoped<ICountriesAdderService, CountriesAdderService>();
			services.AddScoped<ICountriesUploaderService, CountriesUploaderService>();
			services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>();
			//services.AddScoped<IPersonsGetterService, PersonsGetterServiceChild>();  Bad practice Violates LIskov Substitution principle
			services.AddScoped<PersonsGetterService>();
			services.AddScoped<IPersonsAdderService, PersonsAdderService>();
			services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
			services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
			services.AddScoped<IPersonsSorterService, PersonsSorterService>();
			services.AddScoped<IPersonsRepository, PersonsRepository>();
			services.AddScoped<ICountriesRepository, CountriesRepository>();

			//filter as a service
			services.AddTransient<ResponseHeaderActionFilter>();
			services.AddTransient<PersonsListActionFilter>();

			services.AddDbContext<ApplicationDbContext>(options =>
			{
				//options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});


			//Enable identity in this project
			services.AddIdentity<ApplicationUser, ApplicationRole>(/*(options) =>
			{
				// password complexcity
				options.Password.RequiredLength = 5;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredUniqueChars = 3;  // eg;AB12AB

			}*/)
				.AddEntityFrameworkStores<ApplicationDbContext>() //Application level
				.AddDefaultTokenProviders()
				.AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>() //at repository level
				.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>(); // at repository level

			services.AddAuthorization(options => {
				options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();//enforces authorization policy(user must be authenticated) for all  the action methods
				options.AddPolicy("NotAuthorized",policy =>
				{
					policy.RequireAssertion(context => { return !context.User.Identity!.IsAuthenticated; });
				});
			});
			services.ConfigureApplicationCookie(options => { options.LoginPath = "/Account/Login"; });


				//services.AddHttpLogging(options => options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders);

				//logging
				//By default the logging provider are Degub , Console , EventViewer or EventLog(Windows) but we can configure the same using below statement
				//builder.Host.ConfigureLogging(loggingProvider => { loggingProvider.ClearProviders(); loggingProvider.AddConsole(); loggingProvider.AddDebug()  ; loggingProvider.AddEventLog(); });

				return services;
		}
	}
}
