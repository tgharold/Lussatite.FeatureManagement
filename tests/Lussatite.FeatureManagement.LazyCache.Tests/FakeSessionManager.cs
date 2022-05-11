using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Lussatite.FeatureManagement.LazyCache.Tests
{
    public class FakeSessionManager : ISessionManager
    {
        private readonly IDictionary<string, bool?> _values =
            new ConcurrentDictionary<string, bool?>(StringComparer.OrdinalIgnoreCase);

        public void SetValue(string featureName, bool? value)
        {
            _values[featureName] = value;
        }

        /// <summary>This session manager does not write values back. It is a read-only provider.</summary>
        public Task SetAsync(string featureName, bool enabled)
        {
            return Task.CompletedTask;
        }

        public Task<bool?> GetAsync(string featureName)
        {
            return Task.FromResult(_values.TryGetValue(featureName, out var result) ? result : null);
        }
    }
}
