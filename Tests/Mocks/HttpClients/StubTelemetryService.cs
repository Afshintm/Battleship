using System;
using System.Collections.Generic;
using SampleTests.Domain.Abstracts;
using SampleTests.Domain.ValueObjects;

namespace SampleTests.Mocks.HttpClients
{
    public class StubTelemetryService : ITelemetryService
    {
        public void Send(UserIdentity userIdentity, ClientIdentity clientIdentity, string eventName, IDictionary<string, object> properties) { }
    }
}
