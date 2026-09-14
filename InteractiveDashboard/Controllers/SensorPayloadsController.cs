using InteractiveDashboard.Data;
using InteractiveDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InteractiveDashboard.Controllers
{
    public class SensorPayloadsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SensorPayloadsController(IHttpClientFactory httpClientFactory) { 
            
            _httpClientFactory = httpClientFactory; 
        
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("SensorApi"); 
           
            var response = await client.GetAsync("SensorPayloads"); 
           
            if (!response.IsSuccessStatusCode) {
             
                return View(new List<SensorPayload>());
           
            }
           
            var json = await response.Content.ReadAsStringAsync(); 
            
            var sensors = JsonSerializer.Deserialize<List<SensorPayload>>(json, new JsonSerializerOptions { 
               
                PropertyNameCaseInsensitive = true }) ?? new List<SensorPayload>(); 
            
            ViewBag.TotalSensors = CountSensors(sensors, 0);
          
            ViewBag.TotalCategories = sensors.Where(x => !string.IsNullOrEmpty(x.Category)).Select(x => x.Category).Distinct().Count(); 
           
            ViewBag.TotalLocations = sensors.Where(x => !string.IsNullOrEmpty(x.Deployment_Location)).Select(x => x.Deployment_Location).Distinct().Count(); 
           
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

            var client = _httpClientFactory.CreateClient("SensorApi"); 
          
            var json = JsonSerializer.Serialize(sensorpayload); 
          
            var content = new StringContent(json, Encoding.UTF8, "application/json");
          
            var response = await client.PostAsync("SensorPayloads", content); if (!response.IsSuccessStatusCode) { 
              
                ModelState.AddModelError("", "Unable to add the sensor through the API.");
              
                return View(sensorpayload); 
           
            }
          
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _httpClientFactory.CreateClient("SensorApi");
            
            var response = await client.GetAsync($"SensorPayloads/{id}"); 
            if (!response.IsSuccessStatusCode) { 
                return NotFound(); 
            }

            var json = await response.Content.ReadAsStringAsync();
            var sensor = JsonSerializer.Deserialize<SensorPayload>(json, new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true }); 
            if (sensor == null) { 
                return NotFound(); 
            }
            var filesResponse = await client.GetAsync($"SensorPayloads/{id}/files"); 
            if (filesResponse.IsSuccessStatusCode) {
                var filesJson = await filesResponse.Content.ReadAsStringAsync(); 
                var files = JsonSerializer.Deserialize<List<SensorPayloadFile>>(filesJson, new JsonSerializerOptions { 
                    PropertyNameCaseInsensitive = true }); 
                sensor.Files = files ?? new List<SensorPayloadFile>();
            }
            return View(sensor);
        }

        [HttpGet]
        public async Task<IActionResult> Upload(int id)
        {
            var client = _httpClientFactory.CreateClient("SensorApi"); 
            
            var response = await client.GetAsync($"SensorPayloads/{id}"); 
            
            if (!response.IsSuccessStatusCode) { 
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var sensor = JsonSerializer.Deserialize<SensorPayload>(json, new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true }); 
            if (sensor == null) { 
                return NotFound();
            }
            return View(sensor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int id, IFormFile file)
        {
            if (file == null || file.Length == 0) { 
          
                ModelState.AddModelError("file", "Please select a file."); 
                return RedirectToAction(nameof(Upload), new { id });
            }
            var client = _httpClientFactory.CreateClient("SensorApi"); 

            using var form = new MultipartFormDataContent(); 

            using var stream = file.OpenReadStream();

            using var fileContent = new StreamContent(stream); 

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            form.Add(fileContent, "file", file.FileName); 

            var response = await client.PostAsync($"SensorPayloads/{id}/upload", form); 
            if (!response.IsSuccessStatusCode) { 

                var error = await response.Content.ReadAsStringAsync(); 

                ModelState.AddModelError("file", error); 

                return RedirectToAction(nameof(Upload), new { id }); 
            }
            return RedirectToAction(nameof(Details), new { id });
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
            var client = _httpClientFactory.CreateClient("SensorApi"); 
            
            var response = await client.GetAsync($"SensorPayloads/add/{id1}/{id2}"); 
            if (!response.IsSuccessStatusCode) {
                return NotFound();
            }
            var json = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json); 
            ViewBag.Operation = document.RootElement.GetProperty("operation").GetString(); 
            ViewBag.Sensor1 = document.RootElement.GetProperty("sensor1").GetDouble(); 
            ViewBag.Sensor2 = document.RootElement.GetProperty("sensor2").GetDouble(); 
            ViewBag.Result = document.RootElement.GetProperty("result").GetDouble(); 
            
            return View("Calculation");
        }
        [HttpGet]
        public async Task<IActionResult> Subtract(int id1, int id2)
        {
            var client = _httpClientFactory.CreateClient("SensorApi"); 
            
            var response = await client.GetAsync($"SensorPayloads/subtract/{id1}/{id2}");
            
            if (!response.IsSuccessStatusCode) { 
                return NotFound(); 
            }
            var json = await response.Content.ReadAsStringAsync(); 
            using var document = JsonDocument.Parse(json); 
            ViewBag.Operation = document.RootElement.GetProperty("operation").GetString(); 
            ViewBag.Sensor1 = document.RootElement.GetProperty("sensor1").GetDouble(); 
            ViewBag.Sensor2 = document.RootElement.GetProperty("sensor2").GetDouble(); 
            ViewBag.Result = document.RootElement.GetProperty("result").GetDouble(); 
            
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