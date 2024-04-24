using EduSchool.Models.DataModel;
using System.ComponentModel.DataAnnotations;

namespace EduSchool.ViewModels
{
    public class RecordAbsenceViewModel
    {
        [Required(ErrorMessage = "Válasszon tanulót.")]
        [Display(Name = "Tanuló")]
        public string StudentID { get; set; }

        [Required(ErrorMessage = "A kurzus azonosítóját meg kell adni.")]
        public int CourseID { get; set; }

        [Required(ErrorMessage = "A mulasztás kezdő dátumát meg kell adni.")]
        [Display(Name = "Mulasztás dátuma")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "A mulasztás vége dátumát meg kell adni.")]
        [Display(Name = "Mulasztás dátuma")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Válasszon mulasztás típust.")]
        [Display(Name = "Mulasztás típusa")]
        public int AbsenceTypeID { get; set; }
        public List<AbsenceType> AbsenceTypes { get; set; }
        public List<User> Students { get; set; }
    }
}
