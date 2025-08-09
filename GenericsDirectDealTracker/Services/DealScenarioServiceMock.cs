using System.Collections.Concurrent;
using GenDirTest.Components;

namespace GenDirTest.Services;

public class DealScenarioServiceMock : IDealScenarioService
{
    private readonly ConcurrentDictionary<Guid, List<DealScenarioDetail>> _scenarioDetails = new();

    private readonly List<Manufacturer> _manufacturers = new()
    {
        new Manufacturer { Id = 1, Name = "Manufacturer A" },
        new Manufacturer { Id = 2, Name = "Manufacturer B" },
        new Manufacturer { Id = 3, Name = "Manufacturer C" }
    };

    public Task<DealScenario> CreateScenarioAsync(CreateDealScenarioRequest request)
    {
        var id = Guid.NewGuid();
        var details = Enumerable.Range(1, 25)
            .Select(i => new DealScenarioDetail
            {
                Id = Guid.NewGuid(),
                ProductCode = $"P{i:000}",
                ProductName = $"Product {i}",
                Value = Random.Shared.Next(10, 200)
            })
            .ToList();

        _scenarioDetails[id] = details;
        return Task.FromResult(new DealScenario(id, request.Name));
    }

    public Task<IReadOnlyList<DealScenarioDetail>> GetScenarioDetailsAsync(Guid scenarioId)
    {
        if (_scenarioDetails.TryGetValue(scenarioId, out var list))
            return Task.FromResult<IReadOnlyList<DealScenarioDetail>>(list);
        return Task.FromResult<IReadOnlyList<DealScenarioDetail>>(Array.Empty<DealScenarioDetail>());
    }

    public Task<IReadOnlyList<Manufacturer>> GetManufacturersAsync()
        => Task.FromResult<IReadOnlyList<Manufacturer>>(_manufacturers);

    public Task UpdateAllocationsAsync(Guid scenarioId, IReadOnlyList<AllocationUpdate> allocations)
    {
        // No persistence in mock.
        return Task.CompletedTask;
    }
}