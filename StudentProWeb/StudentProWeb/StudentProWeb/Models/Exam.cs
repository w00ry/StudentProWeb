using System;
using Microsoft.EntityFrameworkCore;

namespace StudentProWeb.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        
        // Soru işareti (?) ekleyerek bu alanların boş (null) gelebileceğini sisteme söylüyoruz!
        public string? ExamType { get; set; }
        public DateTime ExamDate { get; set; }
        public string? Classroom { get; set; }  

        // Sınavın hangi derse ait olduğunu C# tarafında bağlıyoruz
        public virtual Course Course { get; set; }

        
    }
}