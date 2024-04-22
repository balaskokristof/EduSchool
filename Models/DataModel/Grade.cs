namespace EduSchool.Models.DataModel
{
    public class Grade
    {
        public int GradeID { get; set; }
        public int CourseID { get; set; }
        public string StudentID { get; set; }
        public int GradeValue { get; set; }
        public int Weight { get; set; }
        public string Comment { get; set; }
        public DateTime GradeDate { get; set; }
        public Course Course { get; set; }
        public User Student { get; set; }
    }

}
