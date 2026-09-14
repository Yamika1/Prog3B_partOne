using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_ProgPartone.Data;
using WebApi_ProgPartone.Models;
using WebApi_ProgPartone.Models.Entities;


namespace WebApi_ProgPartone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorPayloadsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public SensorPayloadsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllSensorPayloads()
        {
            return Ok(dbContext.SensorPayloads.ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetSensorPayloadById(int id)
        {
            var sensorPayload = dbContext.SensorPayloads.Find(id);

            if (sensorPayload is null)
                return NotFound();

            return Ok(sensorPayload);
        }

        [HttpPost]
        public IActionResult AddSensor(AddSensorPayloadDTO addSensorPayload)
        {
            var addSensorEntity = new SensorPayload()
            {
                MAC_Address = addSensorPayload.MAC_Address,
                DeviceID = addSensorPayload.DeviceID,
                Category = addSensorPayload.Category,
                Deployment_Location = addSensorPayload.Deployment_Location
            };

            dbContext.Add(addSensorEntity);
            dbContext.SaveChanges();
            return Ok(addSensorEntity);
        }

       

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteSensorPayload(int id)
        {
            var sensorPayload = dbContext.SensorPayloads.Find(id);

            if (sensorPayload is null)
                return NotFound();

            dbContext.SensorPayloads.Remove(sensorPayload);
            dbContext.SaveChanges();
            return Ok();
        }
        [HttpGet("{id:int}/files")]
        public async Task<IActionResult> GetSensorFiles(int id)
        {
            var sensorExists =
                await dbContext.SensorPayloads
                    .AnyAsync(x => x.Id == id);

            if (!sensorExists)
            {
                return NotFound();
            }

            var files = await dbContext.SensorPayloadFile
                .Where(x => x.SensorPayloadId == id)
                .ToListAsync();

            return Ok(files);
        }

        [HttpPost("{id:int}/upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
                  int id,
                  IFormFile file)
        {
            var sensor =
                await dbContext.SensorPayloads
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (sensor == null)
            {
                return NotFound("Sensor not found.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select a file.");
            }

            const long maxFileSize = 10 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                return BadRequest(
                    "The file size cannot exceed 10 MB.");
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
                return BadRequest(
                    "Only JPG, JPEG, PNG, GIF, PDF, DOC and DOCX files are allowed.");
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

            await using (var stream =
                new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var sensorFile = new SensorPayloadFile
            {
                FileName = Path.GetFileName(file.FileName),
                FilePath =
                    "/uploads/sensors/" + uniqueFileName,
                FileSize = file.Length,
                UploadedDate = DateTime.UtcNow,
                SensorPayloadId = sensor.Id
            };

            dbContext.SensorPayloadFile.Add(sensorFile);

            await dbContext.SaveChangesAsync();

            return Ok(sensorFile);
        }


        [HttpDelete("{sensorId:int}/files/{fileId:int}")]
        public async Task<IActionResult> DeleteFile(
            int sensorId,
            int fileId)
        {
            var sensorFile =
                await dbContext.SensorPayloadFile
                    .FirstOrDefaultAsync(x =>
                        x.Id == fileId &&
                        x.SensorPayloadId == sensorId);

            if (sensorFile == null)
            {
                return NotFound();
            }

            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                sensorFile.FilePath.TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            dbContext.SensorPayloadFile.Remove(sensorFile);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }


         [HttpGet("telemetry")]
        public IActionResult GetTelemetry()
        {
            double[,] rawTelemetryBatches =
            {
                { 12.4, 13.1, 14.2, 15.0 },
                { 15.3, 16.1, 16.8, 17.2 },
                { 18.0, 18.4, 19.1, 20.0 }
            };

            var telemetryValues =
                ConvertToList(rawTelemetryBatches);

            return Ok(telemetryValues);
        }


      
        [HttpGet("generic-telemetry")]
        public IActionResult GetGenericTelemetry()
        {
            var temperaturePackets =
                new List<TelemetryPacket<float>>
                {
                    new TelemetryPacket<float>(
                        1,
                        "Temperature",
                        22.5f),

                    new TelemetryPacket<float>(
                        1,
                        "Temperature",
                        23.1f),

                    new TelemetryPacket<float>(
                        1,
                        "Temperature",
                        24.3f)
                };

            var powerPackets =
                new List<TelemetryPacket<int>>
                {
                    new TelemetryPacket<int>(
                        2,
                        "Power",
                        120),

                    new TelemetryPacket<int>(
                        2,
                        "Power",
                        150),

                    new TelemetryPacket<int>(
                        2,
                        "Power",
                        180)
                };

            var switchPackets =
                new List<TelemetryPacket<bool>>
                {
                    new TelemetryPacket<bool>(
                        3,
                        "Smart Switch",
                        true),

                    new TelemetryPacket<bool>(
                        3,
                        "Smart Switch",
                        false),

                    new TelemetryPacket<bool>(
                        3,
                        "Smart Switch",
                        true)
                };

            return Ok(new
            {
                TemperaturePackets = temperaturePackets,
                PowerPackets = powerPackets,
                SwitchPackets = switchPackets
            });
        }


        [HttpGet("add/{id1:int}/{id2:int}")]
        public async Task<IActionResult> Add(
            int id1,
            int id2)
        {
            var sensor1 =
                await dbContext.SensorPayloads.FindAsync(id1);

            var sensor2 =
                await dbContext.SensorPayloads.FindAsync(id2);

            if (sensor1 == null || sensor2 == null)
            {
                return NotFound();
            }

            var result =
                sensor1.SensorValue + sensor2.SensorValue;

            return Ok(new
            {
                Operation = "Addition",
                Sensor1 = sensor1.SensorValue,
                Sensor2 = sensor2.SensorValue,
                Result = result
            });
        }

        [HttpGet("subtract/{id1:int}/{id2:int}")]
        public async Task<IActionResult> Subtract(
            int id1,
            int id2)
        {
            var sensor1 =
                await dbContext.SensorPayloads.FindAsync(id1);

            var sensor2 =
                await dbContext.SensorPayloads.FindAsync(id2);

            if (sensor1 == null || sensor2 == null)
            {
                return NotFound();
            }

            var result =
                sensor1.SensorValue - sensor2.SensorValue;

            return Ok(new
            {
                Operation = "Subtraction",
                Sensor1 = sensor1.SensorValue,
                Sensor2 = sensor2.SensorValue,
                Result = result
            });
        }


        private List<T> ConvertToList<T>(T[,] array)
        {
            var result = new List<T>();

            for (int row = 0;
                 row < array.GetLength(0);
                 row++)
            {
                for (int column = 0;
                     column < array.GetLength(1);
                     column++)
                {
                    result.Add(array[row, column]);
                }
            }

            return result;
        }
    }
}