using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Service.UI;
using UnganaConnect.Models.Training___Learning;
using UnganaConnect.Models.Training___Learning; // for CourseProgress, UserProgress

namespace UnganaConnect.Web.Controllers
{
    [Route("learning")]
    public class LearningController : Controller
    {
        private readonly ApiService _apiService;
        private readonly AuthService _authService;

        public LearningController(ApiService apiService, AuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
        }

        [HttpGet("mycourses")]
        public async Task<IActionResult> MyCourses()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("LoginView", "Auth");
            }
            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();
            var enrollments = await _apiService.GetAsync<List<Enrollment>>("Enrollment/my-enrollments");
            return View(enrollments ?? new List<Enrollment>());
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> Course(int courseId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("LoginView", "Auth");
            }

            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();

            var course = await _apiService.GetAsync<Course>($"Course/{courseId}");
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("MyCourses");
            }
            var modules = await _apiService.GetAsync<List<Module>>($"Module/course/{courseId}");
            course.Modules = modules ?? new List<Module>();
            var progress = await _apiService.GetAsync<CourseProgress>($"Progress/course/{courseId}");
            ViewBag.CourseProgress = progress;
            return View(course);
        }

        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> Module(int moduleId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("LoginView", "Auth");
            }
            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();
            var module = await _apiService.GetAsync<Module>($"Module/{moduleId}");
            if (module == null)
            {
                TempData["Error"] = "Module not found.";
                return RedirectToAction("MyCourses");
            }
            var course = await _apiService.GetAsync<Course>($"Course/{module.CourseId}");
            module.Course = course;
            var quizzes = await _apiService.GetAsync<List<Quiz>>($"Quiz/module/{moduleId}");
            ViewBag.Quizzes = quizzes ?? new List<Quiz>();
            var progress = await _apiService.GetAsync<UserProgress>($"Progress/module/{moduleId}");
            ViewBag.ModuleProgress = progress;
            return View(module);
        }

        [HttpPost("complete/{moduleId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteModule(int moduleId)
        {
            if (!_authService.IsAuthenticated())
            {
                return Unauthorized();
            }
            await _apiService.PostAsync<object>($"Progress/complete/{moduleId}", new { });
            TempData["Success"] = "Module marked as complete!";
            return RedirectToAction("Module", new { moduleId });
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> Quiz(int quizId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("LoginView", "Auth");
            }
            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();
            var quiz = await _apiService.GetAsync<Quiz>($"Quiz/{quizId}");
            if (quiz == null)
            {
                TempData["Error"] = "Quiz not found.";
                return RedirectToAction("MyCourses");
            }
            var questions = await _apiService.GetAsync<List<Question>>($"Question/quiz/{quizId}");
            quiz.Questions = questions ?? new List<Question>();
            foreach (var question in quiz.Questions)
            {
                var options = await _apiService.GetAsync<List<Option>>($"Option/question/{question.Id}");
                question.Options = options ?? new List<Option>();
            }
            return View(quiz);
        }

        [HttpPost("submit-quiz")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuiz(QuizSubmissionViewModel model)
        {
            if (!_authService.IsAuthenticated())
            {
                return Unauthorized();
            }
            var result = await _apiService.PostAsync<QuizResultViewModel>("Quiz/submit", model);
            if (result != null)
            {
                TempData["QuizResult"] = result;
                return RedirectToAction("QuizResult", new { quizId = model.QuizId });
            }
            TempData["Error"] = "Error submitting quiz.";
            return RedirectToAction("Quiz", new { quizId = model.QuizId });
        }

        [HttpGet("quiz-result/{quizId}")]
        public IActionResult QuizResult(int quizId)
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("LoginView", "Auth");
            }
            var result = TempData["QuizResult"] as QuizResultViewModel;
            if (result == null)
            {
                TempData["Error"] = "Quiz result not found.";
                return RedirectToAction("MyCourses");
            }
            return View(result);
        }

        [HttpGet("certificates")]
        public async Task<IActionResult> Certificates()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("LoginView", "Auth");
            }
            ViewBag.IsAdmin = _authService.IsAdmin();
            ViewBag.UserEmail = _authService.GetUserEmail();
            var certificates = await _apiService.GetAsync<List<Certificate>>("Certificate/my-certificates");
            return View(certificates ?? new List<Certificate>());
        }

        [HttpPost("generate-certificate/{courseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateCertificate(int courseId)
        {
            if (!_authService.IsAuthenticated())
            {
                return Unauthorized();
            }
            await _apiService.PostAsync<object>($"Certificate/generate/{courseId}", new { });
            TempData["Success"] = "Certificate generated successfully!";
            return RedirectToAction("Certificates");
        }
    }
}


