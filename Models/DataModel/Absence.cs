namespace EduSchool.Models.DataModel
{
    public class Absence
    {
        public int AbsenceID { get; set; }
        public string StudentID { get; set; }
        public int CourseID { get; set; }
        public DateTime Date { get; set; }
        public int AbsenceTypeID { get; set; }
        public Course Course { get; set; }
        public User Student { get; set; }
        public AbsenceType AbsenceType { get; set; }
    }
}
