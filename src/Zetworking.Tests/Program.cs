using Zetworking;

const string ipAddress = "127.0.0.1";
const int port = 1717;

Console.WriteLine("Press\n  `S` for server\n  `C` for client");
ConsoleKey key = Console.ReadKey(intercept: true).Key;
if (key == ConsoleKey.S)
{
    ZetServer server = new();
    server.Start(port);
    server.AcceptNextConnection();
    Console.ReadKey();
    server.Stop();
    Console.ReadKey();
}
else if (key == ConsoleKey.C)
{
    ZetClient client = new();
    client.Connect(ipAddress, port);
    Console.ReadKey();
    client.Disconnect();
    Console.ReadKey();
    client.Connect(ipAddress, port);
    Console.ReadKey();
    client.Disconnect();
    Console.ReadKey();
}