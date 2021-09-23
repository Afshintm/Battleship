using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace SampleTests.Core.Queue
{
    public interface ISqsStorageClient
    {
        Task<ReceiveMessageResponse> ReceiveMessageAsync(
            ReceiveMessageRequest receiveMessageRequest,
            CancellationToken cancellationToken
        );

        Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest deleteMessageRequest, CancellationToken cancellationToken);
        Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendRequest);
    }
}
