
namespace CapnpC.CSharp.Generator.Model
{
    interface IDefinition
    {
        ulong Id { get;  }
        bool IsGenerated { get; }
        TypeTag Tag { get; }
        IHasNestedDefinitions DeclaringElement { get; }
    }
}
