using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Training___Learning;

namespace UnganaConnect.Controllers.Course
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly UnganaConnectDbcontext _context;
        public QuizController(UnganaConnectDbcontext context)
        {
            _context = context;
        }

        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> GetByModule(int moduleId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.ModuleId == moduleId);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return Ok(quiz);
        }

        public class QuizSubmission
        {
            public int ModuleId { get; set; }
            public string UserId { get; set; } = string.Empty;
            public Dictionary<int, int> Answers { get; set; } = new(); // QuestionId -> OptionId
        }

        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] QuizSubmission submission)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.ModuleId == submission.ModuleId);
            if (quiz == null) return NotFound("Quiz not found for module");

            int correct = 0;
            foreach (var q in quiz.Questions)
            {
                if (submission.Answers.TryGetValue(q.Id, out var chosen) && chosen == q.CorrectOptionId)
                    correct++;
            }

            var total = quiz.Questions.Count;
            var passed = total == 0 || correct * 100 >= 70 * total / 1; // >=70%

            if (passed)
            {
                // Mark module complete via ProgressController logic (duplicate minimal logic)
                var exists = await _context.UserProgress
                    .FirstOrDefaultAsync(p => p.ModuleId == submission.ModuleId && p.UserId == submission.UserId);
                if (exists == null)
                {
                    _context.UserProgress.Add(new Models.Training___Learning.UserProgress
                    {
                        ModuleId = submission.ModuleId,
                        UserId = submission.UserId,
                        IsCompleted = true
                    });
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { correct, total, passed });
        }
    }
}
