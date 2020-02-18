using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace EnvironmentInitializer
{
    public abstract class DockerContainer : IDisposable
    {
        protected abstract string ImageName { get; }
        protected abstract string ContainerName { get; }
        protected virtual Func<Task> OnStartContainerAsync => () => Task.CompletedTask;

        private readonly IDockerClient _client;
        
        protected DockerContainer(IDockerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        
        public async Task StartAsync()
        {
            await CreateImageIfNotExistsAsync();

            var container = await CreateContainerIfNotExistsAsync();
            if (container.State == "running")
            {
                Console.WriteLine($"Container '{ContainerName}' is already running.");
                return;
            }
            
            var started = await _client.Containers.StartContainerAsync(ContainerName, new ContainerStartParameters());
            if (!started)
            {
                throw new InvalidOperationException($"Container '{ContainerName}' did not start");
            }

            Console.WriteLine($"Container '{ContainerName}' is ready.");
        }

        private async Task CreateImageIfNotExistsAsync()
        {
            var images = await _client.Images.ListImagesAsync(new ImagesListParameters {MatchName = ImageName});

            if (images.Count == 0)
            {
                Console.WriteLine($"Fetching Docker image '{ImageName}'");

                await _client.Images.CreateImageAsync(
                    new ImagesCreateParameters {FromImage = ImageName, Tag = "latest"},
                    null,
                    new Progress<JSONMessage>());
            }
        }

        public async Task StopAsync()
        {
            await _client.Containers.StopContainerAsync(ContainerName, new ContainerStopParameters());
        }

        public async Task RemoveAsync()
        {
            await _client.Containers.RemoveContainerAsync(ContainerName, new ContainerRemoveParameters { Force = true });
        }

        protected abstract HostConfig GetHostConfig();

        protected abstract Config GetConfig();

        public override string ToString()
        {
            return $"{nameof(ImageName)}: {ImageName}, {nameof(ContainerName)}: {ContainerName}";
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private async Task<ContainerListResponse> CreateContainerIfNotExistsAsync()
        {
            var container = await GetExistingContainerAsync();
            if (container != null)
            {
                return container;
            }

            Console.WriteLine($"Creating container '{ContainerName}' using image '{ImageName}'");

            var hostConfig = GetHostConfig();
            var config = GetConfig();

            await _client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
            {
                Image = ImageName,
                Name = ContainerName,
                HostConfig = hostConfig,
            });
            
            return await GetExistingContainerAsync();
        }

        private async Task<ContainerListResponse> GetExistingContainerAsync()
        {
            var list = await _client.Containers.ListContainersAsync(new ContainersListParameters {All = true});

            var containerListResponse = list.FirstOrDefault(x => x.Names.Contains("/" + ContainerName));
            return containerListResponse;
        }
    }
}