using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportService.Api.Models
{
    public class Item
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public DateTime VisitDay { get; set; }

        [Required]
        public bool Payment { get; set; }
    }
}
