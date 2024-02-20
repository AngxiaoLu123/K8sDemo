using k8s.Models;
using k8s;
using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Controllers
{
    [ApiController]
    public class MasterController : ControllerBase
    {
        public MasterController()
        {
        }

        [HttpPost]
        [Route( "/pod" )]
        public async Task<IActionResult> CreatePod( [FromQuery] string id )
        {
            try
            {
                IKubernetes k8sClient = GetKubernetesClient();
                var newWorkerPodName = await CreatePod( k8sClient, id );
                return Ok( newWorkerPodName );
            }
            catch( Exception ex )
            {
                return Problem( ex.Message + ex.StackTrace );
            }

        }


        [HttpGet]
        [Route( "/name" )]
        public string GetName( [FromQuery] string id )
        {
            try
            {
                var url = "http://worker-api-" + id + ".worker-subdomain.default.svc.cluster.local";
                return RedirectRequest( url, id );
            }
            catch( Exception ex )
            {
                return ex.Message + ex.StackTrace;
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
                    Name = "worker-api-" + id,
                    Labels = new Dictionary<string, string>() { { "subdomain", "worker-subdomain" } }
                },
                Spec = new V1PodSpec
                {
                    Hostname = "worker-api-" + id,
                    Subdomain = "worker-subdomain",
                    Containers = new List<V1Container>
                    {
                        new V1Container
                        {
                            Name = "worker-api-container",
                            Image = "luangxiao/worker-api:v3",
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
                return $"Error creating pod: {ex.Message} {ex.StackTrace}";
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
    }
}