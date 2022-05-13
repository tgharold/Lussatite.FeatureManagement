using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lussatite.FeatureManagement.AspNetCore.Tests
{
    public static class TestFeatures
    {
        // avoid feature flag names with spaces, Pascal-case is good, sticking to just letters/numbers is good.

        public const string NotInAppConfig = nameof(NotInAppConfig);
        public const string NullInAppConfig = nameof(NullInAppConfig);
        public const string TrueInAppConfig = nameof(TrueInAppConfig);
        public const string StringTrueInAppConfig = nameof(StringTrueInAppConfig);
        public const string FalseInAppConfig = nameof(FalseInAppConfig);
        public const string StringFalseInAppConfig = nameof(StringFalseInAppConfig);
        public const string GarbageValueInAppConfig = nameof(GarbageValueInAppConfig);

        public static readonly Lazy<IEnumerable<string>> All = new Lazy<IEnumerable<string>>(() =>
        {
            return typeof(TestFeatures)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToList();
        });
    }
}
