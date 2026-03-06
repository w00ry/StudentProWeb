using System;
using System.Collections.Generic;

namespace StudentProWeb.Models;

public partial class StudentAssignment
{
    public int SubmissionId { get; set; }

    public int? AssignmentId { get; set; }

    public int? StudentId { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public bool? IsCompleted { get; set; }

    public decimal? Score { get; set; }

    public virtual Assignment? Assignment { get; set; }

    public virtual Student? Student { get; set; }
}
