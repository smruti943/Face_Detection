using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PointFindingModule.Model;
using PointFindingModule.Model.BalticDataModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Serilog;

namespace PointFindingModule.Controllers
{
    [ApiController]
    [Route("/")]
    public class ModuleController : ControllerBase
    {
        [HttpPost]
        [Route("token")]
        //endpoint REST

        public IActionResult ProcessTokenMessage([FromBody] object value)
        {
            try
            {
                //Parse token
                //var token = JsonConvert.DeserializeObject<XInputTokenMessage>(value?.ToString());
                Log.Information("Deserializing token");
                Console.WriteLine("Deserializing token");
                XInputTokenMessage token = JsonConvert.DeserializeObject<XInputTokenMessage>(value.ToString());

                Log.Information("Token source");
                Console.WriteLine("Token source");
                Log.Information("-------------------------------------------------");
                Console.WriteLine("-------------------------------------------------");
                Log.Debug(value.ToString());
                Console.WriteLine(value.ToString());
                Log.Information("-------------------------------------------------");
                Console.WriteLine("-------------------------------------------------");

                Log.Information("Token deserialized");
                Console.WriteLine("Token deserialized");
                Log.Information("-------------------------------------------------");
                Console.WriteLine("-------------------------------------------------");
                Log.Information("MsgUid: " + token.MsgUid);
                Console.WriteLine("MsgUid: " + token.MsgUid);
                Log.Information("PinName: " + token.PinName);
                Console.WriteLine("PinName: " + token.PinName);
                Log.Information("TokenSeqStack: " + token.TokenSeqStack);
                Console.WriteLine("TokenSeqStack: " + token.TokenSeqStack);
                Log.Information("AccessType: " + token.AccessType);
                Console.WriteLine("AccessType: " + token.AccessType);
                TokenData tokenData = JsonConvert.DeserializeObject<TokenData>(token.Values);
                Log.Information("FileName: " + tokenData.FileName);
                Console.WriteLine("FileName: " + tokenData.FileName);
                Log.Information("Database: " + tokenData.Database);
                Console.WriteLine("Database: " + tokenData.Database);
                Log.Information("Collection: " + tokenData.Collection);
                Console.WriteLine("Collection: " + tokenData.Collection);
                Log.Information("ObjectId: " + tokenData.ObjectId);
                Console.WriteLine("ObjectId: " + tokenData.ObjectId);
                Log.Information("-------------------------------------------------");
                Console.WriteLine("-------------------------------------------------");

                Log.Information("Deserializing pins");
                Console.WriteLine("Deserializing pins");
                //Read pin configuration from file;
                var pinConfig = System.IO.File.ReadAllText(Environment.GetEnvironmentVariable("SYS_PIN_CONFIG_FILE_PATH"));
                //var pinConfig = System.IO.File.ReadAllText("pin_Examples.json");
                System.Console.WriteLine(pinConfig);
                Log.Information(pinConfig);
                var pins= JsonConvert.DeserializeObject<List<PinMetadata>>(pinConfig);
                var pinInputMetadata = new[]{ pins[0], pins[1]};
                Log.Information("Pins read");
                Console.WriteLine("Pins read");

                // ###############################################################################################################
                // # Switch-case for preforming different calculation for different input pins.                                  #
                // # Change according to a number of INPUT pins.                                                                 #
                // ###############################################################################################################
                PinMetadata pinInput = null;
                foreach (PinMetadata pin in pinInputMetadata)
                {
                    if (token.PinName == pin.PinName)
                        pinInput = pin;
                }
                if (token.PinName == pinInput.PinName)
                {
                    // Implementation of data_processing function or equivalent
                    // should be done in separate file according to the example
                    new Task(() =>
                        ComputationModule.DataProcessing1(token)).Start();

                }
                //else if (token.pinName == pinInputMetadata[1].PinName)
                //{
                //    new Task(() =>
                //        ComputationModule.DataProcessing2(token)).Start();
                //}
                else
                {
                    Log.Information("Error wrong pin: " + token.PinName, pinInputMetadata[0].PinName, token.PinName == pinInputMetadata[1].PinName);
                    Console.WriteLine("Error wrong pin: " + token.PinName, pinInputMetadata[0].PinName, token.PinName == pinInputMetadata[1].PinName);
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                Log.Information("Error ModuleController: " + e.Message);
                Console.WriteLine("Error ModuleController: " + e.Message);
                return BadRequest(e.Message);
            }

            return Ok("to jest nasze ok");
        }

        // Endpoint responsible for providing current job status.
        [HttpGet]
        [Route("status")]
        public IActionResult GetStatus()
        {
            return Ok(ComputationModule.JobStatus);
        }
    }
}
