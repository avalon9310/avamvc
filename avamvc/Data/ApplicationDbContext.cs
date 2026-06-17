using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using avamvc.Models; // 要加這行，才能找到 Post 類別

namespace avamvc.Data {
    public class ApplicationDbContext : IdentityDbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<Post> Posts { get; set; } // 資料庫模型

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            // 強制指定 Posts 的 Id 欄位在 PostgreSQL 中是自動遞增的流水號
            builder.Entity<Post>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
