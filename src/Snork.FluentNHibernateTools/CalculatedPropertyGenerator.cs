using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace Snork.FluentNHibernateTools
{
    public class CalculatedPropertyGenerator<T, TResult> : BaseHqlGeneratorForProperty
    {
        private Expression<Func<T, TResult>> _calculationExp;

        private CalculatedPropertyGenerator()
        {
        } // Private constructor

        public static void Register(ILinqToHqlGeneratorsRegistry registry, Expression<Func<T, TResult>> property,
            Expression<Func<T, TResult>> calculationExp)
        {
            registry.RegisterGenerator(ReflectHelper.GetProperty(property),
                new CalculatedPropertyGenerator<T, TResult> { _calculationExp = calculationExp });
        }

        public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder,
            IHqlExpressionVisitor visitor)
        {
            return visitor.Visit(_calculationExp);
        }
    }
}