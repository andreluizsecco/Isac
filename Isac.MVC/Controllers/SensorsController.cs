using Isac.MVC.DAL;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Isac.MVC.Controllers
{
    public class SensorsController : Controller
    {
        [HttpPost]
        [Route("api/Sensors/DHT11Data")]
        public void DHT11Data(Guid token, decimal temperature, decimal humidity)
        {
            if (Guid.Parse("YourToken").Equals(token))
                Commands.AddDht11SensorData(temperature, humidity);
        }
    }
}