using Isac.MVC.DAL;
using Microsoft.AspNetCore.SignalR;

namespace Isac.MVC.Hubs
{
    public class SensorHub : Hub
    {
        public void Send()
        {
            (var temperature, var humidity) =  Queries.GetSensorsData();
            Clients.All.SendAsync("sensorsData", temperature, humidity);
        }
    }
}