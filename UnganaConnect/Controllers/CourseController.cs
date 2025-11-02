using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using UnganaConnect.Data;
using UnganaConnect.Frontend.Controllers;
using UnganaConnect.Frontend.Services;
using UnganaConnect.Models.Course;

namespace UnganaConnect.Controllers
{
    public class CourseController : Controller
    {
        private readonly UnganaConnectDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<CourseController> _logger;

        public CourseController(UnganaConnectDbContext context, BlobServiceClient blobServiceClient, ILogger<CourseController> logger)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
            _logger= logger;
        }
 
        // LIST COURSES
        public async Task<IActionResult> Index(string search = "", string category = "all")
        {
            var role = HttpContext.Session.GetString("Role");

            var query = _context.Courses
                .Include(c => c.Modules)
                .AsQueryable();

            if (role != "Admin")
                query = query.Where(c => c.Status != "draft");

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Title.Contains(search) || c.Description.Contains(search));

            if (category != "all")
                query = query.Where(c => c.Category == category);

            var model = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();

            return View(model);
        }

         

        // COURSE DETAILS
         
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .ThenInclude(m => m.Quizzes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            // Determine enrollment state for the current user (if logged in)
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var guidUserId = Guid.Parse(userId);
                var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.CourseId == id && e.UserId == guidUserId);
                ViewBag.IsEnrolled = enrollment != null;
                ViewBag.EnrollmentProgress = enrollment?.Progress ?? 0;
                ViewBag.EnrollmentCompleted = enrollment?.Completed ?? false;
            }
            else
            {
                ViewBag.IsEnrolled = false;
            }

            return View(course);
        }


        // COURSE EROLLMENT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");
            _logger.LogInformation("[Enroll] Start - CourseId={CourseId}, UserId={UserId}, Role={Role}", courseId, userId, role);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");
            if (!string.Equals(role, "Member", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Only members can enroll in courses.";
                _logger.LogWarning("[Enroll] Blocked - Non-member attempted enrollment. Role={Role}", role);
                return RedirectToAction("Details", new { id = courseId });
            }

            var guidUserId = Guid.Parse(userId);

            // Prevent duplicate enrollment
            var existing = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == guidUserId);

            if (existing != null)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                _logger.LogInformation("[Enroll] Already enrolled - CourseId={CourseId}, UserId={UserId}", courseId, userId);
                return RedirectToAction("Details", new { id = courseId });
            }

            var enrollment = new CourseEnrollment
            {
                CourseId = courseId,
                UserId = guidUserId,
                Progress = 0,
                EnrolledAt = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Optionally update course counters
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course != null)
            {
                course.Enrolled += 1;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Successfully enrolled in course!";
            _logger.LogInformation("[Enroll] Success - CourseId={CourseId}, UserId={UserId}", courseId, userId);
            return RedirectToAction("Details", new { id = courseId });
        }


         
        // CREATE COURSE
         
        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course model, IFormFile? thumbnailFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            //  Upload Thumbnail to Azure Blob
            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("course-thumbnails");
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobName = $"{Guid.NewGuid()}_{thumbnailFile.FileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                using (var stream = thumbnailFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = thumbnailFile.ContentType });
                }

                model.ThumbnailUrl = blobClient.Uri.ToString();
                model.ThumbnailFileName = thumbnailFile.FileName;
                model.ThumbnailContentType = thumbnailFile.ContentType;
            }

            model.CreatedAt = DateTime.UtcNow;
            model.Status = "draft";

            _context.Courses.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course created successfully!";
            return RedirectToAction("Index");
        }

         
        // EDIT COURSE
         
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "Instructor")
                return RedirectToAction("Index");

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Course model, IFormFile? newThumbnail)
        {
            

                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                    return NotFound();

                if (!ModelState.IsValid)
                    return View(model);

                course.Title = model.Title;
                course.Description = model.Description;
                course.Category = model.Category;
                course.Duration = model.Duration;
                course.Level = model.Level;


                if (newThumbnail != null)
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient("course-thumbnails");
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                    var blobName = $"{Guid.NewGuid()}_{newThumbnail.FileName}";
                    var blobClient = containerClient.GetBlobClient(blobName);

                    using (var stream = newThumbnail.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = newThumbnail.ContentType });
                    }

                    course.ThumbnailUrl = blobClient.Uri.ToString();
                    course.ThumbnailFileName = newThumbnail.FileName;
                    course.ThumbnailContentType = newThumbnail.ContentType;
                }
            


            await _context.SaveChangesAsync();
            TempData["Success"] = "Course updated successfully!";
            return RedirectToAction("Index");
        }

         
        // DELETE COURSE
         
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.Include(c => c.Modules).FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course deleted successfully!";
            return RedirectToAction("Index");
        }

         
        // PUBLISH COURSE
         
        [HttpPost]
        public async Task<IActionResult> Publish(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("Index");

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            course.Status = "available";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Course published successfully!";
            return RedirectToAction("Index");
        }

        // ==============================
        // MODULE CREATION
        // ==============================

        // GET: /Modules/CreateModule?courseId=1
        [HttpGet]
        public async Task<IActionResult> CreateModule(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound();

            ViewBag.CourseTitle = course.Title;
            return View(new Module { CourseId = courseId });
        }

        // POST: /Modules/CreateModule
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModule(Module model, IFormFile? videoFile)
        {
            _logger.LogInformation("[CreateModule] Start - CourseId={CourseId}, Title={Title}", model.CourseId, model.Title);

            // Ignore validation for navigation property not posted by form
            ModelState.Remove("Course");

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState
                    .Where(kv => kv.Value!.Errors.Count > 0)
                    .Select(kv => new { Field = kv.Key, Errors = string.Join(" | ", kv.Value!.Errors.Select(e => e.ErrorMessage)) });
                foreach (var e in allErrors)
                {
                    _logger.LogWarning("[CreateModule] ModelState error - Field={Field}, Errors={Errors}", e.Field, e.Errors);
                }
                var errorCount = ModelState.Values.Sum(v => v.Errors.Count);
                _logger.LogWarning("[CreateModule] Invalid model state with {ErrorCount} errors for CourseId={CourseId}", errorCount, model.CourseId);
                return View(model);
            }
            
            // Ensure the course exists
            var course = await _context.Courses.FindAsync(model.CourseId);
            if (course == null)
            {
                _logger.LogWarning("[CreateModule] Course not found. CourseId={CourseId}", model.CourseId);
                return NotFound();
            }
            

            // Upload video to Azure Blob Storage if provided
            if (videoFile != null && videoFile.Length > 0)
            {
                try
                {
                    _logger.LogInformation("[CreateModule] Uploading video FileName={FileName}, Length={Length}", videoFile.FileName, videoFile.Length);
                    var containerClient = _blobServiceClient.GetBlobContainerClient("module-videos");
                    await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                    var blobName = $"{Guid.NewGuid()}_{Path.GetFileName(videoFile.FileName)}";
                    var blobClient = containerClient.GetBlobClient(blobName);

                    using (var stream = videoFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders
                        {
                            ContentType = videoFile.ContentType
                        });
                    }

                    // Save blob info to model
                    model.VideoUrl = blobClient.Uri.ToString();
                    model.VideoFileName = videoFile.FileName;
                    model.VideoContentType = videoFile.ContentType;
                    model.VideoFileSize = videoFile.Length;

                    _logger.LogInformation("[CreateModule] Video uploaded successfully. BlobUri={BlobUri}", model.VideoUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Video upload failed: {ex.Message}");
                    _logger.LogError(ex, "[CreateModule] Video upload failed. CourseTitle={CourseTitle}", course.Title);
                    return View(model);
                }
            }
            else
            {
                _logger.LogInformation("[CreateModule] No video uploaded for this module.");
            }

            //Save module to database
            _context.Modules.Add(model);
            await _context.SaveChangesAsync();

            _logger.LogInformation("[CreateModule] Module persisted. ModuleId={ModuleId}, Title={ModuleTitle}, CourseTitle={CourseTitle}", model.Id, model.Title, course.Title);
            TempData["Success"] = "Module created successfully!";
            return RedirectToAction("Details", "Course", new { id = model.CourseId });
        }
    

// ==============================
// EDIT MODULE
// ==============================
[HttpGet]
        public async Task<IActionResult> EditModule(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound();

            return View(module);
        }

        [HttpPost]
        public async Task<IActionResult> EditModule(int id, Module model, IFormFile? newVideo)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            module.Title = model.Title;
            module.Content = model.Content;
            module.Order = model.Order;

            if (newVideo != null && newVideo.Length > 0)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("module-videos");
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobName = $"{Guid.NewGuid()}_{newVideo.FileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                using (var stream = newVideo.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = newVideo.ContentType });
                }

                module.VideoUrl = blobClient.Uri.ToString();
                module.VideoFileName = newVideo.FileName;
                module.VideoContentType = newVideo.ContentType;
                module.VideoFileSize = newVideo.Length;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Module updated successfully!";
            return RedirectToAction("Details", new { id = module.CourseId });
        }

        // ==============================
        // DELETE MODULE
        // ==============================
        [HttpPost]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
                return NotFound();

            var courseId = module.CourseId;

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Module deleted successfully!";
            return RedirectToAction("Details", new { id = courseId });
        }


        // ==============================
        // CREATE QUIZ FOR MODULE
        // ==============================
        [HttpGet]
        public async Task<IActionResult> CreateQuiz(int moduleId)
        {
            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
                return NotFound();

            ViewBag.ModuleTitle = module.Title;
            return View(new Quiz { ModuleId = moduleId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz(Quiz model, List<string> questionTexts, List<string[]> options, List<int> correctOptionIndexes)
        {
            // Suppress navigation property that is not posted by the form
            ModelState.Remove("Module");
            // Log all ModelState errors for debugging
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}").ToList();
                _logger.LogWarning("[CreateQuiz] ModelState errors: {Errors}", string.Join(" | ", errors));
                return View(model);
            }

            // Check module exists
            var module = await _context.Modules.FindAsync(model.ModuleId);
            if (module == null)
                return NotFound();

            // Add the quiz to the module
            _context.Quizzes.Add(model);
            await _context.SaveChangesAsync();

            // Parse options into questions/options, binding each string[] as the options for a question
            for (int i = 0; i < questionTexts.Count; i++)
            {
                var question = new QuizQuestion
                {
                    QuizId = model.Id,
                    QuestionText = questionTexts[i],
                    QuestionType = "multiple-choice"
                };
                _context.QuizQuestions.Add(question);
                await _context.SaveChangesAsync();
                var optionArr = options.Count > i ? options[i] : new string[0];
                for (int j = 0; j < optionArr.Length; j++)
                {
                    var option = new QuizOption
                    {
                        QuestionId = question.Id,
                        Text = optionArr[j],
                        IsCorrect = (correctOptionIndexes.Count > i && j == correctOptionIndexes[i])
                    };
                    _context.QuizOptions.Add(option);
                }
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Quiz created successfully!";
            return RedirectToAction("Details", new { id = module.CourseId });
        }

        // ==============================
        // EDIT QUIZ
        // ==============================
        [HttpGet]
        public async Task<IActionResult> EditQuiz(int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound();

            return View(quiz);
        }

        // For simplicity, editing will replace questions/options
        [HttpPost]
        public async Task<IActionResult> EditQuiz(int quizId, Quiz model, List<string> questionTexts, List<List<string>> options, List<int> correctOptionIndexes)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound();

            quiz.Title = model.Title;
            quiz.Instructions = model.Instructions;

            // Remove existing questions/options
            _context.QuizOptions.RemoveRange(quiz.Questions.SelectMany(q => q.Options));
            _context.QuizQuestions.RemoveRange(quiz.Questions);
            await _context.SaveChangesAsync();

            // Re-add questions/options
            for (int i = 0; i < questionTexts.Count; i++)
            {
                var question = new QuizQuestion
                {
                    QuizId = quiz.Id,
                    QuestionText = questionTexts[i],
                    QuestionType = "multiple-choice"
                };

                _context.QuizQuestions.Add(question);
                await _context.SaveChangesAsync();

                for (int j = 0; j < options[i].Count; j++)
                {
                    var option = new QuizOption
                    {
                        QuestionId = question.Id,
                        Text = options[i][j],
                        IsCorrect = (j == correctOptionIndexes[i])
                    };
                    _context.QuizOptions.Add(option);
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Quiz updated successfully!";
            return RedirectToAction("Details", new { id = quiz.Module.CourseId });
        }

        // ==============================
        // DELETE QUIZ
        // ==============================
        [HttpPost]
        public async Task<IActionResult> DeleteQuiz(int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound();

            var courseId = quiz.Module.CourseId;

            _context.QuizOptions.RemoveRange(quiz.Questions.SelectMany(q => q.Options));
            _context.QuizQuestions.RemoveRange(quiz.Questions);
            _context.Quizzes.Remove(quiz);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Quiz deleted successfully!";
            return RedirectToAction("Details", new { id = courseId });
        }

        // ==============================
        // TAKE QUIZ (STUDENT)
        // ==============================
        [HttpGet]
        public async Task<IActionResult> TakeQuiz(int quizId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .Include(q => q.Module)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound();

            return View(quiz);
        }

        // Submits selected options: form should post pairs like SelectedAnswers[questionId] = optionId
        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(int quizId, Dictionary<int, int> SelectedAnswers)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var guidUserId = Guid.Parse(userId);

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .Include(q => q.Module)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound();

            int total = quiz.Questions.Count;
            int correct = 0;

            foreach (var question in quiz.Questions)
            {
                if (SelectedAnswers != null && SelectedAnswers.TryGetValue(question.Id, out var selectedOptionId))
                {
                    var selected = question.Options.FirstOrDefault(o => o.Id == selectedOptionId);
                    if (selected != null && selected.IsCorrect)
                        correct++;
                }
            }

            double score = total == 0 ? 0 : (correct * 100.0 / total);

            var attempt = new QuizAttempt
            {
                QuizId = quiz.Id,
                UserId = guidUserId,
                Score = score,
                AttemptedAt = DateTime.UtcNow
            };
            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            await RecalculateEnrollmentProgress(guidUserId, quiz.Module.CourseId);

            TempData["Success"] = $"Quiz submitted. Score: {Math.Round(score)}%";
            return RedirectToAction("Details", new { id = quiz.Module.CourseId });
        }

        private async Task RecalculateEnrollmentProgress(Guid userId, int courseId)
        {
            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
            if (enrollment == null)
                return;

            var courseQuizIds = await _context.Quizzes
                .Include(q => q.Module)
                .Where(q => q.Module.CourseId == courseId)
                .Select(q => q.Id)
                .ToListAsync();

            if (courseQuizIds.Count == 0)
            {
                // No quizzes: keep existing progress
                return;
            }

            // Count passing attempts (>=50) per quiz (at least one passing attempt counts)
            var passingAttempts = await _context.QuizAttempts
                .Where(a => a.UserId == userId && courseQuizIds.Contains(a.QuizId) && a.Score >= 50)
                .Select(a => a.QuizId)
                .Distinct()
                .CountAsync();

            var progress = (int)Math.Round(passingAttempts * 100.0 / courseQuizIds.Count);
            enrollment.Progress = progress;
            enrollment.Completed = progress >= 100;
            await _context.SaveChangesAsync();
        }

        // ==============================
        // CERTIFICATE DOWNLOAD
        // ==============================
        [HttpGet]
        public async Task<IActionResult> Certificate(int courseId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var guidUserId = Guid.Parse(userId);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
                return NotFound();

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == guidUserId);
            if (enrollment == null || !enrollment.Completed)
            {
                TempData["Error"] = "Complete the course to download your certificate.";
                return RedirectToAction("Details", new { id = courseId });
            }

            var vm = new UnganaConnect.Frontend.Models.CertificateViewModel
            {
                CourseTitle = course.Title,
                Instructor = "Ungana Instructor",
                CompletionDate = enrollment.EnrolledAt.ToString("yyyy-MM-dd"),
                UserName = HttpContext.Session.GetString("Name") ?? "Student",
                CertificateId = $"CERT-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
            };

            return View(vm);
        }

        public IActionResult Learn(int id)
        {
            var module = _context.Modules
                .Include(m => m.Course)
                .Include(m => m.Quizzes)
                .FirstOrDefault(m => m.Id == id);

            if (module == null)
                return NotFound();

            return View(module); 


        }

        

    }
}
