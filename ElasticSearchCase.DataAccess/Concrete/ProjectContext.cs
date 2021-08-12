using ElasticSearchCase.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchCase.DataAccess.Concrete
{
    public class ProjectContext : DbContext
    {
        public ProjectContext()
        {

        }
        public ProjectContext(DbContextOptions<ProjectContext> options):base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string mySqlConnectionStr = "server = localhost; port = 3306; database = test; user = root; Persist Security Info = False; Connect Timeout = 300";
            optionsBuilder.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr));
        }

        public DbSet<Product> Product { get; set; }
    }
}
