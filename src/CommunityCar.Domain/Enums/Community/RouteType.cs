namespace CommunityCar.Domain.Enums.Community;

public enum RouteType
{
    Shortest = 0,
    Fastest = 1,
    Cheapest = 2,
    EcoFriendly = 3,
    Scenic = 4,
    AvoidTolls = 5,
    AvoidHighways = 6,
    Performance = 7,
    OffRoad = 8,
    Touring = 9,
    Commute = 10,
    Track = 11,
    Rally = 12,
    Cruise = 13
}

public enum RouteRequestStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}

public enum TrafficConditionType
{
    Accident = 0,
    Construction = 1,
    RoadClosure = 2,
    HeavyTraffic = 3,
    WeatherCondition = 4,
    Event = 5,
    Hazard = 6,
    Police = 7,
    SpeedTrap = 8
}

public enum TrafficSeverity
{
    Low = 0,
    Moderate = 1,
    High = 2,
    Severe = 3
}

public enum CommunityReportType
{
    Accident = 0,
    Police = 1,
    Hazard = 2,
    Construction = 3,
    RoadClosure = 4,
    SpeedTrap = 5,
    Fuel = 6,
    Parking = 7,
    Weather = 8,
    Other = 9
}

public enum CommunityReportStatus
{
    Pending = 0,
    Active = 1,
    Resolved = 2,
    Disputed = 3,
    Expired = 4,
    Removed = 5
}

public enum ParkingType
{
    Street = 0,
    Garage = 1,
    Lot = 2,
    Private = 3,
    Valet = 4,
    Residential = 5
}

public enum ParkingReservationStatus
{
    Pending = 0,
    Confirmed = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4,
    NoShow = 5
}

public enum TripPlanStatus
{
    Draft = 0,
    Planned = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4
}

public enum TripStopType
{
    Waypoint = 0,
    Destination = 1,
    FuelStop = 2,
    RestStop = 3,
    Attraction = 4,
    Restaurant = 5,
    Hotel = 6,
    Parking = 7,
    EVCharging = 8
}

public enum TripExpenseType
{
    Fuel = 0,
    Tolls = 1,
    Parking = 2,
    Food = 3,
    Accommodation = 4,
    Attraction = 5,
    Maintenance = 6,
    Other = 7
}