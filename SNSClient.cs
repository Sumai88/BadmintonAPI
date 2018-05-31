using System;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace BadmintonSvc
{
    public class SNSClient
    {
        private AmazonSimpleNotificationServiceClient _client { get; set; }
        
        public SNSClient()
        {
            _client = new AmazonSimpleNotificationServiceClient();
        }

        public string CreatePushEndPoint(string registrationId)
        {
            if (string.IsNullOrEmpty(registrationId))
                throw new Exception("Registration Id is null");

            var resp = _client.CreatePlatformEndpoint(new CreatePlatformEndpointRequest() {PlatformApplicationArn = "", Token=registrationId });
            return resp.EndpointArn;
        }

        public void SendPush(string endPointArn, string message)
        {
            if (string.IsNullOrEmpty(endPointArn))
                throw new Exception("EndPoint ARN is null");
            var pushMessage = new PublishRequest();
            pushMessage.Message = message;
            pushMessage.TargetArn = endPointArn;
            _client.Publish(pushMessage);
        }
    }
}