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

        // 建立功能
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

        //檢視功能
        public async Task<IActionResult> Details(int? id) {
            if (id == null)
                return NotFound();

            var post = await _context.Posts
                .Include(p => p.Author) // 如果要顯示作者資料
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        // 修改功能
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? id) {
            if (User.Identity.Name != "1avalon") return Forbid();

            if (id == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            return View(post);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post editedPost, IFormFile? uploadFile, bool removeFile = false) {
            if (User.Identity.Name != "1avalon") return Forbid();

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            post.EditAt = DateTime.Now;

            if (ModelState.IsValid) {
                // 更新標題與內容
                post.Title = editedPost.Title;
                post.Content = editedPost.Content;

                // 移除附加檔案
                if (removeFile && !string.IsNullOrEmpty(post.FilePath)) {
                    var oldPath = Path.Combine("wwwroot", post.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    post.FilePath = null;
                }

                // 有新檔案上傳就覆蓋
                if (uploadFile != null && uploadFile.Length > 0) {
                    var fileName = Path.GetFileName(uploadFile.FileName);
                    var filePath = Path.Combine("wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                        await uploadFile.CopyToAsync(stream);
                    }

                    post.FilePath = "/uploads/" + fileName;
                }

                _context.Update(post);
                await _context.SaveChangesAsync();

                // ✅ 修改完回到文章列表
                return RedirectToAction("Index");
            }

            // ModelState 有錯，回到編輯畫面
            return RedirectToAction("Index");
        }






        // 刪除功能
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }

}
