using k8s.Models;
using k8s;
using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Controllers
{
    [ApiController]
    public class ExperimentController : ControllerBase
    {
        public ExperimentController()
        {
        }

        [HttpPost]
        [Route( "/pod" )]
        public async Task<IActionResult> Get( [FromQuery] string id )
        {
            try
            {
                IKubernetes k8sClient = GetKubernetesClient();
                var workerPodName = await CreatePod( k8sClient, id );
                return Ok( workerPodName );
            }
            catch( Exception ex )
            {
                return Problem( ex.Message + ex.StackTrace );
            }

        }


        [HttpGet]
        [Route("/name")]
        public string GetName( [FromQuery] string id )
        {
            try
            {

                //var workerPodUrl = GetPodURLFromMapping( id );
                var url = "http://worker-api-" + id + ".worker-subdomain.default.svc.cluster.local";
                return RedirectRequest( url, id );
            }catch(Exception ex )
            {
                return ex.Message+ex.StackTrace;
            }
        }

        [HttpGet]
        [Route( "/health" )]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        private async Task<string> CreatePod( IKubernetes client, string id )
        {
            var pod = new V1Pod()
            {
                Metadata = new V1ObjectMeta
                {
                    Name = "worker-api-pod" + id,
                },
                Spec = new V1PodSpec
                {
                    Containers = new List<V1Container>
                {
                    new V1Container
                    {
                        Name = "worker-api-container",
                        Image = "luangxiao/worker-api:v1",
                        Ports = new []{ new V1ContainerPort { ContainerPort = 80 } },
                        Resources = new V1ResourceRequirements
                        {
                            Limits = new Dictionary<string, ResourceQuantity>
                            {
                                {"cpu", new ResourceQuantity("0.5")}, // 0.5 CPU cores
                                {"memory", new ResourceQuantity("512Mi")}, // 512 Megabytes of memory
                            },
                            Requests = new Dictionary<string, ResourceQuantity>
                            {
                                {"cpu", new ResourceQuantity("0.2")}, // 0.2 CPU cores
                                {"memory", new ResourceQuantity("256Mi")}, // 256 Megabytes of memory
                            },
                        },
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
                return $"Error creating pod for API B: {ex.StackTrace}";
            }
        }

        private string RedirectRequest( string podUrl, string id )
        {
            //Http request
            return HttpHelper.InvokeApi( podUrl + "/name?id=" + id );
        }
        private IKubernetes GetKubernetesClient()
        {
            var config = KubernetesClientConfiguration.InClusterConfig();
            return new Kubernetes( config );

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