using System.Net;
using System.Net.Sockets;

namespace Zetworking;

public enum ServerState
{
    None,
    Starting,
    Started,
    Stopping,
    Stopped,
}

public class ZetServer : IDisposable
{
    private bool _disposed;
    private ServerState _state;
    private int _port;
    private List<ZetClient> _connectedClients;

    private TcpListener _tcpListener;

    public ServerState State => _state;
    public int Port => _port;

    public ZetServer()
    {
        _disposed = false;
        _state = ServerState.None;
        _port = 0;
        _connectedClients = new List<ZetClient>();
    }

    ~ZetServer()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        
        _disposed = true;

        if (disposing)
        {
            lock (_connectedClients)
            {
                foreach (ZetClient client in _connectedClients)
                {
                    client.Dispose();
                }
            }

            _connectedClients.Clear();
        }
    }

    public bool Start(int port)
    {
        if (_state is not ServerState.None and not ServerState.Stopped)
            return false;

        _state = ServerState.Starting;

        _port = port;

        _tcpListener = new TcpListener(IPAddress.Any, port);
        _tcpListener.Start();
        _tcpListener.BeginAcceptTcpClient(IncomingConnectionCallback, null);

        _state = ServerState.Started;
        return true;
    }

    public bool Stop()
    {
        if (_state is not ServerState.Started)
            return false;
        
        _state = ServerState.Stopping;

        _tcpListener.Stop();

        _state = ServerState.Stopped;
        return false;
    }

    private void IncomingConnectionCallback(IAsyncResult result)
    {
        TcpClient client = _tcpListener.EndAcceptTcpClient(result);
        _tcpListener.BeginAcceptTcpClient(IncomingConnectionCallback, null);
        _connectedClients.Add(new ZetClient(client));
        Console.WriteLine($"Connection established with {client.Client.RemoteEndPoint}");
    }
}

public enum ClientState
{
    None,
    Connecting,
    Connected,
    Disconnecting,
    Disconnected
}

public class ZetClient : IDisposable
{
    private bool _disposed;
    private ClientState _state;
    private string _ipAddress;
    private int _port;

    private TcpClient _tcpClient;
    private NetworkStream _networkStream;

    public ClientState State => _state;
    public string IpAddress => _ipAddress;
    public int Port => _port;

    public ZetClient()
    {
        _tcpClient = new TcpClient();
    }

    internal ZetClient(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        _networkStream = _tcpClient.GetStream();
    }

    ~ZetClient()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;

        if (disposing)
        {
            if (_state is ClientState.Connected)
            {
                Disconnect();
            }

            _tcpClient.Dispose();
            _state = ClientState.None;
        }
    }

    public bool Connect(string ipAddress, int port)
    {
        if (_state is not ClientState.None and not ClientState.Disconnected)
            return false;
        
        _state = ClientState.Connecting;

        _ipAddress = ipAddress;
        _port = port;

        _tcpClient.Connect(ipAddress, port);
        _networkStream = _tcpClient.GetStream();

        _state = ClientState.Connected;
        return true;
    }

    public bool Disconnect()
    {
        if (_state is not ClientState.Connected)
            return false;
        
        _state = ClientState.Connecting;

        _tcpClient.Close();
        _networkStream.Close();

        _state = ClientState.Disconnected;
        return true;
    }
}
