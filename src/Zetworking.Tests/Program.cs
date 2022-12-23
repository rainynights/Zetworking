using Zetworking;

Console.WriteLine("1: Client\n2: Server");
int choosed = int.Parse(Console.ReadLine());
if (choosed == 1)
{
    using var client = new ZetClient();
    client.Connect("127.0.0.1", 1717);
    Console.WriteLine("Connected");
    client.Disconnect();
    Console.WriteLine("Disconnected");
    client.Connect("127.0.0.1", 1717);
    Console.WriteLine("Connected");
    client.Disconnect();
    Console.WriteLine("Disconnected");
}
else
{
    using var server = new ZetServer();
    server.Start(1717);
    Console.ReadLine();
}