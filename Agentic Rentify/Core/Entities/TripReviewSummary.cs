namespace Agentic_Rentify.Core.Entities;

public class TripReviewSummary
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public RatingCriteria RatingCriteria { get; set; } = new();
}
