using System.Reflection;
using System.Runtime.CompilerServices;

using Arinc.Spec424.Records;

namespace Arinc.Spec424.Attributes;

/// <summary>
/// Specifies that the property is one-to-many and relationships should be established after parsing.
/// </summary>
/// <param name="recipientType"></param>
/// <param name="linkedType"></param>
/// <param name="propertyName"></param>
internal abstract class ManyAttribute(Type recipientType, Type linkedType, string propertyName) : Attribute
{
    internal Type LinkedType { get; } = linkedType;

    internal PropertyInfo Property { get; } = recipientType.GetProperty(propertyName) ?? throw new Exception("ops");
}

[AttributeUsage(AttributeTargets.Property)]
internal class ManyAttribute<TRecipient, TRecord>([CallerMemberName] string propertyName = "")
    : ManyAttribute(typeof(TRecipient), typeof(TRecord), propertyName) where TRecipient : Record424, IIdentifiable
                                                                       where TRecord : Record424
{ }
