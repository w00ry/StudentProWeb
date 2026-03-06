using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentProWeb.Models;

public partial class StudentProDbContext : DbContext
{
    public StudentProDbContext()
    {
    }

    public StudentProDbContext(DbContextOptions<StudentProDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentAssignment> StudentAssignments { get; set; }
    public virtual DbSet<Exam> Exams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=StudentProDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__32499E57B3AE28B2");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.AssignmentTitle).HasMaxLength(150);

            entity.HasOne(d => d.Course).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Assignmen__Cours__44FF419A");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263C42DABC90");

            entity.ToTable("Attendance");

            entity.Property(e => e.AttendanceId).HasColumnName("AttendanceID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.IsAbsent).HasDefaultValue(true);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Course).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Attendanc__Cours__4D94879B");

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Attendanc__Stude__4CA06362");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71879CB06ACC");

            entity.HasIndex(e => e.CourseCode, "UQ__Courses__FC00E0005EE16689").IsUnique();

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.CourseCode).HasMaxLength(20);
            entity.Property(e => e.CourseName).HasMaxLength(100);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A3780FFA98B");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.FinalScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.GradingSystemType).HasDefaultValue((byte)1);
            entity.Property(e => e.Midterm1Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Midterm2Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Devam Ediyor");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Course).WithMany(p => p.Grades)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Grades__CourseID__412EB0B6");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Grades__StudentI__403A8C7D");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A796C587681");

            entity.HasIndex(e => e.Email, "UQ__Students__A9D105346D1C61A3").IsUnique();

            

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ClassLevel).HasDefaultValue((byte)2);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasDefaultValue("Bilgisayar Programcılığı");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            
        });

        modelBuilder.Entity<StudentAssignment>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__StudentA__449EE1053F77A562");

            entity.Property(e => e.SubmissionId).HasColumnName("SubmissionID");
            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.SubmissionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Assignment).WithMany(p => p.StudentAssignments)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("FK__StudentAs__Assig__47DBAE45");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentAssignments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__StudentAs__Stude__48CFD27E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
