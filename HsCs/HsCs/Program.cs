using HsCs;

namespace hscs
{
    class Program
    {
        static async Task Main()
        {
            var client = new BitFlyerWebSocketClient(new Uri("wss://ws.lightstream.bitflyer.com/json-rpc"));
            await client.StartAsync((execution) =>
            {
                Console.WriteLine($"Executed {execution.Side} {execution.Size} BTC at {execution.Price} JPY ({execution.ExecDate})");
            });
        }
    }


}
