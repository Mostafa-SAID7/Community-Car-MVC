using CommunityCar.Application.Features.Reviews.ViewModels;
using CommunityCar.Application.Common.Models;

namespace CommunityCar.Application.Features.Reviews.DTOs;

public class ReviewsSearchResponse
{
    public IEnumerable<ReviewVM> Reviews { get; set; } = new List<ReviewVM>();
    public PaginationInfo Pagination { get; set; } = new();
    public ReviewsStatsVM Stats { get; set; } = new();
    public IEnumerable<string> AvailableCarMakes { get; set; } = new List<string>();
}