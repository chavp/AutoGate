using HisTrnActivityDtlWorker.Test.Domains;
using HisTrnActivityDtlWorker.Test.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisTrnActivityDtlWorker.Test.Dal
{
    public class ImmegateContext : DbContext
    {
        public DbSet<RegisteredGate> RegisteredGates { get; set; }
        public DbSet<GateActivity> GateActivities { get; set; }
        public DbSet<HisTrnActivity> HisTrnActivities { get; set; }

        static ImmegateContext()
        {
            //Database.SetInitializer<ImmegateContext>(
            //    new DropCreateDatabaseIfModelChanges<ImmegateContext>());

            Database.SetInitializer(
                new CreateDatabaseIfNotExists<ImmegateContext>());
            
            //Database.SetInitializer<ImmegateContext>(null);
        }

        public ImmegateContext()
            : base("Name=ImmegateEntities")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new RegisteredGateMap());
            modelBuilder.Configurations.Add(new GateActivityMap());
            modelBuilder.Configurations.Add(new HisTrnActivityMap());
        }
    }
}
