using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Services;
using UnganaConnect.Models.User;

namespace UnganaConnect.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UnganaConnectDbContext _context;
        private readonly AzureBlobService _blobService;

        public ProfileController(UnganaConnectDbContext context, AzureBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please log in to view your profile.";
                return RedirectToAction("Login", "Auth");
            }

            var guidUserId = Guid.Parse(userId);
            var user = await _context.Users.FindAsync(guidUserId);
            
            if (user == null)
                return NotFound();

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Organization = user.Organization,
                Location = user.Location,
                Website = user.Website,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return View(viewModel);
        }

        // GET: /Profile/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please log in to edit your profile.";
                return RedirectToAction("Login", "Auth");
            }

            var guidUserId = Guid.Parse(userId);
            var user = await _context.Users.FindAsync(guidUserId);
            
            if (user == null)
                return NotFound();

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Organization = user.Organization,
                Location = user.Location,
                Website = user.Website,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return View(viewModel);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model, IFormFile? profilePicture)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please log in to edit your profile.";
                return RedirectToAction("Login", "Auth");
            }

            var guidUserId = Guid.Parse(userId);
            var user = await _context.Users.FindAsync(guidUserId);
            
            if (user == null)
                return NotFound();

            // Verify user can only edit their own profile
            if (user.Id != model.Id)
            {
                TempData["Error"] = "You can only edit your own profile.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                return View(model);

            // Handle profile picture upload
            if (profilePicture != null && profilePicture.Length > 0)
            {
                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif" };
                if (!allowedTypes.Contains(profilePicture.ContentType))
                {
                    ModelState.AddModelError("ProfilePicture", "Only JPEG, PNG, and GIF images are allowed.");
                    return View(model);
                }

                // Validate file size (max 5MB)
                if (profilePicture.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfilePicture", "Image size must be less than 5MB.");
                    return View(model);
                }

                try
                {
                    // Delete old profile picture if exists
                    if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                    {
                        await _blobService.DeleteAsync(user.ProfilePictureUrl);
                    }

                    // Upload new profile picture
                    var imageUrl = await _blobService.UploadAsync(profilePicture, "profile-pictures");
                    user.ProfilePictureUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ProfilePicture", $"Error uploading image: {ex.Message}");
                    return View(model);
                }
            }

            // Update user profile
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Organization = model.Organization;
            user.Location = model.Location;
            user.Website = model.Website;
            user.Bio = model.Bio;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }
    }
}

