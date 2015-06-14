using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Gpio;
using System.Threading.Tasks;
using ServiceBus.OpenSdk;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GpioController m_Gpio;

        public MainPage()
        {
            this.InitializeComponent();
            m_GpioStatus.Text = "Connecting to GPIO.";
            m_Gpio = GpioController.GetDefault();

            if (m_Gpio == null)
            {
                m_GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            m_Pin27 = m_Gpio.OpenPin(27);

            m_Pin22 = m_Gpio.OpenPin(22);

            m_Pin27.SetDriveMode(GpioPinDriveMode.Output);

            m_Pin22.SetDriveMode(GpioPinDriveMode.Output);

            DispatcherTimer t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(2);
            t.Tick += onTimer;
            t.Start();
            Task.Run(() =>
            {
                doIt(null, null);
            });
        }

        private void onTimer(object sender, object e)
        {
            doIt(null, null);
        }

        private bool m_Voltage = false;

        private GpioPin m_Pin27;

        private GpioPin m_Pin22;

        private async void doIt(object sender, RoutedEventArgs e)
        {
            m_Voltage = !m_Voltage;
            
           
            if (m_Voltage)
            {
                m_Pin27.Write(GpioPinValue.High);
                m_Pin22.Write(GpioPinValue.High);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    m_GpioStatus.Text = "ON";
                });

                await sendTelemetryData();
            }
            else
            {
                m_Pin27.Write(GpioPinValue.Low);
                m_Pin22.Write(GpioPinValue.Low);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    m_GpioStatus.Text = "OFF";
                });               
            }            
        }

        private async Task sendTelemetryData()
        {
            var tokenProvider = new SASTokenProvider("send_receive", "bADkRExvoXjw7t81Ln/MGRd2u6mHYMVjUV4+zEInz+4=");
            var sendClient = new QueueClient("factorybh", "ingress", tokenProvider, "http");

            try
            {
                Message msg = new Message(
                    new Command
                    {
                        DeviceId = "TEMPPi007",
                        Id = "102928",
                        DeviceName = "Pi Temperature Sensor",
                        Temperature = DateTime.Now.Second,
                        Timestamp = DateTime.Now.ToString(),
                    })
                {
                    Properties = { { "DeviceName", "Win10 Device" },
                                                    { "Temperature", DateTime.Now.Minute }}
                };

                await sendClient.Send(msg);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
