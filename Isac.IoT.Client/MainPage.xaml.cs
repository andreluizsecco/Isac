using Microsoft.Azure.Devices.Client;
using Sensors.Dht;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Isac.IoT.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string DeviceConnectionString = "YourDeviceConnectionString";
        private DispatcherTimer timer = new DispatcherTimer();
        private GpioPin pinDht = null;
        private GpioPin pinLamp = null;
        private IDht sensor = null;
        private float humidity = 0f;
        private float temperature = 0f;

        public MainPage()
        {
            this.InitializeComponent();

            Task.Run(() => ReceiveDataFromAzure());

            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += timer_Tick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            pinLamp = GpioController.GetDefault().OpenPin(27, GpioSharingMode.Exclusive);
            pinLamp.Write(GpioPinValue.Low);
            pinLamp.SetDriveMode(GpioPinDriveMode.Output);
            pinDht = GpioController.GetDefault().OpenPin(17, GpioSharingMode.Exclusive);
            sensor = new Dht11(pinDht, GpioPinDriveMode.Input);

            timer.Start();
        }

        private async void timer_Tick(object sender, object e)
        {
            DhtReading reading = new DhtReading();
            reading = await sensor.GetReadingAsync().AsTask();

            if (reading.IsValid)
            {
                this.temperature = Convert.ToSingle(reading.Temperature);
                this.humidity = Convert.ToSingle(reading.Humidity);

                //Aqui é possível chamar uma API, passando os valores de temperatura e umidade, fazendo que sua API insira em seu banco de dados esses valores
                var url = "appname.azurewebsites.net";
                var token = "YourToken";
                var client = new HttpClient();
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://{url}/api/Sensors/DHT11Data"));
                requestMessage.Content = new StringContent($"token={token}&temperature={temperature}&humidity={humidity}", Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = client.SendAsync(requestMessage);
            }
        }

        public async Task ReceiveDataFromAzure()
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Amqp);

            Message receivedMessage;
            string messageData;

            
            while (true)
            {
                try
                {
                    receivedMessage = await deviceClient.ReceiveAsync();

                    if (receivedMessage != null)
                    {
                        messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        if (messageData.Equals("LampOn"))
                            pinLamp.Write(GpioPinValue.High);
                        else
                            pinLamp.Write(GpioPinValue.Low);
                        await deviceClient.CompleteAsync(receivedMessage);
                    }
                }
                catch (Exception ex) { }
            }
        }
    }
}