﻿using GraphQl.AspNetCore;
using GraphQlDemo.Data;
using GraphQlDemo.Query.Data;
using GraphQlDemo.Query.GraphQlTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GraphQlDemo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddSingleton<IBookRepository, BookRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IBookRepository bookRepository)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseGraphQl(new GraphQlMiddlewareOptions
            {
                GraphApiUrl = "/graph", // default
                RootGraphType = new BooksQuery(bookRepository),
                FormatOutput = true // default: false
            });
            app.UseGraphQl(options =>
            {
                options.GraphApiUrl = "/graph-api";
                options.RootGraphType = new BooksQuery(bookRepository);
                options.FormatOutput = false; // default
            });

            app.UseMvc();
        }
    }
}
