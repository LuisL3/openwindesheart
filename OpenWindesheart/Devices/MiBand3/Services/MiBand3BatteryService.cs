﻿/* Copyright 2020 Research group ICT innovations in Health Care, Windesheim University of Applied Sciences

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */

using Plugin.BluetoothLE;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using OpenWindesheart.Devices.MiBand3Device.Models;
using OpenWindesheart.Devices.MiBand3Device.Resources;
using OpenWindesheart.Models;

namespace OpenWindesheart.Devices.MiBand3Device.Services
{
    public class MiBand3BatteryService
    {
        private readonly MiBand3 _miBand3;
        public IDisposable RealTimeDisposible;

        public MiBand3BatteryService(MiBand3 device)
        {
            _miBand3 = device;
        }

        /// <summary>
        /// Get Raw Battery data.
        /// </summary>
        /// <exception cref="NullReferenceException">Throws exception if BatteryCharacteristic or its value is null.</exception>
        /// <returns>byte[]</returns>
        public async Task<byte[]> GetRawBatteryData()
        {
            var batteryCharacteristic = GetBatteryCharacteristic();
            if (batteryCharacteristic != null)
            {
                var gattResult = await batteryCharacteristic.Read();

                if (gattResult.Characteristic.Value != null)
                {
                    var rawData = gattResult.Characteristic.Value;
                    return rawData;
                }
                throw new NullReferenceException("BatteryCharacteristic value is null!");
            }
            throw new NullReferenceException("BatteryCharacteristic could not be found!");
        }

        /// <summary>
        /// Get Battery-object from raw data.
        /// </summary>
        /// <exception cref="NullReferenceException">Throws exception if rawData is null.</exception>
        /// <returns>Battery</returns>
        public async Task<BatteryData> GetCurrentBatteryData()
        {
            var rawData = await GetRawBatteryData();
            if (rawData != null)
            {
                return CreateBatteryObject(rawData);
            }
            throw new NullReferenceException("Rawdata is null!");
        }

        /// <summary>
        /// Creates Battery-object from rawData.
        /// </summary>
        /// <param name="rawData"></param>
        /// <exception cref="NullReferenceException">Throws exception if rawData is null.</exception>
        /// <returns>Battery</returns>
        private BatteryData CreateBatteryObject(byte[] rawData)
        {
            if (rawData != null)
            {
                var batteryPercentage = rawData[1];
                BatteryStatus status = BatteryStatus.NotCharging;

                if (rawData[2] == 1)
                {
                    status = BatteryStatus.Charging;
                }

                var battery = new BatteryData
                {
                    RawData = rawData,
                    Percentage = batteryPercentage,
                    Status = status
                };
                return battery;
            }
            throw new NullReferenceException("Rawdata of battery is null!");
        }

        /// <summary>
        /// Receive BatteryStatus-updates continuously.
        /// </summary>
        public void EnableRealTimeBattery(Action<BatteryData> callback)
        {
            RealTimeDisposible?.Dispose();
            RealTimeDisposible = _miBand3.GetCharacteristic(MiBand3Resource.GuidBatteryInfo).RegisterAndNotify().Subscribe(
                x => callback(CreateBatteryObject(x.Characteristic.Value))
            );
        }

        public void DisableRealTimeBattery()
        {
            RealTimeDisposible?.Dispose();
        }

        /// <summary>
        /// Get Battery Characteristic
        /// </summary>
        /// <returns>IGattCharacteristic</returns>
        private IGattCharacteristic GetBatteryCharacteristic()
        {
            return _miBand3.GetCharacteristic(MiBand3Resource.GuidBatteryInfo);
        }
    }
}
