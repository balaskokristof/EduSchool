namespace EduSchool.Models.DataModel
{
    public class CoursePost
    {
        public int CoursePostID { get; set; }
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        public string AuthorID { get; set; }
        public User Author { get; set; }
    }
}