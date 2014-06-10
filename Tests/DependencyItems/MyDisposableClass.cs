using System;

namespace Tests.DependencyItems
{
    public interface IMyDisposableClass { }

    public class MyDisposableClass : IMyDisposableClass, IDisposable
    {

        private readonly Action _disposeWasCalled;

        public MyDisposableClass(Action disposeWasCalled)
        {
            _disposeWasCalled = disposeWasCalled;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposeWasCalled();
                }
            }
            _disposed = true;
        }
        
    }
}
