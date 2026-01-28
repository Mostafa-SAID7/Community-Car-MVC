using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Services.Community;

public partial class InteractionService
{
    #region Ratings

    public async Task<bool> AddRatingAsync(Guid userId, int entityId, string entityTypeStr, int value, string? review = null)
    {
        if (Enum.TryParse<EntityType>(entityTypeStr, true, out var entityType))
        {
             // Placeholder logic for int -> Guid if needed, or just assume Guid.Empty if legacy system
             Guid guidEntityId = Guid.Empty; 

             var existing = await _unitOfWork.Ratings.GetUserRatingAsync(guidEntityId, entityType, userId);
             if (existing != null)
             {
                 existing.UpdateValue(value, review);
             }
             else
             {
                 var rating = new Rating(guidEntityId, entityType, userId, value, review);
                 await _unitOfWork.Ratings.AddAsync(rating);
             }
             
             await _unitOfWork.SaveChangesAsync();
             return true;
        }
        return false;
    }

    public async Task<List<Rating>> GetRatingsAsync(int entityId, string entityTypeStr)
    {
         if (Enum.TryParse<EntityType>(entityTypeStr, true, out var entityType))
        {
             // Placeholder
             return new List<Rating>(); 
        }
        return new List<Rating>();
    }

    public async Task<double> GetAverageRatingAsync(int entityId, string entityTypeStr)
    {
        if (Enum.TryParse<EntityType>(entityTypeStr, true, out var entityType))
        {
             return 0.0;
        }
        return 0.0;
    }

    public async Task<bool> UpdateRatingAsync(int id, Guid userId, int value, string? review = null)
    {
        return false; 
    }

    public async Task<bool> DeleteRatingAsync(int id, Guid userId)
    {
         return false;
    }

    #endregion
}



