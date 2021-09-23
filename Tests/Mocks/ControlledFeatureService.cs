using System;
using System.Linq;
using System.Threading.Tasks;
using SampleTests.Domain.Abstracts;
using SampleTests.Domain.Constants;
using SampleTests.Domain.ValueObjects;

namespace SampleTests.Mocks
{
    public static class ControlledFeatureService
    {
        public static IFeatureService OnlyEnableFeatures(params Features[] exceptingFeatures)
        {
            return new StubFeatureService(exceptingFeatures.Contains);
        }

        public static IFeatureService EnableAllFeaturesExcept(params Features[] exceptingFeatures)
        {
            return new StubFeatureService(features => !exceptingFeatures.Contains(features));
        }

        public static IFeatureService EnableFeatureWithConfig(string treatment, string config)
        {
            return StubFeatureService.GetFeatureServiceWithConfig(treatment, config);
        }

        private sealed class StubFeatureService : IFeatureService
        {
            private readonly Func<Features, bool> _shouldAllowService;
            private readonly string _treatment;
            private readonly string _config;

            public StubFeatureService(Func<Features, bool> shouldAllowService) =>
                _shouldAllowService = shouldAllowService;

            private Func<Features, Task<bool>> ShouldAllow => feature =>
                Task.FromResult(_shouldAllowService(feature));

            public Task<bool> IsAllowed(Features feature, ClientIdentity clientIdentity) => ShouldAllow(feature);

            public Task<bool> IsAllowedForComplianceType(
                Features feature,
                ClientIdentity clientIdentity,
                ComplianceTypes complianceType
            ) => ShouldAllow(feature);

            public bool CheckIfReady()
            {
                return true;
            }

            public async Task<(string Treatment, string Config)> GetFeatureWithConfig(Features feature)
            {
                return await Task.FromResult((_treatment, _config));
            }

            private StubFeatureService(string treatment, string config)
            {
                _treatment = treatment;
                _config = config;
            }

            public static IFeatureService GetFeatureServiceWithConfig(string treatment, string config)
            {
                return new StubFeatureService(treatment, config);
            }
        }
    }
}
