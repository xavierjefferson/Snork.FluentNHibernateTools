using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;

namespace Snork.FluentNHibernateTools
{
    internal class CastHqlGeneratorForMethod : BaseHqlGeneratorForMethod
    {
        public CastHqlGeneratorForMethod()
        {
            SupportedMethods = new[] {ReflectionHelper.GetMethodDefinition(() => ObjectExtensions.Cast<object>(null))};
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
            ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            return (treeBuilder.Cast(visitor.Visit(arguments[0]).AsExpression(), method.ReturnType));
        }
    }
}