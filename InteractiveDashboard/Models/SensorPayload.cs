using System.ComponentModel.DataAnnotations.Schema;

namespace InteractiveDashboard.Models
{
    [Table("SensorPayload")]
    public class SensorPayload
    {
        public int Id { get; set; }
        public int DeviceID { get; set; }
        public string MAC_Address { get; set; }
        public string Deployment_Location { get; set; }
        public string Category { get; set; }
        public double SensorValue { get; set; }

        public ICollection<SensorPayloadFile> Files { get; set; } = new List<SensorPayloadFile>();

        public static SensorPayload operator +(SensorPayload a, SensorPayload b)
        {
            return new SensorPayload
            {
                SensorValue = a.SensorValue + b.SensorValue
            };
        }

        public static SensorPayload operator -(SensorPayload a, SensorPayload b)
        {
            return new SensorPayload
            {
                SensorValue = a.SensorValue - b.SensorValue
            };
        }
    }
}