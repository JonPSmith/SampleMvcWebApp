namespace Tests.DependencyItems
{
    public interface ISimpleClass
    {
        void DoSomething();
    };

    public class SimpleClass : ISimpleClass
    {
        public void DoSomething()
        {
        }
    }
}
