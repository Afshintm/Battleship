using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SampleTests.Core.Queue
{
    public class SqsClientFactory : ISqsClientFactory
    {
        private readonly ILogger<SqsClientFactory> _logger;
        private readonly IOptions<QueueSettings> _queueSettings;

        public SqsClientFactory(ILogger<SqsClientFactory> logger, IOptions<QueueSettings> queueSettings)
        {
            _logger = logger;
            _queueSettings = queueSettings;
        }

        private static AmazonSQSClient _amazonSqsClient;
        private static DateTime _timeOfCreation = DateTime.UtcNow;


        public ISqsStorageClient CreateSqsClient()
        {
            return new RealStorageClient(GetSqsClientUsingAssumeRole());
        }

        private class RealStorageClient : ISqsStorageClient
        {
            private readonly IAmazonSQS _amazonSqs;

            public RealStorageClient(IAmazonSQS amazonSqs)
            {
                _amazonSqs = amazonSqs;
            }

            public Task<ReceiveMessageResponse> ReceiveMessageAsync(
                ReceiveMessageRequest receiveMessageRequest,
                CancellationToken cancellationToken
            )
            {
                return _amazonSqs.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
            }

            public Task<DeleteMessageResponse> DeleteMessageAsync(
                DeleteMessageRequest deleteMessageRequest,
                CancellationToken cancellationToken
            )
            {
                return _amazonSqs.DeleteMessageAsync(deleteMessageRequest, cancellationToken);
            }

            public Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendRequest)
            {
                return _amazonSqs.SendMessageAsync(sendRequest);
            }
        }

        private IAmazonSQS GetSqsClientUsingAssumeRole()
        {
            try
            {
                if (_amazonSqsClient != null)
                    return _amazonSqsClient;

                _logger.LogInformation(
                    $"Service Url {_queueSettings.Value.ServiceUrl} , Queue Url {_queueSettings.Value.QueueUrl}");

                var amazonSqsConfig = new AmazonSQSConfig
                {
                    ServiceURL = _queueSettings.Value.ServiceUrl
                };

                _amazonSqsClient = new AmazonSQSClient(amazonSqsConfig);

                _logger.LogInformation($"Created the SQS client");

                return _amazonSqsClient;
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to create AmazonSQSClient");
                _logger.LogError(e, e.Message);
                return null;
            }
        }
    }
}
