using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTest5.Model;

namespace WebCoreTest5.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<data_info> datas { get; set; }

        public DbSet<userinfo> users { get; set; }

        public DbSet<cardinfo> cards { get; set; }

        public DbSet<userinfo2> users2 { get; set; }

        public DbSet<cardinfo2> cards2 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<data_info>().ToTable("data_info").HasKey("id");
            modelBuilder.Entity<cardinfo>(entity =>
            {
                // entity.HasOne(u => u.open).WithMany(r => r.cards).HasForeignKey(p => p.openId).OnDelete(DeleteBehavior.NoAction);
                // entity.HasOne(u => u.add).WithMany(r => r.cards2).HasForeignKey(p => p.addId).OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(u => u.open).WithMany().HasForeignKey(p => p.openId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(u => u.add).WithMany().HasForeignKey(p => p.addId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<cardinfo2>(entity =>
            {
                entity.HasOne(u => u.open).WithMany().HasForeignKey(p => p.open_id).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(u => u.add).WithMany().HasForeignKey(p => p.add_id).OnDelete(DeleteBehavior.NoAction);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
