using Isac.MVC.DAL;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Isac.MVC.Hubs
{
    public class SensorHub : Hub
    {
        public void Send()
        {
            (var date, var sender, var temperature, var humidity) = Queries.GetSensorsData();
            var localDate = TimeZoneInfo.ConvertTimeFromUtc(date.Value, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            Clients.All.SendAsync("sensorsData", string.Format("{0:dd/MM/yyyy HH:mm:ss}", localDate), sender, temperature, humidity);
        }
    }
}