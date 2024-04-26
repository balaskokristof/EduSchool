namespace EduSchool.ViewModels
{
    public class AllCourseGradesViewModel1
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public List<StudentGradeViewModel1> Students { get; set; }
    }

    public class StudentGradeViewModel1
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public List<GradeViewModel1> Grades { get; set; }
    }

    public class GradeViewModel1
    {
        public int GradeID { get; set; }
        public int GradeValue { get; set; }
        public string GradeTitle { get; set; }
        public int Weight { get; set; }
        public string Comment { get; set; }
        public DateTime GradeDate { get; set; }
    }

}
