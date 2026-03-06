using System;
using System.Collections.Generic;

namespace StudentProWeb.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } 

    public string? Department { get; set; }

    public byte? ClassLevel { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
}
