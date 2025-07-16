using System.ComponentModel.DataAnnotations;

namespace PlatformService.Entities
{
    public class Platform
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Publisher { get; set; } 
        
        [Required]
        public string Cost { get; set; }
    }
}
