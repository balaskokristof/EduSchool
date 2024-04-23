public class GradeCreateViewModel
{
    public int CourseID { get; set; }
    public List<StudentGradeViewModel> Students { get; set; }
    public string GradeTitle { get; set; }
}

public class StudentGradeViewModel
{
    public string StudentID { get; set; }
    public string StudentName { get; set; }
    public int SelectedGradeValue { get; set; }
    public int Weight { get; set; }
    public string Comment { get; set; }
}
