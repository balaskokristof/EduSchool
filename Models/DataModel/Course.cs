using System.Diagnostics;

namespace EduSchool.Models.DataModel
{
    public class Course
    {
        public int CourseID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Category { get; set; }
        public string InstructorID { get; set; }
        public string InstructorName { get; set; }
        public User Instructor { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<Grade> Grades { get; set; }
        public List<Absence> Absences { get; set; }
    }
}
