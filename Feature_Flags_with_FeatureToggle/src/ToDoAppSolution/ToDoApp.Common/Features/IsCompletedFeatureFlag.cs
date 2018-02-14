namespace ToDoApp.Common.Features
{
    using FeatureToggle;

    /// <summary>
    /// IsCompletedFeatureFlag gets if Todo Completed feature is enabled.
    /// </summary>
    /// <seealso cref="FeatureToggle.SimpleFeatureToggle" />
    public class IsCompletedFeatureFlag : SqlFeatureToggle { }
}