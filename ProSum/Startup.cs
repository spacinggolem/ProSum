using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProSum.Containers;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.Helpers;
using ProSum.MssqlContext;
using ProSum.Services;
using ProSum.Services.Interfaces;
using System;
using System.Linq;

namespace ProSum
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
            string connectionString = Configuration.GetConnectionString("VPSConnection");
            RoleContainer roleContainer = new RoleContainer(connectionString);
            StepContainer stepContainer = new StepContainer(connectionString);
            PermissionContainer permissionContainer = new PermissionContainer(connectionString);
            SessionContainer sessionContainer = new SessionContainer(new ProjectService(stepContainer, permissionContainer, connectionString));
            AnnouncementContainer announcementContainer = new AnnouncementContainer(connectionString);
            ProjectFileContainer projectFileContainer = new ProjectFileContainer(connectionString);

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddSingleton<IProjectService>(new ProjectService(stepContainer, permissionContainer, connectionString));
            services.AddSingleton<IUserService>(new UserService(roleContainer, connectionString));
            services.AddSingleton<ILogger>(new LoggingContainer(stepContainer, connectionString));
            services.AddSingleton<IClientService>(new ClientService(connectionString));
            services.AddSingleton<IAdminService>(new AdminService(connectionString));
            services.AddSingleton<IPermissionContainer>(permissionContainer);
            services.AddSingleton<IRoleContainer>(roleContainer);
            services.AddSingleton<ISessionContainer>(sessionContainer);
            services.AddSingleton<IStepContainer>(stepContainer);
            services.AddSingleton<IAnnouncementContainer>(announcementContainer);
            services.AddSingleton<IProjectFileContainer>(projectFileContainer);

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseCookiePolicy();

            /* app.UseAuthentication();
             app.UseAuthorization();*/


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Project}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            CreateAdmin(serviceProvider);
        }

        private void CreateAdmin(IServiceProvider services)
        {
            IUserService userService = services.GetService<IUserService>();

            if (userService.GetByEmail("admin@prosum.com") == null)
            {
                IAdminService adminService = services.GetService<IAdminService>();
                IRoleContainer roleContainer = services.GetService<IRoleContainer>();

                Role role = roleContainer.Roles.FirstOrDefault(Role => Role.Name == "Admin");
                Guid roleId = Guid.Empty;
                if (role == null)
                {
                    roleContainer.CreateRole("Admin");
                }
                role = roleContainer.Roles.FirstOrDefault(Role => Role.Name == "Admin");



                adminService.CreateUser(new User("John", "Doe", "Admin", PasswordHasher.HashPassword("admin123"), "admin@prosum.com", "0612345678", role, DepartmentEnum.None));
            }
        }
    }
}
