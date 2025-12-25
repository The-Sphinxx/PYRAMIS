namespace Agentic_Rentify.Core.Entities;

public class HotelReviewSummary
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public RatingCriteria RatingCriteria { get; set; } = new();
}
