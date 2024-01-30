namespace Recaudacion.Data
{
    public class WeatherForecastService
    {
        private static readonly string[] Summaries = new[]
        {
        "Helado", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Caluroso", "Sweltering", "Scorching"
        };

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray());
        }


        public Task<String[]> GetRecibosAsync(int num_recibos) {
            return Task.FromResult(Enumerable.Range(1, num_recibos).Select(index => "hola " + index).ToArray());
        }


    }
}