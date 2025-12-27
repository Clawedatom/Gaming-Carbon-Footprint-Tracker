using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalCarbonFootprintTracker
{
    internal class SessionSummary
    {
        private string gameName;

        private float targetDuration; // hours

        private float sessionDuration; // hours

        private DeviceType deviceUsed = DeviceType.Desktop;

        private float carbonEmissions;

        private float KWhPerHour;
        private const float CO2PerKWh = 0.233f; //average for uk

        public DeviceType DeviceUsed
        {
            get {  return deviceUsed; }
            set
            {
                deviceUsed = value;
                KWhPerHour = GetkWhFromDevice(deviceUsed);
                
            }
        }

        public float GetCarbon()
        {
            return carbonEmissions;
        }

        public float GetSessionHours()
        {
            return sessionDuration;
        }


        public SessionSummary(string gameName, float targetDuration, DeviceType type)
        {
            this.gameName = gameName;
            this.targetDuration = targetDuration;
            DeviceUsed = type;
        }

        public void OnSessionEnd(float timer)
        {
            this.sessionDuration = timer;
            carbonEmissions = sessionDuration * KWhPerHour * CO2PerKWh;
        }

        public override string ToString()
        {
            TimeSpan sessionTime = TimeSpan.FromHours(sessionDuration);
            TimeSpan targetTime = TimeSpan.FromHours(targetDuration);

            string sessionFormatted = sessionTime.TotalHours >= 1   
                ? $"{(int)sessionTime.TotalHours}h {sessionTime.Minutes}m {sessionTime.Seconds}s"
                : $"{sessionTime.Minutes}m {sessionTime.Seconds}s";

            return
                $"===== Session Summary ===== \n" +
                $"Game: {gameName}\n" +
                $"Target Duration: {targetTime.TotalHours:0.#}h\n" +
                $"Session Duration: {sessionFormatted}\n" +
                $"Device Used: {deviceUsed}\n" +
                $"Estimated Carbon Footprint: {carbonEmissions:0.000} kg CO2\n" +
                $"=========================== ";
        }


        public string SaveData()
        {
            return $"{gameName} {targetDuration} {sessionDuration} {deviceUsed}+";
        }

        


        
        static public float GetkWhFromDevice(DeviceType type)
        {
            float KWhPerHour = 0.0f;
            switch (type)
            {
                case DeviceType.Desktop:
                KWhPerHour = 0.3f;
                break;

                case DeviceType.Laptop:
                KWhPerHour = 0.075f;
                break;

                case DeviceType.Console:
                KWhPerHour = 0.125f;
                break;
                case DeviceType.Tablet:
                KWhPerHour = 0.03f;
                break;
            }
            return KWhPerHour;
        }
    }
}
public enum DeviceType
{
    Desktop,
    Laptop,
    Console,
    Tablet
}