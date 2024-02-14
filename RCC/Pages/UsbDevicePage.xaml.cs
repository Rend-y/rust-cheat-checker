using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace RCC.Pages
{
    public partial class UsbDevicePage : APage
    {
        public UsbDevicePage(ILogger<UsbDevicePage> logger) : base(logger)
        {
            InitializeComponent();
            RunBackgroundWorker();
        }

        protected override void BackgroundWorkerDoWork()
        {
            base.BackgroundWorkerDoWork();
            string localPathToFile = $"{Utilities.PathToLocalApplication}\\USBDeview.exe";
            string pathToSaveUsbList = $"{Utilities.PathToLocalApplication}\\usb_info.xml";
            var loadXmlDocument = Utilities.GetXmlDocumentFromWebProcess(localPathToFile,
                "https://github.com/Midoruya/rust-cheat-checker/blob/main/RCC/Resources/USBDeview.exe?raw=true",
                pathToSaveUsbList);

            foreach (XElement element in loadXmlDocument)
            {
                string deviceName = element.Element("device_name")?.Value;
                string description = element.Element("description")?.Value;
                string deviceType = element.Element("device_type")?.Value;
                bool isConnect = element.Element("connected")?.Value == "Yes";
                string timeLastUsed = element.Element("last_plug_unplug_date")?.Value;
                string creatingTime = element.Element("created_date")?.Value;
                UsbDeviceInfo deviceInfo = new UsbDeviceInfo(deviceName, description, deviceType, isConnect,
                    timeLastUsed, creatingTime);
                BackgroundWorkerSendProgress(deviceInfo);
            }
        }

        protected override void BackgroundWorkerProgressChanged(object sender)
        {
            base.BackgroundWorkerProgressChanged(sender);
            var usbData = sender as UsbDeviceInfo;

            if (usbData == null)
                return;

            ListAllUsbDevice.Items.Add(new UsbDeviceInfo(usbData.DeviceName, usbData.Description, usbData.DeviceType,
                usbData.IsConnect, usbData.TimeLastUsed, usbData.CreatingTime));
        }

        public class UsbDeviceInfo
        {
            public UsbDeviceInfo(string device_name, string description, string device_type, bool is_connect,
                string time_last_used, string creating_time)
            {
                this.DeviceName = device_name;
                this.Description = description;
                this.DeviceType = device_type;
                this.IsConnect = is_connect;
                this.TimeLastUsed = time_last_used;
                this.CreatingTime = creating_time;
            }

            public string DeviceName { get; set; }
            public string Description { get; set; }
            public string DeviceType { get; set; }
            public bool IsConnect { get; set; }
            public string TimeLastUsed { get; set; }
            public string CreatingTime { get; set; }
        }
    }
}