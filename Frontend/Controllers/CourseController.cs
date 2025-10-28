using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Enroll(int? id)
        {
            var course = GetCourseById(id ?? 1);
            return View(course);
        }

        [Authorize]
        public IActionResult Exam(int id)
        {
            var exam = GetExamById(id);
            return View(exam);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SubmitExam(int id, Dictionary<int, string> answers)
        {
            var score = CalculateScore(id, answers);
            if (score >= 70)
            {
                return RedirectToAction("Certificate", new { id, score });
            }
            TempData["Error"] = "Score too low. Minimum 70% required.";
            return RedirectToAction("Exam", new { id });
        }

        [Authorize]
        public IActionResult Certificate(int id, int score)
        {
            var certificate = new CertificateViewModel
            {
                CourseTitle = "Web Development Fundamentals",
                StudentName = "John Doe",
                CompletionDate = DateTime.Now.ToString("MMMM dd, yyyy"),
                Score = score
            };
            return View(certificate);
        }

        [Authorize]
        public IActionResult DownloadCertificate(int id)
        {
            var html = GenerateCertificateHtml(id);
            return File(System.Text.Encoding.UTF8.GetBytes(html), "text/html", "certificate.html");
        }

        [Authorize]
        public IActionResult Completed()
        {
            return View();
        }

        private ExamViewModel GetExamById(int id)
        {
            var exams = new Dictionary<int, ExamViewModel>
            {
                [1] = new() {
                    CourseId = 1, Title = "Web Development Fundamentals - Final Exam",
                    Questions = new List<ExamQuestion> {
                        new() { Id = 1, Question = "What does HTML stand for?", Options = new[] { "Hyper Text Markup Language", "Home Tool Markup Language", "Hyperlinks Text Markup Language" }, CorrectAnswer = "Hyper Text Markup Language" },
                        new() { Id = 2, Question = "Which CSS property is used to change text color?", Options = new[] { "color", "text-color", "font-color" }, CorrectAnswer = "color" },
                        new() { Id = 3, Question = "What is the correct way to declare a JavaScript variable?", Options = new[] { "var myVar;", "variable myVar;", "v myVar;" }, CorrectAnswer = "var myVar;" }
                    }
                },
                [2] = new() {
                    CourseId = 2, Title = "Database Management Systems - Final Exam",
                    Questions = new List<ExamQuestion> {
                        new() { Id = 1, Question = "What is the primary purpose of a database?", Options = new[] { "Store data", "Display data", "Delete data" }, CorrectAnswer = "Store data" },
                        new() { Id = 2, Question = "Which SQL command is used to retrieve data?", Options = new[] { "SELECT", "INSERT", "UPDATE" }, CorrectAnswer = "SELECT" },
                        new() { Id = 3, Question = "What does ACID stand for in database transactions?", Options = new[] { "Atomicity, Consistency, Isolation, Durability", "Access, Control, Integration, Data", "Automatic, Consistent, Independent, Durable" }, CorrectAnswer = "Atomicity, Consistency, Isolation, Durability" }
                    }
                },
                [3] = new() {
                    CourseId = 3, Title = "Cloud Computing Essentials - Final Exam",
                    Questions = new List<ExamQuestion> {
                        new() { Id = 1, Question = "What is cloud computing?", Options = new[] { "Internet-based computing services", "Local server computing", "Desktop computing" }, CorrectAnswer = "Internet-based computing services" },
                        new() { Id = 2, Question = "Which is a major cloud service provider?", Options = new[] { "Amazon Web Services", "Microsoft Office", "Adobe Photoshop" }, CorrectAnswer = "Amazon Web Services" },
                        new() { Id = 3, Question = "What does SaaS stand for?", Options = new[] { "Software as a Service", "System as a Service", "Security as a Service" }, CorrectAnswer = "Software as a Service" }
                    }
                },
                [4] = new() {
                    CourseId = 4, Title = "Cybersecurity for Organizations - Final Exam",
                    Questions = new List<ExamQuestion> {
                        new() { Id = 1, Question = "What is the first line of defense in cybersecurity?", Options = new[] { "Firewalls", "User awareness training", "Antivirus software" }, CorrectAnswer = "User awareness training" },
                        new() { Id = 2, Question = "What does VPN stand for?", Options = new[] { "Virtual Private Network", "Very Private Network", "Verified Public Network" }, CorrectAnswer = "Virtual Private Network" },
                        new() { Id = 3, Question = "Which is a common type of malware?", Options = new[] { "Ransomware", "Shareware", "Freeware" }, CorrectAnswer = "Ransomware" }
                    }
                },
                [5] = new() {
                    CourseId = 5, Title = "Mobile App Development - Final Exam",
                    Questions = new List<ExamQuestion> {
                        new() { Id = 1, Question = "What is React Native?", Options = new[] { "A framework for building mobile apps", "A database system", "A web browser" }, CorrectAnswer = "A framework for building mobile apps" },
                        new() { Id = 2, Question = "Which language is primarily used for iOS development?", Options = new[] { "Swift", "Java", "Python" }, CorrectAnswer = "Swift" },
                        new() { Id = 3, Question = "What does API stand for?", Options = new[] { "Application Programming Interface", "Advanced Programming Interface", "Automated Programming Interface" }, CorrectAnswer = "Application Programming Interface" }
                    }
                }
            };
            
            return exams.ContainsKey(id) ? exams[id] : exams[1];
        }

        private int CalculateScore(int courseId, Dictionary<int, string> answers)
        {
            if (!answers.Any()) return 0;
            
            var exam = GetExamById(courseId);
            var correctAnswers = exam.Questions.ToDictionary(q => q.Id, q => q.CorrectAnswer);
            
            int correct = answers.Count(a => correctAnswers.ContainsKey(a.Key) && correctAnswers[a.Key] == a.Value);
            return correctAnswers.Count > 0 ? (correct * 100) / correctAnswers.Count : 0;
        }

        private string GenerateCertificateHtml(int id)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Certificate of Completion</title>
                <style>
                    body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; background: #f8f9fa; }}
                    .certificate {{ 
                        border: 8px solid #2c3e50; 
                        padding: 60px; 
                        margin: 20px auto; 
                        background: white;
                        max-width: 800px;
                        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                    }}
                    .title {{ color: #2c3e50; font-size: 42px; margin-bottom: 30px; font-weight: bold; }}
                    .name {{ color: #3498db; font-size: 32px; margin: 30px 0; font-weight: bold; }}
                    .course {{ color: #2c3e50; font-size: 28px; margin: 30px 0; font-style: italic; }}
                    .details {{ font-size: 18px; margin: 15px 0; }}
                    .signature {{ margin-top: 50px; font-size: 16px; color: #7f8c8d; }}
                    @media print {{
                        body {{ background: white; }}
                        .certificate {{ box-shadow: none; }}
                    }}
                </style>
            </head>
            <body>
                <div class='certificate'>
                    <h1 class='title'>🏆 Certificate of Completion 🏆</h1>
                    <p class='details'>This is to certify that</p>
                    <h2 class='name'>John Doe</h2>
                    <p class='details'>has successfully completed the course</p>
                    <h3 class='course'>Database Management Systems</h3>
                    <p class='details'>Completion Date: {DateTime.Now:MMMM dd, yyyy}</p>
                    <p class='details'>Final Score: 95%</p>
                    <div class='signature'>
                        <p>UnganaConnect Platform</p>
                        <p>Digitizing ICT Training for African CSOs</p>
                    </div>
                </div>
                <script>
                    window.onload = function() {{
                        setTimeout(function() {{
                            window.print();
                        }}, 1000);
                    }};
                </script>
            </body>
            </html>";
        }

        private CourseContentViewModel GetCourseById(int id)
        {
            var courses = new Dictionary<int, CourseContentViewModel>
            {
                [1] = new() {
                    Id = 1, Title = "Web Development Fundamentals", Description = "Learn HTML, CSS, and JavaScript to build modern websites.", Instructor = "David Kiptoo", Duration = "8 weeks", Level = "Beginner", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "HTML Basics", Duration = "45 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/UB1O30fR-EE" },
                        new() { Id = 2, Title = "CSS Styling", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/yfoY53QXEnI" },
                        new() { Id = 3, Title = "JavaScript Fundamentals", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/PkZNo7MFNFg" },
                        new() { Id = 4, Title = "Responsive Design", Duration = "50 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/srvUrASNdxk" },
                        new() { Id = 5, Title = "Course Exam", Duration = "30 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [2] = new() {
                    Id = 2, Title = "Database Management Systems", Description = "Master SQL and database design for effective data management.", Instructor = "Dr. Amara Okafor", Duration = "6 weeks", Level = "Intermediate", Progress = 100,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Database Fundamentals", Duration = "50 minutes", IsCompleted = true, VideoUrl = "https://www.youtube.com/embed/HXV3zeQKqGY" },
                        new() { Id = 2, Title = "SQL Queries", Duration = "65 minutes", IsCompleted = true, VideoUrl = "https://www.youtube.com/embed/7S_tz1z_5bA" },
                        new() { Id = 3, Title = "Database Design", Duration = "70 minutes", IsCompleted = true, VideoUrl = "https://www.youtube.com/embed/ztHopE5Wnpc" },
                        new() { Id = 4, Title = "Advanced SQL", Duration = "80 minutes", IsCompleted = true, VideoUrl = "https://www.youtube.com/embed/XqIk2PwP0To" },
                        new() { Id = 5, Title = "Course Exam", Duration = "30 minutes", IsCompleted = true, VideoUrl = "" }
                    }
                },
                [3] = new() {
                    Id = 3, Title = "Cloud Computing Essentials", Description = "Understand cloud services and how to leverage them.", Instructor = "Sarah Mensah", Duration = "5 weeks", Level = "Intermediate", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Cloud Fundamentals", Duration = "40 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/M988_fsOSWo" },
                        new() { Id = 2, Title = "AWS Services", Duration = "55 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/ulprqHHWlng" },
                        new() { Id = 3, Title = "Cloud Security", Duration = "45 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/hiKPPy584Mg" },
                        new() { Id = 4, Title = "Course Exam", Duration = "30 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [4] = new() {
                    Id = 4, Title = "Cybersecurity for Organizations", Description = "Protect your CSO's digital assets with essential cybersecurity practices.", Instructor = "Ahmed Ali", Duration = "7 weeks", Level = "Advanced", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Security Fundamentals", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/inWWhr5tnEA" },
                        new() { Id = 2, Title = "Network Security", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/9GZlVOafYTg" },
                        new() { Id = 3, Title = "Threat Detection", Duration = "65 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/rcDO8km6R6c" },
                        new() { Id = 4, Title = "Incident Response", Duration = "70 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/nvTQInQyNI4" },
                        new() { Id = 5, Title = "Security Policies", Duration = "55 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/WnN6dbos5u8" },
                        new() { Id = 6, Title = "Course Exam", Duration = "45 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [5] = new() {
                    Id = 5, Title = "Mobile App Development", Description = "Build mobile applications to extend your CSO's reach and impact.", Instructor = "Rachel Mwangi", Duration = "10 weeks", Level = "Advanced", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Mobile Development Intro", Duration = "50 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/fis26HvvDII" },
                        new() { Id = 2, Title = "React Native Basics", Duration = "90 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/0-S5a0eXPoc" },
                        new() { Id = 3, Title = "UI/UX Design", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/c9Wg6Cb_YlU" },
                        new() { Id = 4, Title = "Navigation & Routing", Duration = "65 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/nQVCkqvU1uE" },
                        new() { Id = 5, Title = "API Integration", Duration = "80 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/VozPNrt-LfE" },
                        new() { Id = 6, Title = "Database Integration", Duration = "70 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/BosZ3KCDtuQ" },
                        new() { Id = 7, Title = "Testing & Deployment", Duration = "85 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/u-b-Akalj0Q" },
                        new() { Id = 8, Title = "Course Exam", Duration = "45 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [6] = new() {
                    Id = 6, Title = "Data Analytics & Visualization", Description = "Transform data into insights using modern analytics tools.", Instructor = "Dr. Fatima Ndour", Duration = "6 weeks", Level = "Intermediate", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Data Analytics Fundamentals", Duration = "55 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/yZvFH7B6gKI" },
                        new() { Id = 2, Title = "Excel for Data Analysis", Duration = "70 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/Vl0H-qTclOg" },
                        new() { Id = 3, Title = "Python for Data Science", Duration = "95 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/LHBE6Q9XlzI" },
                        new() { Id = 4, Title = "Data Visualization", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/a9UrKTVEeZA" },
                        new() { Id = 5, Title = "Dashboard Creation", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/SPuDz1hrVE8" },
                        new() { Id = 6, Title = "Course Exam", Duration = "40 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [7] = new() {
                    Id = 7, Title = "Digital Project Management", Description = "Master digital tools and methodologies for effective project management.", Instructor = "Moses Kiprotich", Duration = "4 weeks", Level = "Beginner", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Project Management Basics", Duration = "45 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/3qYbVd7DILs" },
                        new() { Id = 2, Title = "Agile Methodology", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/Z9QbYZh1YXY" },
                        new() { Id = 3, Title = "Digital PM Tools", Duration = "50 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/MfJjMbABbes" },
                        new() { Id = 4, Title = "Course Exam", Duration = "25 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [8] = new() {
                    Id = 8, Title = "IT Infrastructure Management", Description = "Learn to set up and maintain IT systems for your organization.", Instructor = "Kofi Asante", Duration = "8 weeks", Level = "Advanced", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Infrastructure Fundamentals", Duration = "65 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/3QhU9jd03a0" },
                        new() { Id = 2, Title = "Server Management", Duration = "80 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/YS5Zh7KExvE" },
                        new() { Id = 3, Title = "Network Configuration", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/qiQR5rTSshw" },
                        new() { Id = 4, Title = "Virtualization", Duration = "70 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/FZR0rG3HKIk" },
                        new() { Id = 5, Title = "Backup & Recovery", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/1uUWqQ_jgBs" },
                        new() { Id = 6, Title = "Monitoring & Maintenance", Duration = "55 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/Lb4IcGF5iTQ" },
                        new() { Id = 7, Title = "Course Exam", Duration = "40 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [9] = new() {
                    Id = 9, Title = "Artificial Intelligence Fundamentals", Description = "Introduction to AI concepts and applications for CSOs.", Instructor = "Dr. Kwame Nkrumah", Duration = "12 weeks", Level = "Advanced", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "AI Introduction", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/ad79nYk2keg" },
                        new() { Id = 2, Title = "Machine Learning Basics", Duration = "90 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/ukzFI9rgwfU" },
                        new() { Id = 3, Title = "Neural Networks", Duration = "85 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/aircAruvnKk" },
                        new() { Id = 4, Title = "Natural Language Processing", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/CMrHM8a3hqw" },
                        new() { Id = 5, Title = "Computer Vision", Duration = "80 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/01sAkU_NvOY" },
                        new() { Id = 6, Title = "AI Ethics", Duration = "50 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/AaAELh2xzMc" },
                        new() { Id = 7, Title = "AI for Social Good", Duration = "65 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/tJVhujWBILs" },
                        new() { Id = 8, Title = "Practical AI Applications", Duration = "70 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/mJeNghZXtMo" },
                        new() { Id = 9, Title = "Course Exam", Duration = "60 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [10] = new() {
                    Id = 10, Title = "Blockchain Technology", Description = "Understanding blockchain and its applications for transparency in CSOs.", Instructor = "Amina Hassan", Duration = "9 weeks", Level = "Advanced", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Blockchain Fundamentals", Duration = "70 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/SSo_EIwHSd4" },
                        new() { Id = 2, Title = "Cryptocurrency Basics", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/1YyAzVmP9xQ" },
                        new() { Id = 3, Title = "Smart Contracts", Duration = "85 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/ZE2HxTmxfrI" },
                        new() { Id = 4, Title = "Decentralized Applications", Duration = "90 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/F50OrwV6Uk8" },
                        new() { Id = 5, Title = "Blockchain for NGOs", Duration = "65 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/hYip_Vuv8J0" },
                        new() { Id = 6, Title = "Implementation Strategies", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/kHybf1aC-jE" },
                        new() { Id = 7, Title = "Course Exam", Duration = "45 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [11] = new() {
                    Id = 11, Title = "IoT for Smart Organizations", Description = "Implementing Internet of Things solutions for efficient CSO operations.", Instructor = "Samuel Ochieng", Duration = "7 weeks", Level = "Intermediate", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "IoT Introduction", Duration = "50 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/LlhmzVL5bm8" },
                        new() { Id = 2, Title = "Sensors and Devices", Duration = "65 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/6mBO2vqLv38" },
                        new() { Id = 3, Title = "IoT Connectivity", Duration = "70 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/h0gWfVCSGQQ" },
                        new() { Id = 4, Title = "Data Collection & Analysis", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/EmSrQCDsMv4" },
                        new() { Id = 5, Title = "IoT Security", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/15VjzMLzWzE" },
                        new() { Id = 6, Title = "Course Exam", Duration = "35 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                },
                [12] = new() {
                    Id = 12, Title = "Digital Transformation Strategy", Description = "Leading digital transformation initiatives in civil society organizations.", Instructor = "Grace Wanjiku", Duration = "6 weeks", Level = "Advanced", Progress = 0,
                    Modules = new List<CourseModule> {
                        new() { Id = 1, Title = "Digital Transformation Overview", Duration = "55 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/BzC9I3Rg1x4" },
                        new() { Id = 2, Title = "Change Management", Duration = "70 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/NP9AIUT9nos" },
                        new() { Id = 3, Title = "Technology Assessment", Duration = "65 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/jQKJ1qgx915" },
                        new() { Id = 4, Title = "Implementation Planning", Duration = "75 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/02EZPxPcFqs" },
                        new() { Id = 5, Title = "Measuring Success", Duration = "60 minutes", IsCompleted = false, VideoUrl = "https://www.youtube.com/embed/7s_NInhk9jM" },
                        new() { Id = 6, Title = "Course Exam", Duration = "40 minutes", IsCompleted = false, VideoUrl = "" }
                    }
                }
            };
            
            return courses.ContainsKey(id) ? courses[id] : courses[1];
        }
    }
}