namespace RecambiosWOW.Application.Services;

public record DimensionsDto
{
    public int Length { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int WheelBase { get; init; }
}