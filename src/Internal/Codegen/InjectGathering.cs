using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kk.BusyEcs.Internal
{
    internal static class InjectGathering
    {
        public static void GatherInjects(ClassDeclarationSyntax aClass, InitialPassResult ctx, List<Injection> results)
        {
            foreach (MemberDeclarationSyntax member in aClass.Members)
            {
                if (!(member is FieldDeclarationSyntax field))
                {
                    continue;
                }

                foreach (AttributeListSyntax attributeList in field.AttributeLists)
                {
                    foreach (AttributeSyntax attribute in attributeList.Attributes)
                    {
                        if (!SourceContextFactory.Eq(
                                SourceContextFactory.ResolveCu(aClass),
                                new CsTypeId("Inject", new CsNamespaceId("Kk.BusyEcs")),
                                attribute.Name
                            ))
                        {
                            continue;
                        }

                        ClassDeclarationSyntax declaringClass = (ClassDeclarationSyntax)field.Parent;
                        results.Add(new Injection(
                            new CsTypeId(declaringClass.Identifier.Text,
                                SourceContextFactory.NamespaceFullname((MemberDeclarationSyntax)declaringClass.Parent)),
                            field.Declaration.Variables[0].Identifier.Text,
                            SourceContextFactory.ParseType(field.Declaration.Type)
                        ));
                    }
                }
            }
        }

        public void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            foreach (AttributeListSyntax attributeList in node.AttributeLists)
            {
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    if (_ctx.phases.Contains(new CsTypeId(SourceContextFactory.NameSyntaxToString(attribute.Name)))) { }

                    if (SourceContextFactory.NameSyntaxToString(attribute.Name) == "EcsSystem")
                    {
                        _ctx.systemClasses.Add(new CsTypeId(aClass.Identifier.Text));
                        _ctx.systemsByPhase[]
                    }
                }
            }
        }
    }
}