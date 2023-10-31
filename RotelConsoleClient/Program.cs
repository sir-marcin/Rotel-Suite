using System;
using System.Threading.Tasks;
using RotelNetworkApi;
using RotelNetworkApi.Communication;

namespace RotelConsoleClient
{
    internal class Program
    {
        /*
         * - BUG: after turning off through Communicator the app can't reconnect
         * - TODO: Introduce error handling for connection interruptions
         * - TODO: Implement Communicator : IDisposable class for encapsulated Rotel controls
         * - TODO: Detect all local Rotel devices in network
         * - TODO: Assign proper config through `model?` response (or notify if no config is available)
         */

        private const string ModelName = "a14";
        
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Rotel client");
            
            var configsManager = new ConfigsManager();
            configsManager.LoadConfigs();
            if (!configsManager.TryGetConfig(ModelName, out var config))
            {
                Console.WriteLine("Device config unavailable");
                return;
            }

            var communicator = CommunicatorProvider.GetCommunicator(CommunicationType.RS232, config);
            var connected = await communicator.Connect();

            try
            {
                while (connected)
                {
                    Console.WriteLine("Available commands");

                    foreach (var kv in config.Commands)
                    {
                        var commandKey = kv.Key;
                        var commandName = kv.Value;

                        Console.WriteLine($"{commandKey} => {commandName}");
                    }

                    var commandCode = Console.ReadLine();

                    if (commandCode == "q")
                    {
                        break;
                    }

                    if (Enum.TryParse(commandCode, out MessageType messageType))
                    {
                        await communicator.SendMessage(messageType);
                    }
                    else
                    {
                        Console.WriteLine(
                            $"No command available for code {commandCode} in current config ({config.DisplayName})");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                ((IDisposable)communicator).Dispose();
                Console.WriteLine("FINISHED");
            }
        }
    }
}