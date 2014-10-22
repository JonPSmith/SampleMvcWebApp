using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DependencyItems
{
    class ServiceTypeClass
    {
        private Type _resolvedType;

        public ServiceTypeClass(Type resolvedType)
        {
            this._resolvedType = resolvedType;
        }
    }
}
