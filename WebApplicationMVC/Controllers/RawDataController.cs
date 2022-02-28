using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace WebApplicationMVC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RawDataController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public RawDataController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetRawData")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "PostRawData")]
        public String Post(String data)
        {
            string resp = "";
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "mdtdevsqlserver.database.windows.net";
                builder.UserID = "mdtadmin";
                builder.Password = "B@rcodeSp3cialists!";
                builder.InitialCatalog = "mdtdevdatabase";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    String sql = "INSERT INTO [dbo].[post_data_raw]([data])VALUES('"+data+"')";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resp = resp + "\t" + reader.GetString(0) + "\t" + reader.GetString(1);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                return e.ToString();
            }
            return resp;
        }
    }
}