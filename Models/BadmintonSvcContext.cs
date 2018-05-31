using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace BadmintonSvc.Models
{
    public class BadmintonSvcContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BadmintonSvcContext() : base("name=BadmintonSvcContext")
        {
        }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Club> Clubs { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Player> Players { get; set; }

        //public System.Data.Entity.DbSet<BadmintonSvc.Models.Queue> Queues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<Club>()
            //    .HasMany(c => c.Players).WithMany(i => i.Clubs)
            //    .Map(t => t.MapLeftKey("ClubID")
            //        .MapRightKey("PlayerID")
            //        .ToTable("PlayerClub"));
        }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Queue> Queues { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Skillset> Skillsets { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.QStatus> QStatus { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Price> Prices { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.QueueTemp> QueueTemps { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Logger> Logs { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Game> Games { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Court> Courts { get; set; }

        public System.Data.Entity.DbSet<BadmintonSvc.Models.Process> Processes { get; set; }
    }
    }
