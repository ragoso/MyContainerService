using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HttpSocket 
{
    public class UnixHttpClient
    {
        private static UnixDomainSocketEndPoint _endPoint;

        private static async ValueTask<Stream> SocketConnectionAsync(SocketsHttpConnectionContext socketsHttpConnectionContext, CancellationToken cancellationToken)
        {
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            
            await socket.ConnectAsync(_endPoint);
            
            return  new NetworkStream(socket);;
        }

        public static HttpClient CreateHttpClient(string unixSockPath)
        {
            var con = new SocketsHttpHandler();

            con.ConnectCallback = SocketConnectionAsync;
            
            var httpClient = new HttpClient(con);

            httpClient.BaseAddress = new System.Uri("http://hub.docker.io");

            _endPoint = new UnixDomainSocketEndPoint(unixSockPath);

            return httpClient;
        }
    }
}