namespace InteractiveDashboard.Models
{
    public class SensorPayloadFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }
        public int SensorPayloadId { get; set; }
        public SensorPayload SensorPayload { get; set; }

    }

}

