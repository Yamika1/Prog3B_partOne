using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi_ProgPartone.Models.Entities
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

    }
}
