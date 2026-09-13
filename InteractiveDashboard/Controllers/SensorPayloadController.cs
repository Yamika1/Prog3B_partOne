using InteractiveDashboard.Data;
using InteractiveDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InteractiveDashboard.Controllers
{
    public class SensorPayloadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SensorPayloadController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<SensorPayload> sensors = await _context.SensorPayloads.ToListAsync();

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
        // GET: /Books/Add
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DeviceID,MAC_Address,Deployment_Location,Category,SensorValue,Files")] SensorPayload sensorpayload)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sensorpayload);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sensorpayload);

        }
        [HttpGet]
        public async Task<IActionResult> Upload(int id)
        {
            var sensor = await _context.SensorPayloads
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
                ModelState.AddModelError("file", "Please select a file.");
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

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

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

            var uniqueFileName = Guid.NewGuid().ToString() + extension;

            var filePath = Path.Combine(
                uploadsFolder,
                uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var sensorFile = new SensorPayloadFile
            {
                FileName = file.FileName,
                FilePath = "/uploads/sensors/" + uniqueFileName,
                FileSize = file.Length,
                UploadedDate = DateTime.Now,
                SensorPayloadId = sensor.Id
            };

            _context.SensorPayloadFiles.Add(sensorFile);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = sensor.Id });
        }
        [HttpGet]
        public async Task<IActionResult> Add(int id1, int id2)
        {
            var sensor1 = await _context.SensorPayloads.FindAsync(id1);
            var sensor2 = await _context.SensorPayloads.FindAsync(id2);

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
            var sensor1 = await _context.SensorPayloads.FindAsync(id1);
            var sensor2 = await _context.SensorPayloads.FindAsync(id2);

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

        private bool SensorPayloadExists(int? id)
        {
            return _context.SensorPayloads.Any(e => e.Id == id);
        }
    }
}