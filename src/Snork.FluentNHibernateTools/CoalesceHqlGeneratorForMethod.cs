using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;

namespace Snork.FluentNHibernateTools
{
    internal class CoalesceHqlGeneratorForMethod : BaseHqlGeneratorForMethod
    {
        public CoalesceHqlGeneratorForMethod()
        {
            SupportedMethods = new[]
                {ReflectionHelper.GetMethodDefinition(() => ObjectExtensions.Coalesce<object>(null, null))};
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
            ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            return (treeBuilder.Coalesce(visitor.Visit(arguments[0]).AsExpression(),
                visitor.Visit(arguments[1]).AsExpression()));
        }
    }
}