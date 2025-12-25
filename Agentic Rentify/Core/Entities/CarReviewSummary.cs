namespace Agentic_Rentify.Core.Entities;

public class CarReviewSummary
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public CarRatingCriteria RatingCriteria { get; set; } = new();
}
