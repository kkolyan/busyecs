using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using V = UnityEngine.Vector3;

namespace Kk.BusyEcs.Internal
{
    internal static class SourceContextFactory
    {
        private delegate void FileCallback(string file);

        internal static CodegenContext Scan(string basePath = "Assets")
        {
            CodegenContext ctx = new CodegenContext();

            InitialPassResult initialPassResult = new InitialPassResult();
            ScanPath(basePath, file =>
            {
                string code = File.ReadAllText(file);
                CompilationUnitSyntax unit = (CompilationUnitSyntax)CSharpSyntaxTree.ParseText(code).GetRoot();
                unit.Accept(new InitialPassWalker(initialPassResult));
            });
            SecondPassResult secondPassResult = new SecondPassResult();
            foreach (ClassDeclarationSyntax aClass in initialPassResult.systems)
            {
                InjectGathering.GatherInjects(aClass, initialPassResult, secondPassResult.injections);
            }


            return ctx;
        }

        private static void ScanPath(string basePath, FileCallback callback)
        {
            if (File.Exists(basePath))
            {
                callback(basePath);
            }
            else
            {
                ScanDir(basePath, callback);
            }
        }

        private static void ScanDir(string dir, FileCallback callback)
        {
            foreach (string subDir in Directory.GetDirectories(dir))
            {
                ScanDir(subDir, callback);
            }

            foreach (string file in Directory.GetFiles(dir))
            {
                callback(file);
            }
        }

        public static CsTypeId ParseType(TypeSyntax type)
        {
            return type switch
            {
                ArrayTypeSyntax arrayTypeSyntax => throw new NotImplementedException(),
                AliasQualifiedNameSyntax aliasQualifiedNameSyntax => throw new NotImplementedException(),
                GenericNameSyntax genericNameSyntax => throw new NotImplementedException(),
                NameSyntax nameSyntax => new CsTypeId(NameSyntaxToString(nameSyntax)),
                NullableTypeSyntax nullableTypeSyntax => throw new NotImplementedException(),
                OmittedTypeArgumentSyntax omittedTypeArgumentSyntax => throw new NotImplementedException(),
                PointerTypeSyntax pointerTypeSyntax => throw new NotImplementedException(),
                PredefinedTypeSyntax predefinedTypeSyntax => throw new NotImplementedException(),
                RefTypeSyntax refTypeSyntax => ParseType(refTypeSyntax.Type),
                TupleTypeSyntax tupleTypeSyntax => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public static string NameSyntaxToString(NameSyntax node)
        {
            return node switch
            {
                AliasQualifiedNameSyntax a => throw new NotImplementedException(),
                GenericNameSyntax g => throw new NotImplementedException(),
                IdentifierNameSyntax i => i.Identifier.Text,
                QualifiedNameSyntax q => NameSyntaxToString(q.Left) + "." + NameSyntaxToString(q.Right),
                SimpleNameSyntax s => s.Identifier.Text,
            };
        }


        public static CsNamespaceId NamespaceFullname(MemberDeclarationSyntax namespaceEmitter)
        {
            List<string> parts = new List<string>();
            SyntaxNode n = namespaceEmitter;
            while (true)
            {
                if (n is ClassDeclarationSyntax c)
                {
                    parts.Add(c.Identifier.Text);
                }
                else if (n is StructDeclarationSyntax s)
                {
                    parts.Add(s.Identifier.Text);
                }
                else if (n is NamespaceDeclarationSyntax ns)
                {
                    parts.Add(NameSyntaxToString(ns.Name));
                }
                else if (n is CompilationUnitSyntax)
                {
                    break;
                }

                n = n.Parent;
            }

            return new CsNamespaceId(string.Join(".", parts));
        }

        public static CsTypeId Of(BaseTypeDeclarationSyntax node)
        {
            return new CsTypeId(node.Identifier.Text, NamespaceFullname((MemberDeclarationSyntax)node.Parent));
        }

        public static CompilationUnitSyntax ResolveCu(SyntaxNode node)
        {
            SyntaxNode n = node;
            while (true)
            {
                if (n == null)
                {
                    return null;
                }
                if (n is CompilationUnitSyntax cu)
                {
                    return cu;
                }

                n = n.Parent;
            }
        }

        public static bool Eq(CompilationUnitSyntax cu, CsTypeId attributeId, NameSyntax name)
        {
            return name is IdentifierNameSyntax idName
                   && idName.Identifier.Text == attributeId.name
                   && cu.Usings.Any(
                       aUsing => aUsing.Alias == null && NameSyntaxToString(aUsing.Name) == attributeId.ns.name
                   );
        }

        public static CsTypeId ResolveTypeId(CompilationUnitSyntax cu, InitialPassResult ctx, NameSyntax name)
        {
            return name switch {
                AliasQualifiedNameSyntax aliasQualifiedNameSyntax => throw new NotImplementedException(),
                GenericNameSyntax genericNameSyntax => throw new NotImplementedException(),
                IdentifierNameSyntax identifierNameSyntax => throw new NotImplementedException(),
                QualifiedNameSyntax qualifiedNameSyntax => throw new NotImplementedException(),
                SimpleNameSyntax simpleNameSyntax => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
            ctx.knownStructs.TryGetValue(new CsSimpleName(NameSyntaxToString(name)))
            cu.Usings.Single(aUsing => aUsing.Alias == null && NameSyntaxToString(aUsing.Name) == ctx)
        }
    }
}