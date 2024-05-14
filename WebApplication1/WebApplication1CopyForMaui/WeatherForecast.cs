using System.ComponentModel.DataAnnotations;

namespace WebApplication1;

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
public class Item
{
    public string PkId { get; set; }
    [Required]
    public string Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Barcode { get; set; }
    public decimal Price { get; set; }
}
