using System;
using System.Threading.Tasks;
using RotelNetworkApi;

namespace RotelConsoleClient
{
    internal class Program
    {
        /*
         * - BUG: after turning off through Communicator the app can't reconnect
         * - Introduce error handling for connection interruptions
         * - Implement Communicator : IDisposable class for encapsulated Rotel controls
         * - Detect all local Rotel devices in network
         * - Assign proper config through `model?` response (or notify if no config is available)
         */
        
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Rotel client");
            
            var configsManager = new ConfigsManager();
            configsManager.LoadConfigs();
            if (!configsManager.TryGetConfig("a14", out var config))
            {
                Console.WriteLine("Device config unavailable");
                return;
            }
                
            var communicator = new Communicator(config);
            await communicator.Connect();

            try
            {
                while (true)
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