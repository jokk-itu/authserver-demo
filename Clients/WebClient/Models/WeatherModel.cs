using WebApp.Contracts;

namespace WebApp.Models;

#nullable disable
public class WeatherModel
{
  public IEnumerable<WeatherDto> WeatherDtos { get; set; }
}
