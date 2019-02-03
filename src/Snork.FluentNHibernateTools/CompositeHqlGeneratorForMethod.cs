using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;

namespace Snork.FluentNHibernateTools
{
    public abstract class CompositeHqlGeneratorForMethod : BaseHqlGeneratorForMethod
    {
        private readonly Dictionary<MethodInfo, BuildHqlDelegate> _delegateDictionary;


        public CompositeHqlGeneratorForMethod(params BaseHqlGeneratorForMethod[] baseHqlGeneratorsForMethod)


        {
            var list = (from m in baseHqlGeneratorsForMethod
                from q in m.SupportedMethods
                select new Tuple<MethodInfo, BuildHqlDelegate>(q, m.BuildHql)).ToList();
            _delegateDictionary = list.ToDictionary(i => i.Item1, i => i.Item2);

            SupportedMethods = list.Select(i => i.Item1);
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
            ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            return _delegateDictionary[method](method, targetObject, arguments, treeBuilder, visitor);
        }

        private delegate HqlTreeNode BuildHqlDelegate(MethodInfo method, Expression targetObject,
            ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor);
    }
}