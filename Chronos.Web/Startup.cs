using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace Chronos.Web
{
    public class Startup
    {
        private readonly Container _container = new Container();
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the _container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            IntegrateSimpleInjector(services);
        }

        private void IntegrateSimpleInjector(IServiceCollection services) {
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(
                new SimpleInjectorViewComponentActivator(_container));

            services.EnableSimpleInjectorCrossWiring(_container);
            services.UseSimpleInjectorAspNetRequestScoping(_container);

            services.AddSingleton(p => _container.GetService<ICommandBus>());
            services.AddSingleton(p => _container.GetService<IQueryProcessor>());
            services.AddSingleton(p => _container.GetService<ITimeNavigator>());
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
                app.UseExceptionHandler("/Error");
            }

            InitializeContainer(app);
            _container.Verify();
            
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
        
        private void InitializeContainer(IApplicationBuilder app) {
            // Add application presentation components:
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            var root = new CompositionRoot().WriteWith().Persistent().Database("Chronos.ES")
                .ReadWith().Persistent().Database("Chronos.Read");
            root.ComposeApplication(_container);
            _container.Register<IDebugLog,DebugLog>(Lifestyle.Singleton);
            
            // Cross-wire ASP.NET services (if any). For instance:
            //_container.CrossWire<ILoggerFactory>(app);

            // NOTE: Do prevent cross-wired instances as much as possible.
            // See: https://simpleinjector.org/blog/2016/07/
        }
    }
}