namespace NSwag.Integration.WebAPI.Models
{
    public class Teacher : Person
    {
        public string Course { get; set; }

        public SkillLevel MinimumSkillLevel { get; set; }
    }
}