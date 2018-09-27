using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineCalculator.Models
{
    public class AppDataContext : DbContext
    {
        public IConfiguration _ConnectionString;

        public AppDataContext(IConfiguration configuration)
        {
            _ConnectionString = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_ConnectionString.GetSection("Data").GetSection("PostgreSqlConnectionString").Value);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
        }
        public DbSet<AppData> AppDatas { get; set; }
    }
}
