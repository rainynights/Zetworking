using System.Net;

namespace Zetworking;

public static class Extensions
{
    public static (string ipAddress, int port) ParseEndPoint(this EndPoint? self)
    {
        if (self is null)
            return default;
        
        string? endPoint = self.ToString();
        if (string.IsNullOrEmpty(endPoint))
            return default;

        string[] sections = endPoint.Split(':', 2, StringSplitOptions.TrimEntries);
        if (sections.Length != 2)
            return default;

        if (!int.TryParse(sections[1], out int parsedPort))
            return default;

        return (ipAddress: sections[0], port: parsedPort);
    }
}
