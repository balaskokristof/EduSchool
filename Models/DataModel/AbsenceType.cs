namespace EduSchool.Models.DataModel
{
    public class AbsenceType
    {
        public int AbsenceTypeID { get; set; }
        public string TypeName { get; set; }
        public List<Absence> Absences { get; set; }
    }

}
