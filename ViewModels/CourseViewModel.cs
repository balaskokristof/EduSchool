using System.ComponentModel.DataAnnotations;

namespace EduSchool.ViewModels
{
    public class CourseViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A leírás megadása kötelező.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "A kezdési dátum megadása kötelező.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "A befejezési dátum megadása kötelező.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "A kategória megadása kötelező.")]
        public string Category { get; set; }
        public string InstructorID { get; set; }
    }
}
