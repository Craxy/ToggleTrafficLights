// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
  // Old .net hasn't CallerMemberNameAttribute, CallerFilePathAttribute and CallerLineNumberAttribute
  // But (new) compiler understands these attributes
  // Source: https://github.com/dotnet/coreclr
  
  // Source: coreclr/src/mscorlib/shared/System/Runtime/CompilerServices/CallerMemberNameAttribute.cs
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class CallerMemberNameAttribute : Attribute
  {}
  // Source: coreclr/src/mscorlib/shared/System/Runtime/CompilerServices/CallerFilePathAttribute.cs
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class CallerFilePathAttribute : Attribute
  {}
  // Source: coreclr/src/mscorlib/shared/System/Runtime/CompilerServices/CallerLineNumberAttribute.cs
  [AttributeUsage(AttributeTargets.Parameter)]
  public sealed class CallerLineNumberAttribute : Attribute
  {}
}
