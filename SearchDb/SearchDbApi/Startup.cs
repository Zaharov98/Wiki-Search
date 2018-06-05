using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using NLog.Web;
using SearchDbApi.Data.Context;
using SearchDbApi.Data.Sql;
using SearchDbApi.Indexer;
using SearchDbApi.Search;

namespace SearchDbApi
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
            services.AddMvc();

            if (Configuration["Env"] == "Local") {
                services.AddEntityFrameworkSqlite()
                        .AddDbContext<WordsDbContext>(
                            (options) => options.UseSqlite(Configuration["Local:ConnectionString"]),
                            ServiceLifetime.Scoped
                        );

                services.AddScoped<SqlReader>((service) => new SqlReader(Configuration["Local:ConnectionString"]));
            }
            else if (Configuration["Env"] == "Dev") {
                services.AddEntityFrameworkSqlServer()
                        .AddDbContext<WordsDbContext>(
                    (options) => options.UseSqlServer(Configuration["Dev:ConnectionString"]),
                    ServiceLifetime.Scoped
                );

                services.AddScoped<SqlReader>((service) => new SqlReader(Configuration["Dev:ConnectionString"]));
            }
            
            services.AddScoped<IPageIndexer, SearchDbIndexer>();
            services.AddScoped<ISearch, WikiSearch>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddNLog();
            env.ConfigureNLog(Configuration["Logging:Configuration"]);

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
