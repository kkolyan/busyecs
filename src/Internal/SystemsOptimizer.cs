using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;

namespace Kk.BusyEcs.Internal
{
    public static class SystemsOptimizer
    {
        public static void GenerateOptimizedSystems(IEnumerable<Assembly> assemblies)
        {
            CodegenContext ctx = SourceContextFactory.Scan("Assets/Code/EcsSystems/RestartGameSystem.cs");
            if (true)
            {
                return;
            }
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText("Assets/Code/EcsSystems/RestartGameSystem.cs"));
            CSharpSyntaxNode root = (CSharpSyntaxNode)tree.GetRoot();
            SyntaxNode optimizedRoot = root.Accept(new OptimizingVisitor(ctx));

            File.WriteAllText("Assets/Code/EcsSystems/RestartGameSystem_Optimized.cs", optimizedRoot.ToFullString());
        }

        private class OptimizingVisitor : CSharpSyntaxRewriter
        {
            private readonly CodegenContext _ctx;

            public OptimizingVisitor(CodegenContext ctx)
            {
                _ctx = ctx;
            }

            public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                if (node.Parent is ClassDeclarationSyntax)
                {
                    return base.VisitClassDeclaration(node);
                }

                SyntaxToken className = SyntaxFactory.Identifier(node.Identifier.Text + "_Optimized");
                ClassDeclarationSyntax classDeclarationSyntax = node.WithIdentifier(className);
                return base.VisitClassDeclaration(classDeclarationSyntax);
            }

            public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
            {
                throw new NotImplementedException("TODO");
            }

            public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
            {
                if (node.Expression is InvocationExpressionSyntax invocation
                    && invocation.Expression is MemberAccessExpressionSyntax method
                    && method.Name.Identifier.Text == "Query"
                    && invocation.ArgumentList.Arguments.Count == 1
                    && invocation.ArgumentList.Arguments[0].Expression is ParenthesizedLambdaExpressionSyntax lambda)
                {
                    Debug.Log(node);
                    string s = "";
                    SyntaxTriviaList indent = method.GetLeadingTrivia();
                    s += $"{indent}/*\n";
                    s += $"{indent}{lambda.ParameterList}\n";
                    s += $"{indent}*/\n";
                    s += $"{indent}{{\n";
                    s += $"{indent}var env__ = {method.Expression.ToString()};\n";
                    for (var wi = 0; wi < _ctx.worlds.Count; wi++)
                    {
                        List<ParamType> paramTypes = lambda.ParameterList.Parameters
                            .Select(it => ParseParamType(it.Type))
                            .ToList();

                        List<string> components;
                        bool supplyEntity;
                        if (paramTypes[0].name == "Entity")
                        {
                            supplyEntity = true;
                            components = paramTypes.Skip(1).Select(it => it.name).ToList();
                        }
                        else
                        {
                            supplyEntity = false;
                            components = paramTypes.Select(it => it.name).ToList();
                        }

                        s += $"{indent}foreach (int entity__ in env__.{EcsContainerSourcesGenerator.FilterName(_ctx.worlds[wi], components)}) {{\n";
                        for (var i = 0; i < lambda.ParameterList.Parameters.Count; i++)
                        {
                            ParameterSyntax parameter = lambda.ParameterList.Parameters[i];
                            ParamType paramType = paramTypes[i];
                            string varName = parameter.Identifier.Text;
                            if (varName.Replace("_", "") == "")
                            {
                                continue;
                            }

                            if (i == 0 && paramType.name == "Entity")
                            {
                                s += $"{indent}Entity {varName} = new Entity({wi}, entity__);\n";
                            }
                            else
                            {
                                // string @ref = (paramType.type == ParamType.Type.Ref ? "ref " : "");
                                string poolVar = EcsContainerSourcesGenerator.PoolVar(_ctx.worlds[wi], paramType.name);
                                s += $"{indent}ref {paramType} {varName} = ref env__.{poolVar}.Get(entity__);\n";
                            }
                        }
                        s += $"{indent}{lambda.Body.ToString()}\n";
                        s += $"{indent}}}\n";
                    }

                    s += $"{indent}}}\n";
                    s += $"{indent}\n";

                    return SyntaxFactory.ParseStatement(s).Accept(this);
                }

                return base.VisitExpressionStatement(node);
            }

            private static ParamType ParseParamType(TypeSyntax ast)
            {
                return ast switch
                {
                    SimpleNameSyntax simpleType => new ParamType(simpleType.Identifier.Text, ParamType.Type.Simple),
                    RefTypeSyntax refType => new ParamType(((SimpleNameSyntax)refType.Type).Identifier.Text, refType.RefKeyword.Text switch
                    {
                        "ref" => ParamType.Type.Ref,
                        "in" => ParamType.Type.In,
                        _ => throw new ArgumentOutOfRangeException("invalid type: " + ast.ToFullString())
                    }),
                    _ => throw new ArgumentOutOfRangeException("invalid type: " + ast.ToFullString())
                };
            }

            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                if (ParseParamType(node.Declaration.Type).name == nameof(IEnv))
                {
                    FieldDeclarationSyntax converted = node.WithDeclaration(
                        node.Declaration.WithType(
                            SyntaxFactory.ParseTypeName(CodeGenConstants.GeneratedEcsContainerClassName).WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia(" "))
                        )
                    );
                    SyntaxNode convertedByBase = base.VisitFieldDeclaration(converted);
                    return convertedByBase;
                }

                return base.VisitFieldDeclaration(node);
            }

            public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
            {
                throw new NotImplementedException("TODO");
            }
        }
    }
}