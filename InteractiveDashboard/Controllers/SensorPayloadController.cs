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

    }
}