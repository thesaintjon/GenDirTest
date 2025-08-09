using GenericsDirectDealTracker.Models;

namespace GenericsDirectDealTracker.Services
{
    public class DealScenarioServiceMock : IDealScenarioService
    {
        private readonly List<Manufacturer> _manufacturers;
        private readonly List<Product> _products;
        private readonly List<DealScenario> _scenarios;
        private readonly List<DealScenarioDetail> _scenarioDetails;
        private int _nextScenarioId = 1;
        private int _nextDetailId = 1;

        public DealScenarioServiceMock()
        {
            _manufacturers = new List<Manufacturer>
            {
                new Manufacturer { Id = 1, ManufacturerName = "Pfizer" },
                new Manufacturer { Id = 2, ManufacturerName = "Johnson & Johnson" },
                new Manufacturer { Id = 3, ManufacturerName = "Merck" },
                new Manufacturer { Id = 4, ManufacturerName = "Bristol Myers Squibb" }
            };

            _products = new List<Product>
            {
                new Product { Id = 1, MoleculeName = "Atorvastatin", Strength = "20mg", Form = "Tablet", Price = 15.50m },
                new Product { Id = 2, MoleculeName = "Metformin", Strength = "500mg", Form = "Tablet", Price = 8.75m },
                new Product { Id = 3, MoleculeName = "Lisinopril", Strength = "10mg", Form = "Tablet", Price = 12.25m },
                new Product { Id = 4, MoleculeName = "Amlodipine", Strength = "5mg", Form = "Tablet", Price = 9.80m },
                new Product { Id = 5, MoleculeName = "Omeprazole", Strength = "40mg", Form = "Capsule", Price = 18.90m },
                new Product { Id = 6, MoleculeName = "Sertraline", Strength = "50mg", Form = "Tablet", Price = 22.40m }
            };

            _scenarios = new List<DealScenario>();
            _scenarioDetails = new List<DealScenarioDetail>();
        }

        public async Task<DealScenario> CreateScenarioAsync(CreateDealScenarioRequest request)
        {
            await Task.Delay(100); // Simulate async work

            var scenario = new DealScenario
            {
                Id = _nextScenarioId++,
                CreatedDate = DateTime.UtcNow,
                DealScenarioDetails = new List<DealScenarioDetail>()
            };

            // Create scenario details for all products (initially unassigned)
            foreach (var product in _products)
            {
                var detail = new DealScenarioDetail
                {
                    Id = _nextDetailId++,
                    DealScenarioId = scenario.Id,
                    DealScenario = scenario,
                    ProductId = product.Id,
                    Product = product,
                    Value = product.Price,
                    ManufacturerId = null,
                    Manufacturer = null
                };
                scenario.DealScenarioDetails.Add(detail);
                _scenarioDetails.Add(detail);
            }

            _scenarios.Add(scenario);
            return scenario;
        }

        public async Task<DealScenario> GetScenarioWithDetailsAsync(int scenarioId)
        {
            await Task.Delay(50); // Simulate async work
            var scenario = _scenarios.FirstOrDefault(s => s.Id == scenarioId);
            if (scenario == null) throw new ArgumentException("Scenario not found");
            
            // Ensure products are loaded
            foreach (var detail in scenario.DealScenarioDetails)
            {
                detail.Product = _products.FirstOrDefault(p => p.Id == detail.ProductId);
                detail.Manufacturer = detail.ManufacturerId.HasValue 
                    ? _manufacturers.FirstOrDefault(m => m.Id == detail.ManufacturerId.Value)
                    : null;
            }
            
            return scenario;
        }

        public async Task<List<Manufacturer>> GetManufacturersAsync()
        {
            await Task.Delay(30); // Simulate async work
            return _manufacturers.ToList();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            await Task.Delay(30); // Simulate async work
            return _products.ToList();
        }

        public async Task SaveAllocationsAsync(int scenarioId, List<AllocationUpdate> allocations)
        {
            await Task.Delay(200); // Simulate async work
            
            var scenario = _scenarios.FirstOrDefault(s => s.Id == scenarioId);
            if (scenario == null) throw new ArgumentException("Scenario not found");

            foreach (var allocation in allocations)
            {
                var detail = _scenarioDetails.FirstOrDefault(d => d.Id == allocation.DealScenarioDetailId);
                if (detail != null)
                {
                    detail.ManufacturerId = allocation.ManufacturerId;
                    detail.Manufacturer = allocation.ManufacturerId.HasValue
                        ? _manufacturers.FirstOrDefault(m => m.Id == allocation.ManufacturerId.Value)
                        : null;
                }
            }
        }

        public async Task<List<DealScenarioDetail>> GetUnassignedScenarioDetailsAsync(int scenarioId)
        {
            await Task.Delay(30); // Simulate async work
            return _scenarioDetails
                .Where(d => d.DealScenarioId == scenarioId && !d.ManufacturerId.HasValue)
                .ToList();
        }

        public async Task<List<DealScenarioDetail>> GetManufacturerScenarioDetailsAsync(int scenarioId, int manufacturerId)
        {
            await Task.Delay(30); // Simulate async work
            return _scenarioDetails
                .Where(d => d.DealScenarioId == scenarioId && d.ManufacturerId == manufacturerId)
                .ToList();
        }
    }
}