using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace avamvc.Models {
    public class Post {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? AuthorId { get; set; } // 對應 IdentityUser.Id
        public IdentityUser? Author { get; set; }

        public string? FilePath { get; set; } // 儲存檔案路徑
    }
}

