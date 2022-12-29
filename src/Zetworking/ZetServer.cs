using System.Data;
using System.Net;
using System.Net.Sockets;

namespace Zetworking;

public class ZetServer
{
    private ServerState _state;
    private int _port;
    private readonly Socket _socket;
    private readonly List<ZetClient> _connectedClients;

    public ServerState State => _state;
    public int Port => _port;

    public ZetServer()
    {
        _state = ServerState.None;
        _port = 0;
        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _connectedClients = new List<ZetClient>();
    }

    public void Start(int port)
    {
        if (_state is not ServerState.None and not ServerState.Stopped)
            throw new InvalidOperationException("Couldn't start server because it's not available or already started");

        _state = ServerState.Starting;
        
        _port = port;
        
        Console.WriteLine($"Trying to start server at port {_port}");

        // try
        // {
            _socket.Bind(new IPEndPoint(IPAddress.Any, _port));
            _socket.Listen();
        // }
        // catch (ArgumentNullException)
        // {
        //     throw new InvalidOperationException("Couldn't start server because provided port wasn't available");
        // }
        // catch
        // {
        //     throw new InvalidOperationException("Coudln't reach the inner socket");
        // }
        
        _state = ServerState.Started;
        Console.WriteLine($"Server successfully started at 0.0.0.0:{_port}");
    }

    public void Stop()
    {
        if (_state is not ServerState.Started)
            throw new InvalidOperationException("Couldn't stop server because it's not started");
        
        Console.WriteLine("Trying to stop server");
        _state = ServerState.Stopping;
        
        _socket.Close();

        _state = ServerState.Stopped;
        Console.WriteLine("Server stopped");
    }

    public void AcceptNextConnection()
    {
        if (_state is not ServerState.Started)
            throw new InvalidOperationException("Can't accept the incoming connection because server isn't started");

        _socket.BeginAccept(IncomingConnectionCallback, null);
    }

    private void IncomingConnectionCallback(IAsyncResult result)
    {
        if (_state is not ServerState.Started)
            return;
        
        Socket incomingSocket = _socket.EndAccept(result);
        _connectedClients.Add(new ZetClient(incomingSocket));
        Console.WriteLine($"Connection established with {incomingSocket.RemoteEndPoint}");
    }
}
