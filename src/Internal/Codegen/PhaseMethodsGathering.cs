using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kk.BusyEcs.Internal
{
    internal class PhaseMethodsGathering
    {
        public static void GatherPhaseMethods(ClassDeclarationSyntax aClass,
            InitialPassResult ctx,
            Dictionary<CsTypeId, List<MethodDeclarationSyntax>> results
        )
        {
            foreach (MemberDeclarationSyntax member in aClass.Members)
            {
                if (member is MethodDeclarationSyntax method)
                {
                    foreach (AttributeListSyntax attributeList in method.AttributeLists)
                    {
                        foreach (AttributeSyntax attribute in attributeList.Attributes)
                        {
                            
                        }
                    }
                }
            }
        }
    }
}