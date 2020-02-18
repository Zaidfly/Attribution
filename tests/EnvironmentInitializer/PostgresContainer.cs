using System.Collections.Generic;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace EnvironmentInitializer
{
    public class PostgresContainer : DockerContainer
    {
        protected override string ImageName => "registry.youdo.sg/youdo/devdocker/postgres";
        protected override string ContainerName => "postgresint";

        public PostgresContainer(IDockerClient client) : base(client)
        {
        }

        protected override HostConfig GetHostConfig()
        {
            return new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {
                        "5432",
                        new List<PortBinding>
                        {
                            new PortBinding
                            {
                                HostPort = "4848",
                                HostIP = "127.0.0.1"
                            }
                        }
                    },
                },
            };
        }

        protected override Config GetConfig()
        {
            return new Config
            {
                User = "postgres",
                Env = new List<string>
                {
                    "POSTGRES_PASSWORD=postgres",
                    "POSTGRES_DB=postgres",
                    "POSTGRES_USER=postgres"
                },
                ExposedPorts = new Dictionary<string, EmptyStruct> {{"5432", new EmptyStruct()}},
            };
        }
    }
}