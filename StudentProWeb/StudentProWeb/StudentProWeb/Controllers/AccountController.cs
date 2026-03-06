using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StudentProWeb.Models;
using System.Linq;
using System;
using System.Text;

namespace StudentProWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly StudentProDbContext _context;

        public AccountController(StudentProDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // --- KRİTİK DÜZELTME: Şifreyi Kayıttaki Gibi Hash'liyoruz ---
            // Eğer kayıt olurken şifreyi Base64 yaptıysan, kontrol ederken de yapmalısın.
            string hashedInputPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            // 1. Kullanıcıyı bulmaya çalış (Hash'lenmiş şifreyle eşleştir)
            var user = _context.Students.FirstOrDefault(s => s.Email == email && s.PasswordHash == hashedInputPassword);

            // 2. GÜVENLİK KAPISI
            if (user != null)
            {
                // Giriş Başarılı
                HttpContext.Session.SetInt32("LoggedStudentId", user.StudentId);
                HttpContext.Session.SetString("LoggedStudentName", user.FirstName);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Giriş Başarısız
                ViewBag.ErrorMessage = "E-posta veya şifre hatalı!";
                return View();
            }
        }

        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string password)
        {
            // 1. E-POSTA KONTROLÜ
            if (!email.EndsWith(".edu") && !email.EndsWith(".edu.tr"))
            {
                ViewBag.RegisterError = "Sadece okul e-postasıyla (.edu veya .edu.tr) kayıt olabilirsiniz!";
                return View("Login");
            }

            // 2. MÜKERRER KAYIT KONTROLÜ
            var existingUser = _context.Students.FirstOrDefault(s => s.Email == email);
            if (existingUser != null)
            {
                ViewBag.RegisterError = "Bu e-posta adresi zaten sisteme kayıtlı!";
                return View("Login");
            }

            // 3. ŞİFREYİ HASH'LEME
            string hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            // 4. YENİ ÖĞRENCİ OLUŞTURMA
            var newStudent = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = hashedPassword
            };

            _context.Students.Add(newStudent);
            _context.SaveChanges();

            // 5. BAŞARI MESAJI
            ViewBag.SuccessMessage = "Kayıt başarıyla tamamlandı! Şimdi giriş yapabilirsiniz.";
            return View("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}