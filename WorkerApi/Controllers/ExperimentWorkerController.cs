using Microsoft.AspNetCore.Mvc;

namespace WorkerApi.Controllers
{
    [ApiController]
    [Route( "[controller]" )]
    public class ExperimentWorkerController : ControllerBase
    {

        public ExperimentWorkerController()
        {
        }

        [HttpGet( Name = "GetName" )]
        public string Get( [FromQuery] string id)
        {
            return "Name"+id;
        }
    }
}