using k8s.Models;
using k8s;
using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Controllers
{
    [ApiController]
    [Route( "[controller]" )]
    public class ExperimentController : ControllerBase
    {
        public ExperimentController()
        {
        }
        [HttpPost( Name = "CreatePod" )]
        public async Task<IActionResult> Get( [FromQuery] string id )
        {
            IKubernetes k8sClient = GetKubernetesClient();
            var workerPodName = await CreatePod( k8sClient, id );
            var getWorkerPodUrl = GetPodURLFromMapping( workerPodName );
            UpdateIdToWorkerURLMapping( id, getWorkerPodUrl );
            return Ok();
        }

        private async Task<string> CreatePod( IKubernetes client, string id )
        {
            var pod = new V1Pod()
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "api-b-pod",
                },
                Spec = new V1PodSpec
                {
                    Containers = new List<V1Container>
                {
                    new V1Container
                    {
                        Name = "api-b-container",
                        Image = "your-api-b-image",
                    },
                },
                },
            };

            var namespaceParameter = "default";

            try
            {
                var createdPod = await client.CoreV1.CreateNamespacedPodAsync( pod, namespaceParameter );
                return createdPod.Metadata.Name;
            }
            catch( Exception ex )
            {
                Console.WriteLine( $"Error creating pod for API B: {ex.Message}" );
                return null;
            }
        }

        [HttpGet( Name = "GetName" )]
        public string GetName( [FromQuery] string id )
        {
            var workerPodUrl = GetPodURLFromMapping( id );
            return RedirectRequest( workerPodUrl, id );
        }

        private string RedirectRequest( string podUrl, string id )
        {
            //Http request
            throw new NotImplementedException();
        }
        private IKubernetes GetKubernetesClient()
        {
            throw new NotImplementedException();
        }
        private void UpdateIdToWorkerURLMapping( string id, object getWorkerPodUrl )
        {
            throw new NotImplementedException();
        }

        private string GetPodURLFromMapping( string workerPodName )
        {
            throw new NotImplementedException();
        }
    }
}