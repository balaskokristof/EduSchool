namespace EduSchool.Models.DataModel
{
    public class Enrollment
    {
        public int CourseID { get; set; }
        public string StudentID { get; set; }
        public Course Course { get; set; }
        public User Student { get; set; }
    }
}
