using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Snork.FluentNHibernateTools.Tests
{
    public class OneTestAtATimeFixture : IDisposable
    {
     

        private static readonly object GlobalLock = new object();

        public OneTestAtATimeFixture()
        {
            Monitor.Enter(GlobalLock);
            
        }

        

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Monitor.Exit(GlobalLock);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
