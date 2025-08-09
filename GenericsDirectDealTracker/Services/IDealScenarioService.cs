using GenericsDirectDealTracker.Models;

namespace GenericsDirectDealTracker.Services
{
    public record CreateDealScenarioRequest();
    public record AllocationUpdate(int DealScenarioDetailId, int? ManufacturerId);

    public interface IDealScenarioService
    {
        Task<DealScenario> CreateScenarioAsync(CreateDealScenarioRequest request);
        Task<DealScenario> GetScenarioWithDetailsAsync(int scenarioId);
        Task<List<Manufacturer>> GetManufacturersAsync();
        Task<List<Product>> GetProductsAsync();
        Task SaveAllocationsAsync(int scenarioId, List<AllocationUpdate> allocations);
        Task<List<DealScenarioDetail>> GetUnassignedScenarioDetailsAsync(int scenarioId);
        Task<List<DealScenarioDetail>> GetManufacturerScenarioDetailsAsync(int scenarioId, int manufacturerId);
    }
}