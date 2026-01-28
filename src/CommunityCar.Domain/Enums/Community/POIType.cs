namespace CommunityCar.Domain.Enums.Community;

public enum POIType
{
    // Service locations
    AutoRepairShop = 1,
    CarDealership = 2,
    GasStation = 3,
    ChargingStation = 4,
    CarWash = 5,
    TireShop = 6,
    AutoPartsStore = 7,
    
    // Community locations
    MeetupPoint = 10,
    CarShow = 11,
    RaceTrack = 12,
    DrivingRoute = 13,
    ParkingSpot = 14,
    
    // Events
    CarEvent = 20,
    Workshop = 21,
    TrackDay = 22,
    CarMeet = 23,
    
    // Emergency and safety
    EmergencyService = 30,
    TowingService = 31,
    
    // Other
    Landmark = 40,
    RestArea = 41,
    Scenic = 42
}