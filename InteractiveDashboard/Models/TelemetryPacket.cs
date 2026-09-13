namespace InteractiveDashboard.Models
{
    public class TelemetryPacket<T>
    {
        public int DeviceID { get; set; }

        public string Category { get; set; }

        public DateTime Timestamp { get; set; }

        public T Data { get; set; }

        public TelemetryPacket(
            int deviceID,
            string category,
            T data)
        {
            DeviceID = deviceID;
            Category = category;
            Data = data;
            Timestamp = DateTime.UtcNow;
        }
    }

}