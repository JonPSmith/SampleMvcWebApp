namespace Tests.DependencyItems
{
    public interface IConstructorParamClass
    {
        int MyInt { get; }
    }

    public class ConstructorParamClass : IConstructorParamClass
    {

        public int MyInt { get; private set; }

        public ConstructorParamClass(int myInt)
        {
            MyInt = myInt;
        }
    }
}
