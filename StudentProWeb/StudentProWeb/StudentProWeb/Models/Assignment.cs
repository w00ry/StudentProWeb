using System;
using System.Collections.Generic;

namespace StudentProWeb.Models;

public partial class Assignment
{
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string AssignmentTitle { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }

    // Ödevin hangi derse ait olduğunu C# tarafında da bağlıyoruz
    public virtual Course Course { get; set; }

    public virtual ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
}
