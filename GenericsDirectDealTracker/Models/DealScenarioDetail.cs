using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenericsDirectDealTracker.Models
{
    public class DealScenarioDetail
    {
        public int Id { get; set; }
        public int DealScenarioId { get; set; }
        [ForeignKey("DealScenarioId")]
        public DealScenario? DealScenario { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public int? ManufacturerId { get; set; }
        [ForeignKey("ManufacturerId")]
        public Manufacturer? Manufacturer { get; set; }
        public decimal? Value { get; set; }
    }
}