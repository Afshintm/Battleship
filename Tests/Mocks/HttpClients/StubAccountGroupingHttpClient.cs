using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SampleTests.Domain.Abstracts;
using SampleTests.Domain.Abstracts.Models;
using SampleTests.Domain.CompanyTaxReturn;
using SampleTests.Domain.ValueObjects;

namespace SampleTests.Mocks.HttpClients
{
    public class StubAccountGroupingHttpClient : IAccountGroupingRolloverHttpClient
    {
        public Task<HttpResponseMessage> Rollover(string authToken, ITaxCompliance thisYearCompliance, ClientIdentity clientIdentity)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Accepted));
        }
    }
}
