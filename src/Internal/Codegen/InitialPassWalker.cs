using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kk.BusyEcs.Internal
{
    internal class InitialPassWalker : CSharpSyntaxWalker
    {
        private readonly InitialPassResult _result;
        private CompilationUnitSyntax _cu;

        public InitialPassWalker(InitialPassResult result)
        {
            _result = result;
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            _cu = node;
            base.VisitCompilationUnit(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // do not go deeper
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax aClass)
        {
            foreach (AttributeListSyntax attributeList in aClass.AttributeLists)
            {
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    if (SourceContextFactory.Eq(_cu, new CsTypeId("EcsSystem", new CsNamespaceId("Kk.BusyEcs")), attribute.Name))
                    {
                        _result.systems.Add(aClass);
                    }
                }
            }
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            CsSimpleName simpleName = new CsSimpleName(node.Identifier.Text);
            if (!_result.knownStructs.TryGetValue(simpleName, out var namesakes))
            {
                _result.knownStructs[simpleName] = namesakes = new List<CsTypeId>();
            }

            namesakes.Add(SourceContextFactory.Of(node));


            foreach (AttributeListSyntax attributeList in node.AttributeLists)
            {
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    if (SourceContextFactory.Eq(_cu, new CsTypeId("EcsWorld", new CsNamespaceId("Kk.BusyEcs")), attribute.Name))
                    {
                        LiteralExpressionSyntax expressionSyntax = (LiteralExpressionSyntax)attribute.ArgumentList.Arguments[0].Expression;
                        string worldName = (string)expressionSyntax.Token.Value;
                        _result.extraWorldNames.Add(worldName);
                        _result.worldNameByComponent[SourceContextFactory.Of(node)] = worldName;
                    }

                    if (SourceContextFactory.Eq(_cu, new CsTypeId("EcsPhase", new CsNamespaceId("Kk.BusyEcs")), attribute.Name))
                    {
                        _result.phases.Add(SourceContextFactory.Of(node));
                    }
                }
            }
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            // do not go deeper
        }
    }
}