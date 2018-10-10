using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineCalculator.Models
{
    public class ServerCapacityContext : DbContext
    {
        public ServerCapacityContext(DbContextOptions<ServerCapacityContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<AppObj>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<AppParameters>()
                .HasOne<AppObj>(s => s.AppObj)
                .WithMany(g => g.AppParameters)
                .HasForeignKey(s => s.AppId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProjectApp>()
                .HasOne<AppObj>(s => s.AppObj)
                .WithMany(g => g.ProjectApps)
                .HasForeignKey(s => s.appId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProjectApp>()
                .HasOne<Project>(s => s.Project)
                .WithMany(g => g.projectApps)
                .HasForeignKey(s => s.projectId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Project>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            /*
            builder.Entity<CalcAppData>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<AppParameters>()
                .HasOne<CalcAppData>(s => s.AppObj)
                .WithMany(g => g.resourses)
                .HasForeignKey(s => s.AppId);
            */
        }
        public DbSet<AppObj> AppObjDbSet { get; set; }
        public DbSet<AppParameters> AppParameterDbSet { get; set; }
        public DbSet<Project> ProjectDbSet { get; set; }
        public DbSet<ProjectApp> ProjectAppDbSet { get; set; }
    }
}
