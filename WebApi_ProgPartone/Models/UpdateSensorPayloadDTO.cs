namespace WebApi_ProgPartone.Models
{
    public class UpdateSensorPayloadDTO
    {
        public int DeviceID { get; set; }
        public string MAC_Address { get; set; }
        public string Deployment_Location { get; set; }
        public string Category { get; set; }

    }
}
