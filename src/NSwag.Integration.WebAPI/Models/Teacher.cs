using System.ComponentModel;

namespace NSwag.Integration.WebAPI.Models
{
    public class Teacher : Person
    {
        public string Course { get; set; }

        [DefaultValue(SkillLevel.Medium)]
        public SkillLevel MinimumSkillLevel { get; set; }
    }
}