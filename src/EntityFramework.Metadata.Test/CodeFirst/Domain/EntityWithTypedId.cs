namespace EntityFramework.Metadata.Test.CodeFirst.Domain
{
    public abstract class EntityWithTypedId<T>
    {
        public T Id { get; set; }
    }
}