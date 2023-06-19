using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kk.BusyEcs.Internal
{
    public class LambdaRewriter
    {
        public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            throw new NotImplementedException();
        }

        public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax lambda)
        {
            if (lambda.Parent is ArgumentSyntax
                {
                    Parent: ArgumentListSyntax
                    {
                        Parent: MemberAccessExpressionSyntax { Parent: InvocationExpressionSyntax _ } method
                    } argList
                }
               )
            {
                List<CsTypeId> filter = new List<CsTypeId>();
                if (method.Name.Identifier.Text == "Query" && argList.Arguments.Count == 1)
                {
                    for (var i = 0; i < lambda.ParameterList.Parameters.Count; i++)
                    {
                        ParameterSyntax parameter = lambda.ParameterList.Parameters[i];
                        CsTypeId type = SourceContextFactory.ParseType(parameter.Type);
                        if (i == 0 && type.name == "Entity")
                        {
                            continue;
                        }

                        filter.Add(type);
                        _ctx.components.Add(type);
                    }
                }

                var filterKey = string.Join(", ", filter.Select(x => x.name).OrderBy(it => it));
                if (!_ctx.filters.ContainsKey(filterKey))
                {
                    _ctx.filters[filterKey] = filter.OrderBy(x => x.name).ToList();
                }

                if (method.Name.Identifier.Text == "Match")
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}