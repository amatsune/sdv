namespace DaLion.Common.Integrations;

/// <summary>Handles integration with a given mod.</summary>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
public interface IModIntegration
{
    /// <summary>A human-readable name for the mod.</summary>
    string ModName { get; }

    /// <summary>Whether the mod is available.</summary>
    bool IsLoaded { get; }
}