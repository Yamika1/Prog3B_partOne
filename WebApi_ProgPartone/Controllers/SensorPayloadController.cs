using Microsoft.AspNetCore.Mvc;
using WebApi_ProgPartone.Data;
using WebApi_ProgPartone.Models;
using WebApi_ProgPartone.Models.Entities;


namespace WebApi_ProgPartone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorPayloadController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public SensorPayloadController(ApplicationDbContext dbContext)
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

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateSensorPayload(int id, UpdateSensorPayloadDTO updateSensorPayload)
        {
            var sensorPayload = dbContext.SensorPayloads.Find(id);

            if (sensorPayload is null)
                return NotFound();

            sensorPayload.MAC_Address = updateSensorPayload.MAC_Address;
            sensorPayload.Deployment_Location = updateSensorPayload.Deployment_Location;
            sensorPayload.Category = updateSensorPayload.Category;
            sensorPayload.DeviceID = updateSensorPayload.DeviceID;
         

            dbContext.SaveChanges();
            return Ok(sensorPayload);
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

      
    }
}