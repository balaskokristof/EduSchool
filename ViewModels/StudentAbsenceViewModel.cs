using EduSchool.Models.DataModel;

namespace EduSchool.ViewModels
{
    public class StudentAbsenceViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<Absence> Absences { get; set; }
    }
}
