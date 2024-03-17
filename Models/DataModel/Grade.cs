namespace EduSchool.Models.DataModel
{
    public class Grade
    {
        public int GradeID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public string GradeValue { get; set; }
        public int Weight { get; set; }
        public string Comment { get; set; }
        public Course Course { get; set; }
        public User Student { get; set; }
    }

}
