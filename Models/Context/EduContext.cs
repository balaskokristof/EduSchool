namespace EduSchool.Models.Context
{
    using EduSchool.Models.DataModel;
    using Microsoft.EntityFrameworkCore;

    public class EduContext : DbContext
    {
        public EduContext()
        {
        }

        public EduContext(DbContextOptions<EduContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Absence> Absences { get; set; }
        public DbSet<AbsenceType> AbsenceTypes { get; set; }
        public DbSet<CoursePost> CoursePost { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CoursesTaught)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorID);

            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.CourseID, e.StudentID });

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseID);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentID);

            modelBuilder.Entity<Grade>()
                .HasKey(g => g.GradeID);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CourseID);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(u => u.Grades)
                .HasForeignKey(g => g.StudentID);

            modelBuilder.Entity<Absence>()
                .HasKey(a => a.AbsenceID);

            modelBuilder.Entity<Absence>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Absences)
                .HasForeignKey(a => a.CourseID);

            modelBuilder.Entity<Absence>()
                .HasOne(a => a.Student)
                .WithMany(u => u.Absences)
                .HasForeignKey(a => a.StudentID);

            modelBuilder.Entity<Absence>()
                .HasOne(a => a.AbsenceType)
                .WithMany()
                .HasForeignKey(a => a.AbsenceTypeID);

            modelBuilder.Entity<CoursePost>()
            .HasKey(cp => cp.CoursePostID);

            modelBuilder.Entity<CoursePost>()
                .HasOne(cp => cp.Author)
                .WithMany()
                .HasForeignKey(cp => cp.AuthorID);
        }

    }

}
