using Microsoft.EntityFrameworkCore;
using SecretGarden.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SecretGarden.Repositories
{
    public class SecretGardenContext :DbContext
    {
        public SecretGardenContext(DbContextOptions<SecretGardenContext> options):base(options)
        {

        }

        public virtual DbSet<People> Peoples { get; set; }
        public virtual DbSet<ReleaseInformation> ReleaseInformations { get; set; }
        public virtual DbSet<MsgModel> MsgModels { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<People>(entity=> {
               // entity.HasNoKey();
                entity.ToTable("sg_people");
            });

            modelBuilder.Entity<ReleaseInformation>(entity => {
                // entity.HasNoKey();
                entity.ToTable("sg_releaseInformation");
            });
        }
    }
}
