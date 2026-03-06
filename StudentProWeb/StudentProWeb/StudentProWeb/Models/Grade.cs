using System;
using System.Collections.Generic;

namespace StudentProWeb.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }
    public int AbsenceLimit { get; set; }
    public int CurrentAbsence { get; set; }
    public string ExamWeight { get; set; } = "40-60";
    public decimal? Midterm1Score { get; set; }

    public decimal? FinalScore { get; set; }

    public string? Status { get; set; }

    public decimal? Midterm2Score { get; set; }

    public byte? GradingSystemType { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }

}
