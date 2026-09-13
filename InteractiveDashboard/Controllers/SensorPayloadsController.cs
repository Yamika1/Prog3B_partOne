using InteractiveDashboard.Data;
using InteractiveDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InteractiveDashboard.Controllers
{
    public class SensorPayloadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SensorPayloadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<SensorPayload> sensors =
                await _context.SensorPayloads.ToListAsync();

            ViewBag.TotalSensors = CountSensors(sensors, 0);

            ViewBag.TotalCategories = sensors
                .Where(x => !string.IsNullOrEmpty(x.Category))
                .Select(x => x.Category)
                .Distinct()
                .Count();

            ViewBag.TotalLocations = sensors
                .Where(x => !string.IsNullOrEmpty(x.Deployment_Location))
                .Select(x => x.Deployment_Location)
                .Distinct()
                .Count();

            return View(sensors);
        }

        private int CountSensors(List<SensorPayload> sensors, int index)
        {
            if (index >= sensors.Count)
            {
                return 0;
            }

            return 1 + CountSensors(sensors, index + 1);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,DeviceID,MAC_Address,Deployment_Location,Category,SensorValue")]
        SensorPayload sensorpayload)
        {
            if (!ModelState.IsValid)
            {
                return View(sensorpayload);
            }

            _context.SensorPayloads.Add(sensorpayload);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorPayload = await _context.SensorPayloads
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sensorPayload == null)
            {
                return NotFound();
            }

            return View(sensorPayload);
        }

        [HttpGet]
        public async Task<IActionResult> Upload(int id)
        {
            var sensor = await _context.SensorPayloads
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sensor == null)
            {
                return NotFound();
            }

            return View(sensor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int id, IFormFile file)
        {
            var sensor = await _context.SensorPayloads
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sensor == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(
                    "file",
                    "Please select a file.");

                return View(sensor);
            }

            const long maxFileSize = 10 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                ModelState.AddModelError(
                    "file",
                    "The file size cannot exceed 10 MB.");

                return View(sensor);
            }

            var allowedExtensions = new[]
            {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".pdf",
            ".doc",
            ".docx"
        };

            var extension = Path
                .GetExtension(file.FileName)
                .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    "file",
                    "Only JPG, JPEG, PNG, GIF, PDF, DOC and DOCX files are allowed.");

                return View(sensor);
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "sensors");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName =
                Path.GetRandomFileName() + extension;

            var filePath = Path.Combine(
                uploadsFolder,
                uniqueFileName);

            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var sensorFile = new SensorPayloadFile
            {
                FileName = Path.GetFileName(file.FileName),
                FilePath = "/uploads/sensors/" + uniqueFileName,
                FileSize = file.Length,
                UploadedDate = DateTime.UtcNow,
                SensorPayloadId = sensor.Id
            };

            _context.SensorPayloadFiles.Add(sensorFile);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Details),
                new { id = sensor.Id });
        }
        [HttpGet] public IActionResult Telemetry() { 
            
            double[,] rawTelemetryBatches = { 
                { 12.4, 13.1, 14.2, 15.0 }, 
{ 15.3, 16.1, 16.8, 17.2 }, 
                { 18.0, 18.4, 19.1, 20.0 } }; 
         
            List<double> telemetryValues = ConvertToList(rawTelemetryBatches);
         
            ViewBag.TelemetryValues = telemetryValues; 
          
            return View(); 
        }

        private List<T> ConvertToList<T>(T[,] array)
        {

            List<T> result = new List<T>();

            for (int row = 0; row < array.GetLength(0); row++)
            {

                for (int column = 0; column < array.GetLength(1); column++)
                {
                    result.Add(array[row, column]);
                }
            }
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> Add(int id1, int id2)
        {
            var sensor1 =
                await _context.SensorPayloads.FindAsync(id1);

            var sensor2 =
                await _context.SensorPayloads.FindAsync(id2);

            if (sensor1 == null || sensor2 == null)
            {
                return NotFound();
            }

            var result = sensor1 + sensor2;

            ViewBag.Result = result.SensorValue;
            ViewBag.Operation = "Addition";
            ViewBag.Sensor1 = sensor1.SensorValue;
            ViewBag.Sensor2 = sensor2.SensorValue;

            return View("Calculation");
        }

        [HttpGet]
        public async Task<IActionResult> Subtract(int id1, int id2)
        {
            var sensor1 =
                await _context.SensorPayloads.FindAsync(id1);

            var sensor2 =
                await _context.SensorPayloads.FindAsync(id2);

            if (sensor1 == null || sensor2 == null)
            {
                return NotFound();
            }

            var result = sensor1 - sensor2;

            ViewBag.Result = result.SensorValue;
            ViewBag.Operation = "Subtraction";
            ViewBag.Sensor1 = sensor1.SensorValue;
            ViewBag.Sensor2 = sensor2.SensorValue;

            return View("Calculation");
        }

        private List <TelemetryPacket<T>> ProcessTelemetry<T>(
            List<TelemetryPacket<T>> packets)
        {
            return packets.OrderBy(x => x.Timestamp).ToList();
        }
        [HttpGet]
        public IActionResult GenericTelemetry()
        {
            List<TelemetryPacket<float>> temperaturePackets = new List<TelemetryPacket<float>>
{
new TelemetryPacket<float>(1, "Temperature", 22.5f),
new TelemetryPacket<float>(1, "Temperature", 23.1f),
new TelemetryPacket<float>(1, "Temperature", 24.3f)
};

List<TelemetryPacket<int>> powerPackets = new List<TelemetryPacket<int>>
{
    new TelemetryPacket<int>(2, "Power", 120),
    new TelemetryPacket<int>(2, "Power", 150),
    new TelemetryPacket<int>(2, "Power", 180)
};

            List<TelemetryPacket<bool>> switchPackets = new List<TelemetryPacket<bool>>
{
    new TelemetryPacket<bool>(3, "Smart Switch", true),
    new TelemetryPacket<bool>(3, "Smart Switch", false),
    new TelemetryPacket<bool>(3, "Smart Switch", true)
};

            ViewBag.TemperaturePackets = temperaturePackets;
            ViewBag.PowerPackets = powerPackets;
            ViewBag.SwitchPackets = switchPackets;

            return View();


}

    }

}