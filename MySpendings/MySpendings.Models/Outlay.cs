using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MySpendings.Models
{
    public class Outlay
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Range(0, 1000000)]
        public float Cost { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Date")]
        public DateTimeOffset CreatedDate { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }
    }
}
