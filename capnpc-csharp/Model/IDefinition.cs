
namespace CapnpC.Model
{
    interface IDefinition
    {
        ulong Id { get;  }
        TypeTag Tag { get; }
        IHasNestedDefinitions DeclaringElement { get; }
    }
}
