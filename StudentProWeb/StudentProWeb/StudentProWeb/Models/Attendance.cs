using System;
using System.Collections.Generic;

namespace StudentProWeb.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public bool? IsAbsent { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }
}
