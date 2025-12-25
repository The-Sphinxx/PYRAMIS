namespace Agentic_Rentify.Core.Entities;

public class TripAmenitiesInfo
{
    public bool Transport { get; set; } = true;
    public bool Accommodation { get; set; } = true;
    public int Meals { get; set; } = 0; // Number of meals included
}
