using Isac.Bot.Common;
using Microsoft.Azure.Devices;
using System.Text;
using System.Threading.Tasks;

namespace Isac.Bot.Services
{
    public class IoTHub
    {
        private const string DeviceConnectionString = "YourDeviceConnectionString";

        public void DeviceControl(bool on, string device_name)
        {
            string command = string.Empty;

            if (Functions.RemoveDiacritics(device_name.ToLower()).Contains("lampada") || device_name.ToLower().Contains("luz"))
                command = on ? "LampOn" : "LampOff";
            System.Threading.ThreadPool.QueueUserWorkItem(a => SendEvent(command).Wait());
        }

        private async Task SendEvent(string command)
        {
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Amqp);
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(command));
            await serviceClient.SendAsync("rasp3", eventMessage);
        }
    }
}