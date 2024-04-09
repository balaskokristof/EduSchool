namespace EduSchool.Models.DataModel
{
    public enum UserType
    {
        Instructor,
        Student
    }

    public class User
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserType UserType { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public List<Course> CoursesTaught { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<Grade> Grades { get; set; }
        public List<Absence> Absences { get; set; }
    }
}
