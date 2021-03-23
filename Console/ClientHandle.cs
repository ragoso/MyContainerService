using Grpc.Core;

namespace Console
{
    internal abstract class ClientHandle
    {
        private readonly string _token;

        public ClientHandle(string token)
        {
            _token = token;
        }
        protected Metadata GetTokenHeader()
        {
            var headers = new Metadata();

            headers.Add("Authorization", $"Bearer {_token}");

            return headers;
        }
    }
}