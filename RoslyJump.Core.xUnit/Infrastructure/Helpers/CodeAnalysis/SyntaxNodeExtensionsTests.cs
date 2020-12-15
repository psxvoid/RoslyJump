using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using System.Linq;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.xUnit.Infrastructure.Helpers.CodeAnalysis
{
    public class SyntaxNodeExtensionsTests
    {
        [Fact]
        public void TryGetBody_MethodDeclarationBody_BlockSyntax()
        {
            const string target = "class C { int GetAge() { return 5; } }";

            AssertHasBody<MethodDeclarationSyntax, BlockSyntax>(target);
        }

        [Fact]
        public void TryGetBody_MethodDeclarationExpression_ExpressionSyntax()
        {
            const string target = "class C { int GetAge() => 5; }";

            AssertHasBody<MethodDeclarationSyntax, LiteralExpressionSyntax>(target);
        }

        [Fact]
        public void TryGetBody_ConstructorDeclarationBody_BlockSyntax()
        {
            const string target = "using System; class C { C() { Console.WriteLine(5); } }";

            AssertHasBody<ConstructorDeclarationSyntax, BlockSyntax>(target);
        }

        [Fact]
        public void TryGetBody_ConstructorDeclarationExpression_ExpressionSyntax()
        {
            const string target = "using System; class C { C() => Console.WriteLine(5); }";

            AssertHasBody<ConstructorDeclarationSyntax, InvocationExpressionSyntax>(target);
        }

        [Fact]
        public void TryGetBody_AnonimouseFunctionBody_BlockSyntax()
        {
            const string target = "using System; delegate(string s) { Console.WriteLine(s); };";

            AssertHasBody<AnonymousFunctionExpressionSyntax, BlockSyntax>(target);
        }

        [Fact]
        public void TryGetBody_AnonimouseFunctionExpression_ExpressionSyntax()
        {
            const string target = "using System; (string s) => Console.WriteLine(s);";

            AssertHasBody<AnonymousFunctionExpressionSyntax, InvocationExpressionSyntax>(target);
        }

        [Fact]
        public void TryGetBody_GetterBody_BlockSyntax()
        {
            const string target = "class C { int X { get { return 0; } } }";

            AssertHasBody<AccessorDeclarationSyntax, BlockSyntax>(target);
        }

        [Fact]
        public void TryGetBody_GetterExpression_ExpressionSyntax()
        {
            const string target = "class C { int X { get => 0; }";

            AssertHasBody<AccessorDeclarationSyntax, LiteralExpressionSyntax>(target);
        }

        [Fact]
        public void TryGetBody_SetterBody_BlockSyntax()
        {
            const string target = "class C { int x; int X { set { x = value; } } }";

            AssertHasBody<AccessorDeclarationSyntax, BlockSyntax>(target);
        }

        [Fact]
        public void TryGetBody_SetterExpression_ExpressionSyntax()
        {
            const string target = "class C { int x; int X { set => x = value; } }";

            SyntaxNode node =
                AssertHasBody<AccessorDeclarationSyntax, AssignmentExpressionSyntax>(target);

            // those ones are just to demonstrate that
            // when PropertyDeclarationSyntax's has getter or setter
            // then ExpressionBody property will be always set to null
            Assert.IsType<AccessorListSyntax>(node.Parent);
            Assert.IsType<PropertyDeclarationSyntax>(node?.Parent?.Parent);
            Assert.Null(((PropertyDeclarationSyntax?)(node?.Parent?.Parent))?.ExpressionBody);
        }

        [Fact]
        public void TryGetBody_ReadOnlyPropertyExpression_ExpressionSyntax()
        {
            const string target = "class C { int X => 0; }";

            SyntaxNode node =
                AssertHasBody<ArrowExpressionClauseSyntax, LiteralExpressionSyntax>(target);

            // those ones are just to demonstrate that
            // when PropertyDeclarationSyntax's has no getter and setter (read-only)
            // then AccessorList property will be always set to null
            Assert.IsType<PropertyDeclarationSyntax>(node.Parent);
            Assert.Null(((PropertyDeclarationSyntax?)node.Parent)?.AccessorList);
        }

        private static SyntaxNode AssertHasBody<TNode, TResult>(string targetCode)
            where TNode : SyntaxNode
            where TResult : SyntaxNode
        {
            SyntaxTree? tree = CSharpSyntaxTree.ParseText(targetCode);

            TNode nodeWithBody = tree.GetRoot().ChildNodes()
                .GetNodesOfTypeRecursively<TNode>()
                .First();

            Assert.IsType<TResult>(nodeWithBody.TryGetBody());

            return nodeWithBody;
        }
    }
}
