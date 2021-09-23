using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using SampleTests.Core.Queue;

namespace SampleTests.Components.Tests
{
    internal sealed class InMemorySqsClientFactory : ISqsClientFactory
    {
        private readonly object _syncLock = new object();
        private readonly MemoryEmulatedSqsClient _memoryEmulatedSqsClient;

        public InMemorySqsClientFactory()
        {
            _memoryEmulatedSqsClient = new MemoryEmulatedSqsClient();
        }

        public ISqsStorageClient CreateSqsClient()
        {
            return new SynchronizedSqsStorageClient(_memoryEmulatedSqsClient, _syncLock);
        }

        private class QueueMessage
        {
            public string Id { get; set; }
            public string ReceiptHandle { get; set; }
            public DateTimeOffset VisibleFrom { get; set; }
            public string Contents { get; set; }
            public int ApproximateReceiveCount { get; set; }
        }

        private sealed class SynchronizedSqsStorageClient : ISqsStorageClient
        {
            private readonly ISqsStorageClient _wrapped;
            private readonly object _syncLock;

            public SynchronizedSqsStorageClient(ISqsStorageClient wrapped, object syncLock)
            {
                _wrapped = wrapped;
                _syncLock = syncLock;
            }

            public Task<ReceiveMessageResponse> ReceiveMessageAsync(
                ReceiveMessageRequest receiveMessageRequest,
                CancellationToken cancellationToken
            )
            {
                lock (_syncLock)
                    return _wrapped.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
            }

            public Task<DeleteMessageResponse> DeleteMessageAsync(
                DeleteMessageRequest deleteMessageRequest,
                CancellationToken cancellationToken
            )
            {
                lock (_syncLock)
                    return _wrapped.DeleteMessageAsync(deleteMessageRequest, cancellationToken);
            }

            public Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendRequest)
            {
                lock (_syncLock)
                    return _wrapped.SendMessageAsync(sendRequest);
            }
        }

        private sealed class MemoryEmulatedSqsClient : ISqsStorageClient
        {

            private readonly Dictionary<string, List<QueueMessage>> _queueMapping = new Dictionary<string, List<QueueMessage>>();
            public MemoryEmulatedSqsClient()
            {
                _queueMapping.Add(Constants.FormsEngineNotificationQueueUrl, new List<QueueMessage>());
                _queueMapping.Add(Constants.ComplianceQueueUrl, new List<QueueMessage>());
                _queueMapping.Add(Constants.TosNotificationQueue, new List<QueueMessage>());
                _queueMapping.Add(Constants.AssetsAdapterNotificationQueue, new List<QueueMessage>());
            }

            public Task<ReceiveMessageResponse> ReceiveMessageAsync(
                ReceiveMessageRequest receiveMessageRequest,
                CancellationToken cancellationToken
            )
            {
                var currentTime = DateTimeOffset.Now;

                var messages = _queueMapping[receiveMessageRequest.QueueUrl]
                    .Where(x => x.VisibleFrom <= currentTime)
                    .Take(receiveMessageRequest.MaxNumberOfMessages)
                    .ToList();

                foreach (var message in messages)
                {
                    message.VisibleFrom = currentTime.AddSeconds(Math.Max(receiveMessageRequest.VisibilityTimeout, 30));
                    message.ReceiptHandle = Guid.NewGuid().ToString();
                    message.ApproximateReceiveCount += 1;
                }

                return Task.FromResult(
                    new ReceiveMessageResponse
                    {
                        HttpStatusCode = HttpStatusCode.OK,
                        Messages = (
                            from m in messages
                            select new Message
                            {
                                MessageId = m.Id,
                                Body = m.Contents,
                                ReceiptHandle = m.ReceiptHandle,
                                Attributes = new Dictionary<string, string>
                                {
                                    {nameof(m.ApproximateReceiveCount), m.ApproximateReceiveCount.ToString()}
                                }
                            }).ToList()
                    }
                );
            }

            public Task<DeleteMessageResponse> DeleteMessageAsync(
                DeleteMessageRequest deleteMessageRequest,
                CancellationToken cancellationToken
            )
            {
                var index = _queueMapping[deleteMessageRequest.QueueUrl]
                    .FindIndex(x => x.ReceiptHandle == deleteMessageRequest.ReceiptHandle);

                if (index >= 0)
                    _queueMapping[deleteMessageRequest.QueueUrl].RemoveAt(index);

                return Task.FromResult(
                    new DeleteMessageResponse()
                    {
                        HttpStatusCode = HttpStatusCode.OK
                    }
                );
            }

            public Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendRequest)
            {
                var queueMessage = new QueueMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    VisibleFrom = DateTimeOffset.Now.AddSeconds(sendRequest.DelaySeconds),
                    Contents = sendRequest.MessageBody
                };

                _queueMapping[sendRequest.QueueUrl]
                    .Add(queueMessage);

                return Task.FromResult(
                    new SendMessageResponse()
                    {
                        MessageId = queueMessage.Id,
                        HttpStatusCode = HttpStatusCode.OK
                    }
                );
            }
        }
    }
}
