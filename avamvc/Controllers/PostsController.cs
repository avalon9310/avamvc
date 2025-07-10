using avamvc.Data;
using avamvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace avamvc.Controllers {
    public class PostsController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        // 所有人可見
        public async Task<IActionResult> Index() {
            return View(await _context.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        // 如果有登入 進入Create
        [Authorize]
        public IActionResult Create() {
            if (User.Identity.Name != "1avalon") {
                return Forbid();
            }
            return View();
        }

        // POST，包含 IFormFile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, IFormFile? uploadFile) {
            if (User.Identity.Name != "1avalon") {
                return Forbid();
            }

            if (!ModelState.IsValid) {
                // 把驗證錯誤印出來，方便debug
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors) {
                    Console.WriteLine(error);
                }
                return View(post);
            }

            post.CreatedAt = DateTime.Now;

            if (uploadFile != null && uploadFile.Length > 0) {
                var fileName = Path.GetFileName(uploadFile.FileName);
                var filePath = Path.Combine("wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await uploadFile.CopyToAsync(stream);
                }

                post.FilePath = "/uploads/" + fileName;
            } else {
                post.FilePath = null; // 沒上傳檔案，設定成 null
            }
            var user = await _userManager.GetUserAsync(User);
            post.AuthorId = user.Id;

            _context.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id) {
            if (id == null)
                return NotFound();

            var post = await _context.Posts
                .Include(p => p.Author) // 如果你要顯示作者資料
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
                return NotFound();

            return View(post);
        }


        // 顯示確認刪除頁面
        [Authorize]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            
            if (User.Identity.Name != "1avalon") return Forbid();

            return View(post); // 回傳確認刪除頁面
        }

        // 接收確認刪除表單送出
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }

}
