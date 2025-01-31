namespace RecambiosWOW.Core.Domain.ValueObjects;

public record Dimensions
{
    public int Length { get; }
    public int Width { get; }
    public int Height { get; }
    public int WheelBase { get; }

    public Dimensions(int length, int width, int height, int wheelBase)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be positive", nameof(length));
            
        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));
            
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));
            
        if (wheelBase <= 0)
            throw new ArgumentException("Wheel base must be positive", nameof(wheelBase));

        Length = length;
        Width = width;
        Height = height;
        WheelBase = wheelBase;
    }
}