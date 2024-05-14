using System.ComponentModel.DataAnnotations;
using System;
namespace BlazerWebApp.Shared;

public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string Forecast { get; set; }
    public string CloudCover { get; set; }
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



//public class WeatherForecast
//{
//    public DateTime Date { get; set; }
//    public int TemperatureC { get; set; }
//    public string Forecast { get; set; }
//    public string CloudCover { get; set; }
//}