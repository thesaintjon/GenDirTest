using System.ComponentModel.DataAnnotations.Schema;

namespace GenericsDirectDealTracker.Models
{
    public class DealScenario
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<DealScenarioDetail> DealScenarioDetails { get; set; } = new List<DealScenarioDetail>();
    }
}
