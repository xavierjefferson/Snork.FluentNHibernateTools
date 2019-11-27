using System;
using System.Collections.Generic;
using System.Linq;

namespace Snork.FluentNHibernateTools
{
    public class SchemaException : Exception
    {
        public SchemaException(string message, IList<Exception> exceptions) : base(message,
            exceptions.FirstOrDefault())
        {
            Exceptions = exceptions.ToList();
        }

        public List<Exception> Exceptions { get; }
    }
}