namespace SampleTests.Core.Queue
{
    public interface ISqsClientFactory
    {
        ISqsStorageClient CreateSqsClient();
    }
}
