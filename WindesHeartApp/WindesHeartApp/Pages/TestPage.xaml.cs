﻿using System;
using System.Collections.Generic;
using WindesHeartSDK;
using WindesHeartSDK.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WindesHeartApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPage : ContentPage
    {
        BLEDevice Device = null;
        public TestPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            List<BLEDevice> devices = await Windesheart.ScanForDevices();
            if (devices.Count > 0)
            {
                Device = devices[0];
                Device.Connect();
            }
        }

        private async void Disconnect(object sender, EventArgs e)
        {
            Device.Disconnect();
        }

        private async void ReadCurrentBattery(object sender, EventArgs e)
        {
            var battery = await Device.GetBattery();
            Console.WriteLine("Battery: " + battery.BatteryPercentage + "%");
        }

        private async void SetTime(object sender, EventArgs e)
        {
            bool timeset = await Device.SetTime(new DateTime(2000, 1, 1, 1, 1, 1));
            Console.WriteLine("Time set " + timeset);
        }

        private async void SetCurrentTime(object sender, EventArgs e)
        {
            bool timeset = await Device.SetTime(DateTime.Now);
            Console.WriteLine("Time set " + timeset);
        }

        private async void GetCurrentBattery()
        {
            var battery = await Device.GetBattery();
            Console.WriteLine("Battery: " + battery.BatteryPercentage + "%");
            Console.WriteLine("Batterystatus: " + battery.Status);
        }

        private async void ReadBatteryContinuous(object sender, EventArgs e)
        {
            Device.EnableRealTimeBattery(GetBatteryStatus);
        }

        private void GetBatteryStatus(Battery battery)
        {
            Console.WriteLine("Batterypercentage is now: " + battery.BatteryPercentage + "% || Batterystatus is: " + battery.Status);
        }

        private void GetHeartrate(Heartrate heartrate)
        {
            Console.WriteLine(heartrate.HeartrateValue);
        }

        public void GetHeartRate(object sender, EventArgs e)
        {
            Device.SetHeartrateMeasurementInterval(1);
            Device.EnableRealTimeHeartrate(GetHeartrate);
        }

        public async void GetSteps(object sender, EventArgs e)
        {
            StepInfo steps = await Device.GetSteps();
            Console.WriteLine("Steps: " + steps.GetStepCount());
        }

        public void EnableRealTimeSteps(object sender, EventArgs e)
        {
            Device.EnableRealTimeSteps(OnStepsChanged);
            Console.WriteLine("Enabled realtime steps");
        }

        public void DisableRealTimeSteps(object sender, EventArgs e)
        {
            Device.DisableRealTimeSteps();
            Console.WriteLine("Disabled realtime steps");
        }

        public void OnStepsChanged(StepInfo steps)
        {
            Console.WriteLine("Steps updated: " + steps.GetStepCount());
        }

        private void GoBack_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}
