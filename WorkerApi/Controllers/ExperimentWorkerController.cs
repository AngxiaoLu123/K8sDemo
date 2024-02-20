using Microsoft.AspNetCore.Mvc;

namespace WorkerApi.Controllers
{
    [ApiController]
    public class ExperimentWorkerController : ControllerBase
    {

        public ExperimentWorkerController()
        {
        }

        [HttpGet]
        [Route( "/name" )]
        public string Get( [FromQuery] string id)
        {
            return "(v3) Name" + id;
        }
    }
}