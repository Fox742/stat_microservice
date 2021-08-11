using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StatMicroservice
{
    public class Startup
    {
        //private readonly ILogger<Startup> _logger;
        public Startup(IConfiguration configuration/*, ILogger<Startup> logger*/)
        {
            Configuration = configuration;
        //    _logger = logger;
        }



        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            CreateDB();
        }

        private void CreateDB()
        {
            try
            {
                //string connectionString = @"Integrated Security=True;Server=DESKTOP-NCC7QJ7\SQLEXPRESS" + ";Initial Catalog=master";
                //string connectionString = Configuration.GetConnectionString("CommonConnectionString").ToString() + "Database=master";
                //string connectionString = "Server=\".\\\\SQLEXPRESS\";Trusted_Connection=True;" + "Database=master";
                //string connectionString = "Data Source=\".\\\\SQLEXPRESS\";Integrated Security=SSPI;" + "Initial Catalog=master";
                //string connectionString = "Data Source=\"DESKTOP-NCC7QJ7\\\\SQLEXPRESS\";Integrated Security=SSPI;" + "Initial Catalog=master";
                string connectionString = "Server=\"DESKTOP-NCC7QJ7\\SQLEXPRESS\";Integrated Security=SSPI;" + "Initial Catalog=master";
                //string connectionString = "Data Source=\"(localhost)\\\\SQLEXPRESS\";Integrated Security=SSPI;" + "Initial Catalog=master";
                //string connectionString = "Server=(localhost);Trusted_Connection=True;" + "Database=master";

                string command = "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '" + Configuration.GetConnectionString("DataBaseName") + "')"
                + "BEGIN CREATE DATABASE " + Configuration.GetConnectionString("DataBaseName") + ";"
                + "END";
                
                //string command = "SELECT * FROM sys.databases";
                SqlConnection myConn = new SqlConnection(connectionString);
                //SqlCommand myCommand = new SqlCommand(connectionString, myConn);
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(command, myConn);
                myCommand.ExecuteNonQuery();
                myConn.Close();
            }
            catch (Exception e)
            {
               //_logger.LogError("Could not create Data Base: "+e.ToString());
            }
        }
    }
}
