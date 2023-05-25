using Indigo.Areas.Admin.ViewModels;
using Indigo.DAL;
using Indigo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Indigo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PostsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.Where(i => !i.IsDeleted).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostVM createPostVM)
        {
            if (!ModelState.IsValid) return View();

            bool isExists = await _context.Posts.AnyAsync
                (s => s.Title.ToLower().Trim() == createPostVM.Title.ToLower().Trim());

            if (isExists)
            {
                ModelState.AddModelError("Title", "Post with this title already exists");
                return View();
            }
            if (!createPostVM.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "File type must be image");
                return View();
            }
            if (createPostVM.Photo.Length / 1024 > 200)
            {
                ModelState.AddModelError("Photo", "File type must be less than 200kb");
            }

            string rootPath = Path.Combine(_environment.WebRootPath, "assets", "images");
            string fileName = Guid.NewGuid().ToString() + createPostVM.Photo.FileName;
            string resultPath = Path.Combine(rootPath, fileName);
            using (FileStream fileStream = new(resultPath, FileMode.Create))
            {
                await createPostVM.Photo.CopyToAsync(fileStream);
            }


            Post post = new()
            {
                Title = createPostVM.Title,
                Description = createPostVM.Description,
                ImagePath = fileName,
                IsDeleted = createPostVM.IsDeleted
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return View();
        }

        public ActionResult Update(int id)
        {
            return View(new UpdatePostVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdatePostVM updatePostVM)
        {
            if (!ModelState.IsValid) return View(updatePostVM);


            Post? post = await _context.Posts.FirstOrDefaultAsync(s=> s.Id == updatePostVM.Id);
            if (post == null)
            {
                return NotFound();
            }

            if (!updatePostVM.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "File type must be image");
                return View();
            }
            if (updatePostVM.Photo.Length / 1024 > 200)
            {
                ModelState.AddModelError("Photo", "File type must be less than 200kb");
                return View();
            }

            string rootPath = Path.Combine(_environment.WebRootPath, "assets", "images");

            string oldFilePath = Path.Combine(rootPath, post.ImagePath);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            string newFileName = Guid.NewGuid().ToString() + updatePostVM.Photo.FileName;
            string resultPath = Path.Combine(rootPath, newFileName);
            using (FileStream fileStream = new(resultPath, FileMode.Create))
            {
                await updatePostVM.Photo.CopyToAsync(fileStream);
            }

            post.Title = updatePostVM.Title;
            post.Description = updatePostVM.Description;
            post.ImagePath = newFileName;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Post? post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            string filePath = Path.Combine(_environment.WebRootPath, "assets", "images",
                post.ImagePath);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
