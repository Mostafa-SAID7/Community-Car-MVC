using CommunityCar.Domain.Base;
using CommunityCar.Domain.Enums;

namespace CommunityCar.Domain.Entities.Community.Maps;

public class TripPlan : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PlannedStartTime { get; set; }
    public DateTime? PlannedEndTime { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public double TotalDistanceKm { get; set; }
    public int TotalEstimatedTimeMinutes { get; set; }
    public decimal TotalEstimatedCost { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal TotalTollCost { get; set; }
    public decimal TotalParkingCost { get; set; }
    public TripPlanStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public bool IsPublic { get; set; }
    public int ShareCount { get; set; }
    
    // Navigation properties
    public List<TripStop> TripStops { get; set; } = new();
    public List<TripExpense> TripExpenses { get; set; } = new();
}

public class TripStop : BaseEntity
{
    public Guid TripPlanId { get; set; }
    public int Order { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TripStopType Type { get; set; }
    public DateTime? PlannedArrival { get; set; }
    public DateTime? PlannedDeparture { get; set; }
    public DateTime? ActualArrival { get; set; }
    public DateTime? ActualDeparture { get; set; }
    public int EstimatedStayMinutes { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public Guid? PointOfInterestId { get; set; }
    public Guid? ParkingLocationId { get; set; }
    
    // Navigation property
    public TripPlan TripPlan { get; set; } = null!;
}

public class TripExpense : BaseEntity
{
    public Guid TripPlanId { get; set; }
    public Guid? TripStopId { get; set; }
    public TripExpenseType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime ExpenseDate { get; set; }
    public string? Receipt { get; set; } // URL to receipt image
    public bool IsEstimated { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public TripPlan TripPlan { get; set; } = null!;
    public TripStop? TripStop { get; set; }
}