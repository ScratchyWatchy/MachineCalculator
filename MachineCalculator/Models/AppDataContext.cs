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
        //public IConfiguration _ConnectionString;

        /*public AppDataContext(IConfiguration configuration)
        {
            _ConnectionString = configuration;
        }*/

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Server = 10.40.10.143; Database = testakhruslov; Username = postgres; Password = QweAsd123; Enlist = true");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppData>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<Parameter>()
                .HasOne<AppData>(s => s.AppData)
                .WithMany(g => g.resourses)
                .HasForeignKey(s => s.AppId);
        }
        public DbSet<AppData> AppDatas { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
    }
}
