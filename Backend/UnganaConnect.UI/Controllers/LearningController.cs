using Microsoft.AspNetCore.Mvc;
using UnganaConnect.UI.Models;
using UnganaConnect.UI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnganaConnect.UI.Controllers
{
    public class LearningController : Controller
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;

        public LearningController(ApiService apiService, AuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        // GET: Learning/MyCourses
        public async Task<IActionResult> MyCourses()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();

            try
            {
                var enrollments = await _apiService.GetAsync<List<Enrollment>>("Enrollment/my-enrollments");
                return View(enrollments ?? new List<Enrollment>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading your courses: " + ex.Message;
                return View(new List<Enrollment>());
            }
        }

        // GET: Learning/Course/{courseId}
        public async Task<IActionResult> Course(int courseId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();

            try
            {
                // Get course details
                var course = await _apiService.GetAsync<Course>($"Course/{courseId}");
                if (course == null)
                {
                    TempData["Error"] = "Course not found.";
                    return RedirectToAction("MyCourses");
                }

                // Get modules for this course
                var modules = await _apiService.GetAsync<List<Module>>($"Module/course/{courseId}");
                course.Modules = modules ?? new List<Module>();

                // Get user's progress for this course
                var progress = await _apiService.GetAsync<CourseProgress>($"Progress/course/{courseId}");
                ViewBag.CourseProgress = progress;

                return View(course);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading course: " + ex.Message;
                return RedirectToAction("MyCourses");
            }
        }

        // GET: Learning/Module/{moduleId}
        public async Task<IActionResult> Module(int moduleId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();

            try
            {
                // Get module details
                var module = await _apiService.GetAsync<Module>($"Module/{moduleId}");
                if (module == null)
                {
                    TempData["Error"] = "Module not found.";
                    return RedirectToAction("MyCourses");
                }

                // Get course details
                var course = await _apiService.GetAsync<Course>($"Course/{module.CourseId}");
                module.Course = course;

                // Get quizzes for this module
                var quizzes = await _apiService.GetAsync<List<Quiz>>($"Quiz/module/{moduleId}");
                module.Quizzes = quizzes ?? new List<Quiz>();

                // Get user's progress for this module
                var progress = await _apiService.GetAsync<UserProgress>($"Progress/module/{moduleId}");
                ViewBag.ModuleProgress = progress;

                return View(module);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading module: " + ex.Message;
                return RedirectToAction("MyCourses");
            }
        }

        // POST: Learning/CompleteModule/{moduleId}
        [HttpPost]
        public async Task<IActionResult> CompleteModule(int moduleId)
        {
            if (!_authService.IsAuthenticated())
            {
                return Unauthorized();
            }

            try
            {
                await _apiService.PostAsync<object>($"Progress/complete/{moduleId}", new { });
                TempData["Success"] = "Module marked as complete!";
                return RedirectToAction("Module", new { moduleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error completing module: " + ex.Message;
                return RedirectToAction("Module", new { moduleId });
            }
        }

        // GET: Learning/Quiz/{quizId}
        public async Task<IActionResult> Quiz(int quizId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();

            try
            {
                // Get quiz details with questions and options
                var quiz = await _apiService.GetAsync<Quiz>($"Quiz/{quizId}");
                if (quiz == null)
                {
                    TempData["Error"] = "Quiz not found.";
                    return RedirectToAction("MyCourses");
                }

                // Get questions for this quiz
                var questions = await _apiService.GetAsync<List<Question>>($"Question/quiz/{quizId}");
                quiz.Questions = questions ?? new List<Question>();

                // Get options for each question
                foreach (var question in quiz.Questions)
                {
                    var options = await _apiService.GetAsync<List<Option>>($"Option/question/{question.Id}");
                    question.Options = options ?? new List<Option>();
                }

                return View(quiz);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading quiz: " + ex.Message;
                return RedirectToAction("MyCourses");
            }
        }

        // POST: Learning/SubmitQuiz
        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(QuizSubmissionViewModel model)
        {
            if (!_authService.IsAuthenticated())
            {
                return Unauthorized();
            }

            try
            {
                var result = await _apiService.PostAsync<QuizResultViewModel>("Quiz/submit", model);

                if (result != null)
                {
                    TempData["QuizResult"] = result;
                    return RedirectToAction("QuizResult", new { quizId = model.QuizId });
                }
                else
                {
                    TempData["Error"] = "Error submitting quiz.";
                    return RedirectToAction("Quiz", new { quizId = model.QuizId });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error submitting quiz: " + ex.Message;
                return RedirectToAction("Quiz", new { quizId = model.QuizId });
            }
        }

        // GET: Learning/QuizResult/{quizId}
        public async Task<IActionResult> QuizResult(int quizId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();

            var result = TempData["QuizResult"] as QuizResultViewModel;
            if (result == null)
            {
                TempData["Error"] = "Quiz result not found.";
                return RedirectToAction("MyCourses");
            }

            return View(result);
        }

        // GET: Learning/Certificates
        public async Task<IActionResult> Certificates()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();

            try
            {
                var certificates = await _apiService.GetAsync<List<Certificate>>("Certificate/my-certificates");
                return View(certificates ?? new List<Certificate>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading certificates: " + ex.Message;
                return View(new List<Certificate>());
            }
        }

        // POST: Learning/GenerateCertificate/{courseId}
        [HttpPost]
        public async Task<IActionResult> GenerateCertificate(int courseId)
        {
            if (!_authService.IsAuthenticated())
            {
                return Unauthorized();
            }

            try
            {
                await _apiService.PostAsync<object>($"Certificate/generate/{courseId}", new { });
                TempData["Success"] = "Certificate generated successfully!";
                return RedirectToAction("Certificates");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating certificate: " + ex.Message;
                return RedirectToAction("Certificates");
            }
        }
    }
}
