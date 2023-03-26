using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SalesReportSystem.Pages.Report.Model;
using SalesReportSystem.ViewModel;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SalesReportSystem.Model
{
    public class AuthDbContext: DbContext
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        public DbSet<Users> users { get; set; }
        public DbSet<UserRoles> userRoles { get; set; }
        public DbSet<SalesModel> sales { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IConfiguration configuration) : base (options)
        {
            _connectionString = configuration.GetConnectionString("AuthConnectionString");
            _configuration = configuration;
        }

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<UserRoles>().ToTable("UserRoles");
            modelBuilder.Entity<SalesModel>().ToTable("Sales");

        }

        public async Task<IList<object[]>> ExecuteStoredProcedure(string storedProcedureName, params SqlParameter[] parameters)
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>().UseSqlServer(_configuration.GetConnectionString("AuthConnectionString")).Options;
            using (var context = new AuthDbContext(options, _configuration))
            {
                var command = context.Database.GetDbConnection().CreateCommand();
                command.CommandText = storedProcedureName;
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                context.Database.OpenConnection();
                var result = new List<object[]>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        result.Add(values);
                    }
                }

                return result;
            }
        }
    }
}
