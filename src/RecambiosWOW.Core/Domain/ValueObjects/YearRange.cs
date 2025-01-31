namespace RecambiosWOW.Core.Domain.ValueObjects;

public record YearRange
{
    public int StartYear { get; }
    public int EndYear { get; }

    public YearRange(int startYear, int endYear)
    {
        if (startYear > endYear)
            throw new ArgumentException("Start year cannot be greater than end year");
            
        if (startYear < 1900)
            throw new ArgumentException("Start year cannot be before 1900");
            
        if (endYear > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("End year cannot be in the future");

        StartYear = startYear;
        EndYear = endYear;
    }

    public bool Contains(int year) => year >= StartYear && year <= EndYear;
}