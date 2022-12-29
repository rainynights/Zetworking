using System.Net.Sockets;

namespace Zetworking;

public class ZetClient
{
    private ClientState _state;
    private int _port;
    private string _ipAddress;
    private Socket _socket;

    public ClientState State => _state;
    public string IpAddress => _ipAddress;
    public int Port => _port;

    public ZetClient()
    {
        _state = ClientState.None;
        _ipAddress = string.Empty;
        _port = 0;
        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
    }

    internal ZetClient(Socket socket)
    {
        _state = ClientState.Connected;
        (_ipAddress, _port) = socket.RemoteEndPoint.ParseEndPoint();
        _socket = socket;
    }

    public void Connect(string ipAddress, int port)
    {
        if (_state is not ClientState.None and not ClientState.Disconnected)
            throw new InvalidOperationException("Client is already connected or isn't available to connect");
        
        _state = ClientState.Connecting;

        _ipAddress = ipAddress;
        _port = port;
        
        Console.WriteLine($"Trying to connect to {_ipAddress}:{_port}");

        // try
        // {
            _socket.Connect(_ipAddress, _port);
        // }
        // catch (ArgumentNullException)
        // {
        //     throw new InvalidOperationException("Coudln't connect to server because provided host was null");
        // }
        // catch (ArgumentOutOfRangeException)
        // {
        //     throw new InvalidOperationException("Couldn't connect to server beacause provided port was not valid");
        // }
        // catch
        // {
        //     throw new InvalidOperationException("Coudln't reach the inner socket");
        // }

        _state = ClientState.Connected;
        Console.WriteLine($"Connected established with {_ipAddress}:{_port}");
    }

    public void Disconnect()
    {
        if (_state is not ClientState.Connected)
            throw new InvalidOperationException("Coudln't disconnect from server because client isn't connected");
        
        _state = ClientState.Disconnecting;
        Console.WriteLine("Trying to disconnect from server");

        _socket.Shutdown(SocketShutdown.Both);
        _socket.Disconnect(reuseSocket: false);
        
        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        _state = ClientState.Disconnected;
        Console.WriteLine("Disconnected from server");
    }
}
