using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.ObjectModel;

namespace GenDirTest.Components;

public class DealScenarioAllocatorBase : ComponentBase
{
    protected const string UnassignedZoneId = "UNASSIGNED";

    [Inject] public IDealScenarioService ScenarioService { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;

    protected bool IsBusy { get; set; }
    protected bool IsSaving { get; set; }
    protected bool ScenarioCreated { get; set; }
    protected string? ErrorMessage { get; set; }
    protected Guid? DealScenarioId { get; set; }

    protected ObservableCollection<DealScenarioDetail> Unassigned { get; set; } = new();
    protected List<ManufacturerAllocation> ManufacturerAllocations { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var manufacturers = await ScenarioService.GetManufacturersAsync();
        ManufacturerAllocations = manufacturers
            .Select(m => new ManufacturerAllocation(m))
            .ToList();
    }

    protected async Task CreateScenario()
    {
        try
        {
            ErrorMessage = null;
            IsBusy = true;
            StateHasChanged();

            var scenario = await ScenarioService.CreateScenarioAsync(new CreateDealScenarioRequest
            {
                Name = $"Scenario {DateTime.Now:yyyyMMdd-HHmmss}"
            });

            DealScenarioId = scenario.Id;
            ScenarioCreated = true;

            var details = await ScenarioService.GetScenarioDetailsAsync(scenario.Id);
            Unassigned.Clear();
            foreach (var d in details)
                Unassigned.Add(d);

            RecalculateTotals();
            Snackbar.Add("Scenario created and products loaded.", Severity.Success);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to create scenario: " + ex.Message;
            Snackbar.Add(ErrorMessage, Severity.Error);
        }
        finally
        {
            IsBusy = false;
            StateHasChanged();
        }
    }

    protected async Task SaveAllocations()
    {
        if (!ScenarioCreated || DealScenarioId is null) return;

        try
        {
            IsSaving = true;
            ErrorMessage = null;

            var assignments = ManufacturerAllocations
                .SelectMany(a => a.Items.Select(i => new AllocationUpdate
                {
                    DealScenarioDetailId = i.Id,
                    ManufacturerId = a.Manufacturer.Id
                }))
                .ToList();

            var unassigned = Unassigned.Select(i => new AllocationUpdate
            {
                DealScenarioDetailId = i.Id,
                ManufacturerId = null
            });

            assignments.AddRange(unassigned);

            await ScenarioService.UpdateAllocationsAsync(DealScenarioId.Value, assignments);
            Snackbar.Add("Allocations saved.", Severity.Success);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to save allocations: " + ex.Message;
            Snackbar.Add(ErrorMessage, Severity.Error);
        }
        finally
        {
            IsSaving = false;
        }
    }

    protected void ResetAll()
    {
        foreach (var alloc in ManufacturerAllocations)
        {
            foreach (var item in alloc.Items.ToList())
            {
                alloc.Items.Remove(item);
                Unassigned.Add(item);
            }
        }
        RecalculateTotals();
    }

    protected void OnItemDropped(MudItemDropInfo<DealScenarioDetail> dropInfo)
    {
        var item = dropInfo.Item;
        var sourceZone = dropInfo.SourceZoneIdentifier;
        var targetZone = dropInfo.TargetZoneIdentifier;

        if (sourceZone == targetZone)
            return;

        RemoveFromZone(item, sourceZone);
        AddToZone(item, targetZone);
        RecalculateTotals();
        StateHasChanged();
    }

    private void RemoveFromZone(DealScenarioDetail item, string zoneId)
    {
        if (zoneId == UnassignedZoneId)
        {
            Unassigned.Remove(item);
            return;
        }

        var sourceAlloc = ManufacturerAllocations
            .FirstOrDefault(a => a.Manufacturer.Id.ToString() == zoneId);
        sourceAlloc?.Items.Remove(item);
    }

    private void AddToZone(DealScenarioDetail item, string zoneId)
    {
        if (zoneId == UnassignedZoneId)
        {
            if (!Unassigned.Contains(item))
                Unassigned.Add(item);
            return;
        }

        var targetAlloc = ManufacturerAllocations
            .FirstOrDefault(a => a.Manufacturer.Id.ToString() == zoneId);
        targetAlloc?.Items.Add(item);
    }

    private void RecalculateTotals()
    {
        foreach (var alloc in ManufacturerAllocations)
            alloc.RecalculateTotal();
    }

    protected string FormatCurrency(decimal value) => value.ToString("C");
}

public class ManufacturerAllocation
{
    public Manufacturer Manufacturer { get; }
    public ObservableCollection<DealScenarioDetail> Items { get; } = new();
    public decimal TotalValue { get; private set; }

    public ManufacturerAllocation(Manufacturer manufacturer)
    {
        Manufacturer = manufacturer;
        Items.CollectionChanged += (_, __) => RecalculateTotal();
    }

    public void RecalculateTotal() => TotalValue = Items.Sum(i => i.Value);
}

// Domain records (merge/remove if duplicates exist)
public record DealScenario(Guid Id, string Name);

public record DealScenarioDetail
{
    public Guid Id { get; init; }
    public string ProductCode { get; init; } = "";
    public string ProductName { get; init; } = "";
    public decimal Value { get; set; }
}

public record Manufacturer
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
}

public record CreateDealScenarioRequest
{
    public string Name { get; init; } = "";
}

public record AllocationUpdate
{
    public Guid DealScenarioDetailId { get; init; }
    public int? ManufacturerId { get; init; }
}

public interface IDealScenarioService
{
    Task<DealScenario> CreateScenarioAsync(CreateDealScenarioRequest request);
    Task<IReadOnlyList<DealScenarioDetail>> GetScenarioDetailsAsync(Guid scenarioId);
    Task<IReadOnlyList<Manufacturer>> GetManufacturersAsync();
    Task UpdateAllocationsAsync(Guid scenarioId, IReadOnlyList<AllocationUpdate> allocations);
}