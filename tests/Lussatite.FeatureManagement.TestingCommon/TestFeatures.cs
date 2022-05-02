using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Lussatite.FeatureManagement.TestingCommon;

public static class TestFeatures
{
    // avoid feature flag names with spaces, Pascal-case is good, sticking to just letters/numbers is good.
    
    [Description("A: some longer explanation goes here")]
    public const string A = nameof(A);

    [Description("Fancy New Interface: New and improved! Now with Brawndo!")]
    public const string FancyNewInterface = nameof(FancyNewInterface);

    public const string RegisteredButNotInAppConfig = nameof(RegisteredButNotInAppConfig);
    public const string RegisteredAndNullInAppConfig = nameof(RegisteredAndNullInAppConfig);
    public const string RegisteredAndTrueInAppConfig = nameof(RegisteredAndTrueInAppConfig);
    public const string RegisteredAndStringTrueInAppConfig = nameof(RegisteredAndStringTrueInAppConfig);
    public const string RegisteredAndFalseInAppConfig = nameof(RegisteredAndFalseInAppConfig);
    public const string RegisteredAndStringFalseInAppConfig = nameof(RegisteredAndStringFalseInAppConfig);
    public const string RegisteredAndGarbageValueInAppConfig = nameof(RegisteredAndGarbageValueInAppConfig);

    public static readonly Lazy<IEnumerable<string>> All = new(() =>
    {
        return typeof(TestFeatures)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue())
            .ToList();
    });
}