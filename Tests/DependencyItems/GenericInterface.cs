namespace Tests.DependencyItems
{
    public interface IGenericInterface<T> where T : class
    {
        string GetTypeName();
    }

    public class GenericInterface<T> : IGenericInterface<T> where T : class
    {

        public string GetTypeName()
        {
            return typeof (T).Name;
        }

    }
}
