using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampleTests.Core.Integration.TaxRec;
using SampleTests.Core.Integration.TaxRec.Models;
using SampleTests.Domain.ValueObjects;

namespace SampleTests.Mocks.HttpClients
{
    public class StubTaxRecHttpClient : ITaxRecHttpClient
    {
        public static Guid Failed_ToCreate_TaxDeclaration_Request_Id =
            Guid.Parse("23b36267-63c8-4403-94f8-5c6d496e2a26");

        public Task<string> GetTaxDeclarationHtmlAsString(string tenantId, string clientId, string sagaId, string correlationId,
            Guid complianceId, TaxDeclarationRequest declarationRequest)
        {
            var result = Failed_ToCreate_TaxDeclaration_Request_Id.ToString().Equals(correlationId) ?
                throw new NotImplementedException() :
                "<html><body>test</body></html>";


            return Task.FromResult(result);
        }
    }
}
