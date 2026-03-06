using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Session için eklendi
using Microsoft.EntityFrameworkCore;
using StudentProWeb.Models;
using System.Linq;
using System;
using System.Collections.Generic; // Dictionary (Sözlük) yapýsý için eklendi

namespace StudentProWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentProDbContext _context;

        public HomeController(StudentProDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var totalCourses = _context.Grades.Count(g => g.StudentId == loggedInUserId.Value);
            var pendingAssignments = _context.Assignments.Count(a => a.StudentId == loggedInUserId.Value);
            var upcomingExams = _context.Exams.Count(e => e.StudentId == loggedInUserId.Value && e.ExamDate >= DateTime.Now);

            ViewBag.TotalCourses = totalCourses;
            ViewBag.PendingAssignments = pendingAssignments;
            ViewBag.UpcomingExams = upcomingExams;

            return View();
        }

        [HttpGet]
        public IActionResult AddCourse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveCourse(string CourseName, string CourseDay, string ExamWeight, decimal? Midterm1Score, decimal? Midterm2Score, decimal? FinalScore, int AbsenceLimit, int CurrentAbsence)
        {
            var newCourse = new Course
            {
                CourseName = CourseName,
                CourseCode = CourseName.Length >= 3 ? CourseName.Substring(0, 3).ToUpper() + new Random().Next(100, 999).ToString() : "DRS" + new Random().Next(100, 999).ToString()
            };
            _context.Courses.Add(newCourse);
            _context.SaveChanges();

            var weights = ExamWeight.Split('-');
            decimal wVize = decimal.Parse(weights[0]) / 100m;
            decimal wFinal = decimal.Parse(weights[1]) / 100m;

            string durum = "Notlar Bekleniyor";

            if (Midterm1Score.HasValue && FinalScore.HasValue)
            {
                decimal vizeOrtalamasi = Midterm2Score.HasValue ? (Midterm1Score.Value + Midterm2Score.Value) / 2m : Midterm1Score.Value;
                decimal genelOrtalama = (vizeOrtalamasi * wVize) + (FinalScore.Value * wFinal);
                durum = (genelOrtalama >= 50 && FinalScore.Value >= 50) ? "Baþarýlý" : "Baþarýsýz";
            }

            var newGrade = new Grade
            {
                CourseId = newCourse.CourseId,
                StudentId = HttpContext.Session.GetInt32("LoggedStudentId").Value,
                ExamWeight = ExamWeight,
                Midterm1Score = Midterm1Score,
                Midterm2Score = Midterm2Score,
                FinalScore = FinalScore,
                AbsenceLimit = AbsenceLimit,
                CurrentAbsence = CurrentAbsence,
                Status = durum
            };

            _context.Grades.Add(newGrade);
            _context.SaveChanges();

            return RedirectToAction("Course", "Home");
        }

        [HttpPost]
        public IActionResult UpdateCourse(int GradeId, int CourseId, string CourseName, string ExamWeight, decimal? Midterm1Score, decimal? Midterm2Score, decimal? FinalScore, int AbsenceLimit, int CurrentAbsence)
        {
            var grade = _context.Grades.Include(g => g.Course).FirstOrDefault(g => g.GradeId == GradeId);
            if (grade != null)
            {
                grade.Course.CourseName = CourseName;
                grade.ExamWeight = ExamWeight;
                grade.Midterm1Score = Midterm1Score;
                grade.Midterm2Score = Midterm2Score;
                grade.FinalScore = FinalScore;
                grade.AbsenceLimit = AbsenceLimit;
                grade.CurrentAbsence = CurrentAbsence;

                var weights = ExamWeight.Split('-');
                decimal wVize = decimal.Parse(weights[0]) / 100m;
                decimal wFinal = decimal.Parse(weights[1]) / 100m;

                string durum = "Notlar Bekleniyor";
                if (Midterm1Score.HasValue && FinalScore.HasValue)
                {
                    decimal vizeOrtalamasi = Midterm2Score.HasValue ? (Midterm1Score.Value + Midterm2Score.Value) / 2m : Midterm1Score.Value;
                    decimal genelOrtalama = (vizeOrtalamasi * wVize) + (FinalScore.Value * wFinal);
                    durum = (genelOrtalama >= 50 && FinalScore.Value >= 50) ? "Baþarýlý" : "Baþarýsýz";
                }

                grade.Status = durum;
                _context.SaveChanges();
            }

            return RedirectToAction("Course", "Home");
        }

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            var gradeInfo = _context.Grades
                .Include(g => g.Course)
                .FirstOrDefault(g => g.GradeId == id);

            if (gradeInfo == null) return NotFound("Güncellenecek ders bulunamadý.");

            return View(gradeInfo);
        }

        [HttpGet]
        public IActionResult Assignments()
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var userAssignments = _context.Assignments
                .Include(a => a.Course)
                .Where(a => a.StudentId == loggedInUserId.Value)
                .OrderBy(a => a.DueDate)
                .ToList();

            return View(userAssignments);
        }

        [HttpGet]
        public IActionResult AddAssignment()
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var studentCourses = _context.Grades
                .Where(g => g.StudentId == loggedInUserId.Value)
                .Select(g => g.Course)
                .ToList();

            ViewBag.Courses = studentCourses;
            return View();
        }

        [HttpPost]
        public IActionResult SaveAssignment(int CourseId, string AssignmentTitle, DateTime DueDate)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var newAssignment = new Assignment
            {
                StudentId = loggedInUserId.Value,
                CourseId = CourseId,
                AssignmentTitle = AssignmentTitle,
                DueDate = DueDate,
                IsCompleted = false
            };

            _context.Assignments.Add(newAssignment);
            _context.SaveChanges();

            return RedirectToAction("Assignments", "Home");
        }

        [HttpGet]
        public IActionResult CompleteAssignment(int id)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == id && a.StudentId == loggedInUserId.Value);
            if (assignment != null)
            {
                assignment.IsCompleted = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Assignments", "Home");
        }

        [HttpGet]
        public IActionResult ReactivateAssignment(int id)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == id && a.StudentId == loggedInUserId.Value);
            if (assignment != null)
            {
                assignment.IsCompleted = false;
                _context.SaveChanges();
            }
            return RedirectToAction("Assignments", "Home");
        }

        [HttpGet]
        public IActionResult Exams()
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var userExams = _context.Exams
                .Include(e => e.Course)
                .Where(e => e.StudentId == loggedInUserId.Value)
                .OrderBy(e => e.ExamDate)
                .ToList();

            return View(userExams);
        }

        [HttpGet]
        public IActionResult AddExam()
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var studentCourses = _context.Grades
                .Where(g => g.StudentId == loggedInUserId.Value)
                .Select(g => g.Course)
                .ToList();

            ViewBag.Courses = studentCourses;
            return View();
        }

        [HttpPost]
        public IActionResult SaveExam(int CourseId, string ExamType, DateTime ExamDate, string Classroom)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var newExam = new Exam
            {
                StudentId = loggedInUserId.Value,
                CourseId = CourseId,
                ExamType = ExamType,
                ExamDate = ExamDate,
                Classroom = Classroom
            };

            _context.Exams.Add(newExam);
            _context.SaveChanges();

            return RedirectToAction("Exams", "Home");
        }

        [HttpGet]
        public IActionResult EditExam(int id)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var exam = _context.Exams.FirstOrDefault(e => e.ExamId == id && e.StudentId == loggedInUserId.Value);
            if (exam == null) return RedirectToAction("Exams", "Home");

            ViewBag.Courses = _context.Grades
                .Where(g => g.StudentId == loggedInUserId.Value)
                .Select(g => g.Course)
                .ToList();

            return View(exam);
        }

        [HttpPost]
        public IActionResult UpdateExam(int ExamId, int CourseId, string ExamType, DateTime ExamDate, string Classroom)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var exam = _context.Exams.FirstOrDefault(e => e.ExamId == ExamId && e.StudentId == loggedInUserId.Value);
            if (exam != null)
            {
                exam.CourseId = CourseId;
                exam.ExamType = ExamType;
                exam.ExamDate = ExamDate;
                exam.Classroom = Classroom;
                _context.SaveChanges();
            }

            return RedirectToAction("Exams", "Home");
        }

        // --- ÝÞTE YENÝLENEN, SIFIR HATALI COURSE METODU ---
        [HttpGet]
        public IActionResult Course()
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            var student = _context.Students
                .Include(s => s.Grades)
                    .ThenInclude(g => g.Course)
                .FirstOrDefault(s => s.StudentId == loggedInUserId.Value);

            var averages = new Dictionary<int, string>();

            if (student != null && student.Grades != null)
            {
                foreach (var grade in student.Grades)
                {
                    if (grade.FinalScore.HasValue && grade.Midterm1Score.HasValue)
                    {
                        // 1. Veritabanýndaki "ExamWeight" (Örn: 40-60) verisini alýyoruz
                        decimal wVize = 0.4m; // Sistemde yoksa varsayýlan
                        decimal wFinal = 0.6m; // Sistemde yoksa varsayýlan

                        if (!string.IsNullOrEmpty(grade.ExamWeight) && grade.ExamWeight.Contains("-"))
                        {
                            var weights = grade.ExamWeight.Split('-');
                            if (decimal.TryParse(weights[0], out decimal pVize) && decimal.TryParse(weights[1], out decimal pFinal))
                            {
                                wVize = pVize / 100m;
                                wFinal = pFinal / 100m;
                            }
                        }

                        // 2. Senin kendi UpdateCourse'da kullandýðýn muazzam hesaplama mantýðýný çalýþtýrýyoruz
                        decimal v1 = grade.Midterm1Score.Value;
                        decimal v2 = grade.Midterm2Score ?? 0;
                        decimal vizeOrtalamasi = grade.Midterm2Score.HasValue ? (v1 + v2) / 2m : v1;

                        decimal avg = (vizeOrtalamasi * wVize) + (grade.FinalScore.Value * wFinal);

                        averages.Add(grade.GradeId, avg.ToString("0.0"));
                    }
                    else
                    {
                        averages.Add(grade.GradeId, "Final Bekleniyor");
                    }
                }
            }

            // Tüm hesaplamalarý C# bitirdi, HTML'e sadece sonucu yolluyor!
            ViewBag.Averages = averages;

            return View(student);
        }

        [HttpGet]
        public IActionResult DeleteItem(string entityType, int id)
        {
            int? loggedInUserId = HttpContext.Session.GetInt32("LoggedStudentId");
            if (loggedInUserId == null) return RedirectToAction("Login", "Account");

            string redirectPage = "Index";

            if (entityType == "Course")
            {
                var grade = _context.Grades.FirstOrDefault(g => g.GradeId == id && g.StudentId == loggedInUserId.Value);
                if (grade != null) _context.Grades.Remove(grade);
                redirectPage = "Course";
            }
            else if (entityType == "Exam")
            {
                var exam = _context.Exams.FirstOrDefault(e => e.ExamId == id && e.StudentId == loggedInUserId.Value);
                if (exam != null) _context.Exams.Remove(exam);
                redirectPage = "Exams";
            }
            else if (entityType == "Assignment")
            {
                var assignment = _context.Assignments.FirstOrDefault(a => a.AssignmentId == id && a.StudentId == loggedInUserId.Value);
                if (assignment != null) _context.Assignments.Remove(assignment);
                redirectPage = "Assignments";
            }

            _context.SaveChanges();
            return RedirectToAction(redirectPage);
        }
    }
}