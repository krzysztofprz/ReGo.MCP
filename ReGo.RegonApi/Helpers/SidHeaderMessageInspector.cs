using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace ReGo.RegonApi.Helpers
{
    internal class SidHeaderMessageInspector(Func<string> sidProvider) : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object? BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var sid = sidProvider();

            if (!string.IsNullOrWhiteSpace(sid))
            {
                if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                    request.Properties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());

                var httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                httpRequest.Headers[nameof(sid)] = sid;
            }

            return null;
        }
    }
}