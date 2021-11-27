using System.ComponentModel.DataAnnotations;

namespace EComm.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; } = String.Empty;

        [Required]
        [Range(1.0, 500.0)]
        public decimal? UnitPrice { get; set; }
        
        public string? Package { get; set; }
        public bool IsDiscontinued { get; set; }

        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
