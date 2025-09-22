namespace UnganaConnect.Models.Training___Learning
{
   
        public class Quiz
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public int ModuleId { get; set; }
            public Module? Module { get; set; }
            public List<Question> Questions { get; set; } = new();
        }

        public class Question
        {
            public int Id { get; set; }
            public string Text { get; set; } = "";
            public List<Option> Options { get; set; } = new();
            public int CorrectOptionId { get; set; }
        }

        public class Option
        {
            public int Id { get; set; }
            public string Text { get; set; } = "";
            public int QuestionId { get; set; }
        }

    }

