using SQLite;
using System;

namespace technoleight_THandy.Models
{
    public class DeviceLocation
    {
        [PrimaryKey]
        public string UUID { get; set; } = Guid.NewGuid().ToString();
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime GetDateTime { get; set; }
    }
}
