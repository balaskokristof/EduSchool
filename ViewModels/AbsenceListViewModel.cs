using EduSchool.Models.DataModel;

namespace EduSchool.ViewModels
{
    public class AbsenceListViewModel
    {
        public int CourseID { get; set; }
        public List<Absence> Absences { get; set; }
        public List<AbsenceType> AbsenceTypes { get; set; }

    }
}
