using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HttpSocket 
{
    public class UnixHttpClient
    {
        public HttpClient HttpClient { get; }
        private UnixDomainSocketEndPoint EndPoint { get; }
        public UnixHttpClient(string unixSockDir)
        {
            var con = new SocketsHttpHandler();

            con.ConnectCallback = SocketConnectionAsync;
            
            HttpClient = new HttpClient(con);

            HttpClient.BaseAddress = new System.Uri("http://hub.docker.io");

            EndPoint = new UnixDomainSocketEndPoint(unixSockDir);
            
        }
        private async ValueTask<Stream> SocketConnectionAsync(SocketsHttpConnectionContext socketsHttpConnectionContext, CancellationToken cancellationToken)
        {
            var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
            
            await socket.ConnectAsync(EndPoint);
            using var stream = new NetworkStream(socket);
            return  stream;
        }
    }
}