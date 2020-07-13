using Microsoft.EntityFrameworkCore;
using SecretGarden.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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
               // entity.Property(b => b.IsDeleted).IsRequired().HasDefaultValue(false);
            });

            modelBuilder.Entity<ReleaseInformation>(entity => {
                // entity.HasNoKey();
                entity.ToTable("sg_releaseInformation");
                //entity.Property(b => b.IsDeleted).IsRequired().HasDefaultValue(false);
            });
        }

        #region 定义逻辑删除字段更新规则
        /// <summary>
        /// 重写SaveChanges方法
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        /// <summary>
        /// 重写SaveChangesAsync方法
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        /// <summary>
        /// 定义逻辑删除字段更新规则
        /// </summary>
        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["IsDeleted"] = false;
                        entry.CurrentValues["CreateTime"] = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Property("CreateTime").IsModified = false;
                        entry.CurrentValues["EditTime"] = DateTime.Now;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsDeleted"] = true;
                        entry.CurrentValues["EditTime"] = DateTime.Now;
                        break;
                }
            }
        }
        #endregion
    }
}
