using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fondef.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Fondef.DAL
{
    public class FondefContext : DbContext
    {
        public FondefContext() : base("FondefContext")
        {
        }
        public DbSet<Cuenca> Cuencas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}