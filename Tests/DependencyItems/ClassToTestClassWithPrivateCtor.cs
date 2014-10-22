using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DependencyItems
{
    internal interface IClassToTestClassWithPrivateCtor<TInterface>
    {
    }

    internal interface ISetType
    {
        void SetType(Type resolvedType);
    }

    class ClassToTestClassWithPrivateCtor<TInterface> : IClassToTestClassWithPrivateCtor<TInterface>, ISetType
    {
        private Type _classType;

        public void SetType(Type resolvedType)
        {
            _classType = resolvedType;
        }
    }
}
