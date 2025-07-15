using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using avamvc.Models; // 要加這行，才能找到 Post 類別

namespace avamvc.Data {
    public class ApplicationDbContext : IdentityDbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<Post> Posts { get; set; } // 資料庫模型
    }
}
