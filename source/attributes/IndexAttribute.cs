namespace Arinc.Spec424.Attributes;

/// <summary>
/// Specifies the index within an ARINC-424 string.
/// </summary>
/// <remarks>Note that the index are exactly the same as those defined in the specification.</remarks>
internal abstract class IndexAttribute(int index) : Attribute
{
    /// <summary>
    /// Index within an string.
    /// </summary>
    internal int Index { get; } = index - 1;
}
