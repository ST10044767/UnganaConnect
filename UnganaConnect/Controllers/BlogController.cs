using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Services;
using UnganaConnect.Models.Blog;

namespace UnganaConnect.Controllers
{
    public class BlogController : Controller
    {
        private readonly UnganaConnectDbContext _context;
        private readonly AzureBlobService _blobService;

        public BlogController(UnganaConnectDbContext context, AzureBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: /Blog
        public async Task<IActionResult> Index(string? category, string? search, string? status)
        {
            var query = _context.BlogPosts
                .Include(b => b.Author)
                .AsQueryable();

            // Filter by status (for admins/instructors)
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole == "Admin" || userRole == "Instructor")
            {
                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    query = query.Where(b => b.Status == status);
                }
            }
            else
            {
                // Non-admin users only see published posts
                query = query.Where(b => b.Status == "Published");
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category == category);
            }

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search) || 
                                        b.Content.Contains(search) || 
                                        (b.Tags != null && b.Tags.Contains(search)));
            }

            var posts = await query
                .OrderByDescending(b => b.IsFeatured)
                .ThenByDescending(b => b.PublishedAt ?? b.CreatedAt)
                .ToListAsync();

            var viewModel = posts.Select(p => new BlogPostViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Excerpt = p.Excerpt ?? (p.Content.Length > 200 ? p.Content.Substring(0, 200) + "..." : p.Content),
                FeaturedImageUrl = p.FeaturedImageUrl,
                Category = p.Category,
                Tags = p.Tags,
                Status = p.Status,
                Views = p.Views,
                IsFeatured = p.IsFeatured,
                AuthorId = p.AuthorId,
                AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                AuthorProfilePicture = p.Author.ProfilePictureUrl,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                PublishedAt = p.PublishedAt
            }).ToList();

            ViewBag.Categories = await _context.BlogPosts
                .Where(b => b.Status == "Published")
                .Select(b => b.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.StatusFilter = status ?? "All";
            ViewBag.CategoryFilter = category ?? "All";
            ViewBag.SearchTerm = search ?? "";

            return View(viewModel);
        }

        // GET: /Blog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.BlogPosts
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (post == null)
                return NotFound();

            // Check if user can view unpublished posts
            var userRole = HttpContext.Session.GetString("Role");
            if (post.Status != "Published" && userRole != "Admin" && userRole != "Instructor")
            {
                TempData["Error"] = "This post is not available.";
                return RedirectToAction("Index");
            }

            // Increment view count
            post.Views++;
            await _context.SaveChangesAsync();

            var viewModel = new BlogPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Excerpt = post.Excerpt,
                FeaturedImageUrl = post.FeaturedImageUrl,
                Category = post.Category,
                Tags = post.Tags,
                Status = post.Status,
                Views = post.Views,
                IsFeatured = post.IsFeatured,
                AllowComments = post.AllowComments,
                AuthorId = post.AuthorId,
                AuthorName = $"{post.Author.FirstName} {post.Author.LastName}",
                AuthorProfilePicture = post.Author.ProfilePictureUrl,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                PublishedAt = post.PublishedAt
            };

            return View(viewModel);
        }

        // GET: /Blog/Create
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please log in to create a blog post.";
                return RedirectToAction("Login", "Auth");
            }

            if (role != "Admin" && role != "Instructor")
            {
                TempData["Error"] = "Only Admins and Instructors can create blog posts.";
                return RedirectToAction("Index");
            }

            return View(new CreateBlogPostViewModel());
        }

        // POST: /Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogPostViewModel model, IFormFile? featuredImage)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(userId) || (role != "Admin" && role != "Instructor"))
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Index");
            }

            var guidUserId = Guid.Parse(userId);

            if (!ModelState.IsValid)
                return View(model);

            var post = new BlogPost
            {
                Title = model.Title,
                Content = model.Content,
                Excerpt = model.Excerpt ?? (model.Content.Length > 500 ? model.Content.Substring(0, 500) + "..." : model.Content),
                Category = model.Category,
                Tags = model.Tags,
                Status = model.Status,
                IsFeatured = model.IsFeatured,
                AllowComments = model.AllowComments,
                AuthorId = guidUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PublishedAt = model.Status == "Published" ? DateTime.UtcNow : null
            };

            // Handle featured image upload
            if (featuredImage != null && featuredImage.Length > 0)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif" };
                if (!allowedTypes.Contains(featuredImage.ContentType))
                {
                    ModelState.AddModelError("FeaturedImage", "Only JPEG, PNG, and GIF images are allowed.");
                    return View(model);
                }

                if (featuredImage.Length > 10 * 1024 * 1024) // 10MB
                {
                    ModelState.AddModelError("FeaturedImage", "Image size must be less than 10MB.");
                    return View(model);
                }

                try
                {
                    var imageUrl = await _blobService.UploadAsync(featuredImage, "blog-images");
                    post.FeaturedImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("FeaturedImage", $"Error uploading image: {ex.Message}");
                    return View(model);
                }
            }

            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Blog post created successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Blog/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please log in to edit blog posts.";
                return RedirectToAction("Login", "Auth");
            }

            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
                return NotFound();

            var guidUserId = Guid.Parse(userId);
            
            // Only author, admin, or instructor can edit
            if (post.AuthorId != guidUserId && role != "Admin")
            {
                TempData["Error"] = "You can only edit your own posts.";
                return RedirectToAction("Index");
            }

            var viewModel = new CreateBlogPostViewModel
            {
                Title = post.Title,
                Content = post.Content,
                Excerpt = post.Excerpt,
                Category = post.Category,
                Tags = post.Tags,
                Status = post.Status,
                IsFeatured = post.IsFeatured,
                AllowComments = post.AllowComments
            };

            ViewBag.PostId = post.Id;
            ViewBag.CurrentFeaturedImage = post.FeaturedImageUrl;

            return View(viewModel);
        }

        // POST: /Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateBlogPostViewModel model, IFormFile? featuredImage)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please log in to edit blog posts.";
                return RedirectToAction("Login", "Auth");
            }

            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
                return NotFound();

            var guidUserId = Guid.Parse(userId);
            
            if (post.AuthorId != guidUserId && role != "Admin")
            {
                TempData["Error"] = "You can only edit your own posts.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PostId = post.Id;
                ViewBag.CurrentFeaturedImage = post.FeaturedImageUrl;
                return View(model);
            }

            // Handle featured image upload
            if (featuredImage != null && featuredImage.Length > 0)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif" };
                if (!allowedTypes.Contains(featuredImage.ContentType))
                {
                    ModelState.AddModelError("FeaturedImage", "Only JPEG, PNG, and GIF images are allowed.");
                    ViewBag.PostId = post.Id;
                    ViewBag.CurrentFeaturedImage = post.FeaturedImageUrl;
                    return View(model);
                }

                if (featuredImage.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("FeaturedImage", "Image size must be less than 10MB.");
                    ViewBag.PostId = post.Id;
                    ViewBag.CurrentFeaturedImage = post.FeaturedImageUrl;
                    return View(model);
                }

                try
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(post.FeaturedImageUrl))
                    {
                        await _blobService.DeleteAsync(post.FeaturedImageUrl);
                    }

                    var imageUrl = await _blobService.UploadAsync(featuredImage, "blog-images");
                    post.FeaturedImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("FeaturedImage", $"Error uploading image: {ex.Message}");
                    ViewBag.PostId = post.Id;
                    ViewBag.CurrentFeaturedImage = post.FeaturedImageUrl;
                    return View(model);
                }
            }

            // Update post
            post.Title = model.Title;
            post.Content = model.Content;
            post.Excerpt = model.Excerpt ?? (model.Content.Length > 500 ? model.Content.Substring(0, 500) + "..." : model.Content);
            post.Category = model.Category;
            post.Tags = model.Tags;
            post.Status = model.Status;
            post.IsFeatured = model.IsFeatured;
            post.AllowComments = model.AllowComments;
            post.UpdatedAt = DateTime.UtcNow;

            // Update published date if status changed to Published
            if (model.Status == "Published" && post.PublishedAt == null)
            {
                post.PublishedAt = DateTime.UtcNow;
            }
            else if (model.Status != "Published")
            {
                post.PublishedAt = null;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Blog post updated successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Blog/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please log in to delete blog posts.";
                return RedirectToAction("Login", "Auth");
            }

            var post = await _context.BlogPosts
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (post == null)
                return NotFound();

            var guidUserId = Guid.Parse(userId);
            
            if (post.AuthorId != guidUserId && role != "Admin")
            {
                TempData["Error"] = "You can only delete your own posts.";
                return RedirectToAction("Index");
            }

            var viewModel = new BlogPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Excerpt = post.Excerpt,
                Category = post.Category,
                Status = post.Status,
                CreatedAt = post.CreatedAt,
                AuthorName = $"{post.Author.FirstName} {post.Author.LastName}"
            };

            return View(viewModel);
        }

        // POST: /Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please log in to delete blog posts.";
                return RedirectToAction("Login", "Auth");
            }

            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
                return NotFound();

            var guidUserId = Guid.Parse(userId);
            
            if (post.AuthorId != guidUserId && role != "Admin")
            {
                TempData["Error"] = "You can only delete your own posts.";
                return RedirectToAction("Index");
            }

            // Delete featured image if exists
            if (!string.IsNullOrEmpty(post.FeaturedImageUrl))
            {
                try
                {
                    await _blobService.DeleteAsync(post.FeaturedImageUrl);
                }
                catch
                {
                    // Log error but continue with deletion
                }
            }

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Blog post deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}

