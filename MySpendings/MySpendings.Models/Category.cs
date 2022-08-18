using System.ComponentModel.DataAnnotations;

namespace MySpendings.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(1, 10, ErrorMessage = "Priority must be in range between 1 and 10.")]
        public int Priority { get; set; }
    }
}
