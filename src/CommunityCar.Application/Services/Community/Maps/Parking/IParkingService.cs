using CommunityCar.Domain.Enums.Community;
using CommunityCar.Application.Features.Community.Maps.ViewModels;

namespace CommunityCar.Application.Services.Maps.Parking;

public interface IParkingService
{
    Task<List<ParkingLocationVM>> GetNearbyParkingAsync(double latitude, double longitude, double radiusKm = 2);
    Task<List<ParkingLocationVM>> SearchParkingAsync(ParkingSearchVM request);
    Task<ParkingLocationVM?> GetParkingLocationAsync(Guid id);
    Task<ParkingReservationVM> ReserveParkingAsync(ReserveParkingVM request);
    Task<bool> CancelReservationAsync(Guid reservationId, Guid userId);
    Task<List<ParkingLocationVM>> GetEVChargingStationsAsync(double latitude, double longitude, double radiusKm = 10);
    Task<ParkingCostEstimateVM> EstimateParkingCostAsync(Guid parkingLocationId, DateTime startTime, DateTime endTime);
    Task UpdateParkingAvailabilityAsync();
}




