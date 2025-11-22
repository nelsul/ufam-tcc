namespace IcompCare.Application.DTOs.Availabilities;

public class ProfessionalAvailabilityDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public string Date { get; set; } = string.Empty;
    public List<TimeRangeDto> TimeRanges { get; set; } = new();
}

public class TimeRangeDto
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
