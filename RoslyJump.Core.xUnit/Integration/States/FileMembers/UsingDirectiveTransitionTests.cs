using System;
using dngrep.core.Extensions.Nullable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration.States.FileMembers
{
    [Collection(nameof(StateTransitionFixture))]
    public class UsingDirectiveTransitionTests : StateTransitionTestBase
    {
        protected override SyntaxTree SyntaxTree { get; }

        public UsingDirectiveTransitionTests(StateTransitionFixture fixture)
        {
            SyntaxTree = fixture.SyntaxTree;
        }

        private static readonly Func<UsingDirectiveSyntax, bool> UsingDirectivePredicate
            = x => x is UsingDirectiveSyntax u
                    && u.Name.ToString() == "System"
                    && x.ParentAs<CompilationUnitSyntax>() != null;

        [Fact]
        public void ContextSelection_UsingDirective_UsingDirective()
        {
            AssertTransition<UsingDirectiveSyntax, UsingDirectiveState>(
                ActionKind.JumpPrev,
                UsingDirectivePredicate,
                null,
                null,
                x =>
                {
                    Assert.Equal(
                        "System",
                        x.State.ActiveNodeAs<UsingDirectiveSyntax>().Name.ToString());
                });
        }

        [Fact]
        public void ContextSelection_UsingDirectiveName_UsingDirective()
        {
            AssertTransition<IdentifierNameSyntax, UsingDirectiveState>(
                ActionKind.JumpPrev,
                x => x.Parent is UsingDirectiveSyntax && x.Identifier.ValueText == "System",
                null,
                null,
                x =>
                {
                    Assert.Equal(
                        "System",
                        x.State.ActiveNodeAs<UsingDirectiveSyntax>().Name.ToString());
                });
        }

        [Fact]
        public void JumpNext_UsingDirective_NextUsingDirective()
        {
            AssertTransition<UsingDirectiveState>(
                ActionKind.JumpNext,
                x => Assert.Equal(
                    "System.IO",
                    x.ActiveBaseNode.Name.ToString()));
        }

        [Fact]
        public void JumpPrev_UsingDirective_NextUsingDirective()
        {
            AssertTransition<UsingDirectiveState>(
                ActionKind.JumpPrev,
                x => Assert.Equal(
                    "Xunit",
                    x.ActiveBaseNode.Name.ToString()));
        }

        [Fact]
        public void JumpNextSibling_MethodBodyUsingDirective_NextSiblingState()
        {
            AssertTransition<NamespaceDeclarationState>(
                ActionKind.JumpNextSibling,
                x => Assert.Equal(
                    "RoslyJump.Core.xUnit.Integration.Fixtures",
                    x.ActiveBaseNode.Name.ToString()));
        }

        [Fact]
        public void JumpPrevSibling_MethodBodyUsingDirective_PrevSiblingState()
        {
            AssertTransition<NamespaceDeclarationState>(
                ActionKind.JumpPrevSibling,
                x => Assert.Equal(
                    "RoslyJump.Core.xUnit.Integration.Fixtures",
                    x.ActiveBaseNode.Name.ToString()));
        }

        [Fact]
        public void JumpUp_MethodBodyUsingDirective_CompilationUnit()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            AssertTransition<FileContextState>(ActionKind.JumpContextUp, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void JumpDown_MethodBodyUsingDirective_SameStatment()
        {
            AssertTransition<UsingDirectiveState>(
                ActionKind.JumpContextDown,
                x => Assert.Equal(
                    "System",
                    x.ActiveBaseNode.Name.ToString()));
        }

        //[Fact]
        //public void JumpPrev_MethodBodyUsingDirective_PrevUsingDirective()
        //{
        //    this.AssertTransition<UsingDirectiveState>(
        //        ActionKind.JumpPrev,
        //        x => Assert.Equal(
        //            "Stream stream5 = new MemoryStream()",
        //            x.ActiveBaseNode.Declaration?.ToString()));
        //}

        //[Fact]
        //public void JumpPrev_MethodBodyUsingDirective_ChildOfSameMethod()
        //{
        //    AssertTransition<UsingDirectiveState>(
        //        ActionKind.JumpPrev,
        //        x => Assert.Equal(
        //            "Method7",
        //            x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
        //                .Identifier.ValueText));
        //}

        //[Fact]
        //public void JumpNextSibling_MethodBodyUsingDirective_NextSibling()
        //{
        //    AssertTransition<LocalDeclarationState>(
        //        ActionKind.JumpNextSibling,
        //        x => Assert.Equal(
        //            "Stream stream2 = new MemoryStream()",
        //            x.ActiveBaseNode.Declaration?.ToString()));
        //}

        //[Fact]
        //public void JumpNextSibling_MethodBodyUsingDirective_SameParent()
        //{
        //    AssertTransition<LocalDeclarationState>(
        //        ActionKind.JumpNextSibling,
        //        x => Assert.Equal(
        //            "Method7",
        //            x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
        //                .Identifier.ValueText));
        //}

        //[Fact]
        //public void JumpPrevSibling_MethodBodyUsingDirective_PrevSibling()
        //{
        //    AssertTransition<ExpressionStatementState>(
        //        ActionKind.JumpPrevSibling,
        //        x => Assert.Equal(
        //            "stream2.Write(buffer)",
        //            x.ActiveBaseNode.Expression.ToString()));
        //}

        //[Fact]
        //public void JumpPrevSibling_MethodBodyUsingDirective_SameParent()
        //{
        //    AssertTransition<ExpressionStatementState>(
        //        ActionKind.JumpPrevSibling,
        //        x => Assert.Equal(
        //            "Method7",
        //            x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
        //                .Identifier.ValueText));
        //}

        //[Fact]
        //public void JumpContextUp_MethodBodyUsingDirective_UpperContext()
        //{
        //    AssertTransition<MethodBodyState>(
        //        ActionKind.JumpContextUp,
        //        x => Assert.Equal(
        //            "Method7",
        //            x.ActiveBaseNode.ParentAs<MethodDeclarationSyntax>().Identifier.ValueText));
        //}

        //[Fact]
        //public void JumpContextUp_MethodBodyUsingDirective_UppderContextCorrectParent()
        //{
        //    AssertTransition<MethodBodyState>(
        //        ActionKind.JumpContextUp,
        //        x => Assert.Equal(
        //            "Method7",
        //            x.ActiveBaseNode.ParentAs<MethodDeclarationSyntax>().Identifier.ValueText));
        //}

        //[Fact]
        //public void JumpContextDown_MethodBodyUsingDirective_LowerContext()
        //{
        //    AssertTransition<UsingDirectiveState>(
        //        ActionKind.JumpContextDown,
        //        x => Assert.Equal(
        //            "Method7",
        //            x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
        //                .Identifier.ValueText));
        //}

        //[Fact]
        //public void JumpContextDown_MethodBodyUsingDirective_LowerContextCorrectParent()
        //{
        //    AssertTransition<UsingDirectiveState>(
        //        ActionKind.JumpContextDown,
        //        x => Assert.Equal(
        //            "Method7",
        //            x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
        //                .Identifier.ValueText));
        //}

        private void AssertTransition<TExpectedState>(
            ActionKind action,
            Action<TExpectedState> assert)
            where TExpectedState : LocalContextState
        {
            AssertTransition(action, UsingDirectivePredicate, assert);
        }
    }
}
