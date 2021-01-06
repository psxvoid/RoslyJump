using System;
using dngrep.core.Extensions.Nullable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration.States.MethodBodyMembers
{
    [Collection(nameof(StateTransitionFixture))]
    public class UsingStatementTransitionTests : StateTransitionTestBase
    {
        protected override SyntaxTree SyntaxTree { get; }

        public UsingStatementTransitionTests(StateTransitionFixture fixture)
        {
            this.SyntaxTree = fixture.SyntaxTree;
        }

        private static readonly Func<UsingStatementSyntax, bool> UsingStatementPredicate
            = x => x is UsingStatementSyntax u
                    && u.Declaration?.ToString() == "Stream stream1 = new MemoryStream()"
                    && x.ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method7");

        [Fact]
        public void ContextSelection_UsingStatement_UsingStatement()
        {
            AssertTransition<UsingStatementSyntax, UsingStatementState>(
                ActionKind.JumpPrev,
                UsingStatementPredicate,
                null,
                null,
                x =>
                {
                    Assert.IsType<UsingStatementState>(x.State);
                    Assert.Equal(
                        "Stream stream1 = new MemoryStream()",
                        x.State.ActiveNodeAs<UsingStatementSyntax>()
                            .Declaration.NotNull().ToString());
                });
        }

        [Fact]
        public void JumpNext_MethodBodyUsingStatement_NextUsingStatement()
        {
            AssertTransition<UsingStatementState>(
                ActionKind.JumpNext,
                x => Assert.Equal(
                    "Stream stream3 = new MemoryStream()",
                    x.ActiveBaseNode
                        .Declaration?.ToString()));
        }

        [Fact]
        public void JumpNext_MethodBodyUsingStatement_ChildOfSameMethod()
        {
            AssertTransition<UsingStatementState>(
                ActionKind.JumpNext,
                x => Assert.Equal(
                    "Method7",
                    x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>().Identifier.ValueText));
        }

        [Fact]
        public void JumpPrev_MethodBodyUsingStatement_PrevUsingStatement()
        {
            this.AssertTransition<UsingStatementState>(
                ActionKind.JumpPrev,
                x => Assert.Equal(
                    "Stream stream5 = new MemoryStream()",
                    x.ActiveBaseNode.Declaration?.ToString()));
        }

        [Fact]
        public void JumpPrev_MethodBodyUsingStatement_ChildOfSameMethod()
        {
            this.AssertTransition<UsingStatementState>(
                ActionKind.JumpPrev,
                x => Assert.Equal(
                    "Method7",
                    x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
                        .Identifier.ValueText));
        }

        [Fact]
        public void JumpNextSibling_MethodBodyUsingStatement_NextSibling()
        {
            AssertTransition<LocalDeclarationState>(
                ActionKind.JumpNextSibling,
                x => Assert.Equal(
                    "Stream stream2 = new MemoryStream()",
                    x.ActiveBaseNode.Declaration?.ToString()));
        }

        [Fact]
        public void JumpNextSibling_MethodBodyUsingStatement_SameParent()
        {
            AssertTransition<LocalDeclarationState>(
                ActionKind.JumpNextSibling,
                x => Assert.Equal(
                    "Method7",
                    x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
                        .Identifier.ValueText));
        }

        [Fact]
        public void JumpPrevSibling_MethodBodyUsingStatement_PrevSibling()
        {
            AssertTransition<ExpressionStatementState>(
                ActionKind.JumpPrevSibling,
                x => Assert.Equal(
                    "stream2.Write(buffer)",
                    x.ActiveBaseNode.Expression.ToString()));
        }

        [Fact]
        public void JumpPrevSibling_MethodBodyUsingStatement_SameParent()
        {
            AssertTransition<ExpressionStatementState>(
                ActionKind.JumpPrevSibling,
                x => Assert.Equal(
                    "Method7",
                    x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
                        .Identifier.ValueText));
        }

        [Fact]
        public void JumpContextUp_MethodBodyUsingStatement_UpperContext()
        {
            AssertTransition<MethodBodyState>(
                ActionKind.JumpContextUp,
                x => Assert.Equal(
                    "Method7",
                    x.ActiveBaseNode.ParentAs<MethodDeclarationSyntax>().Identifier.ValueText));
        }

        [Fact]
        public void JumpContextUp_MethodBodyUsingStatement_UppderContextCorrectParent()
        {
            AssertTransition<MethodBodyState>(
                ActionKind.JumpContextUp,
                x => Assert.Equal(
                    "Method7",
                    x.ActiveBaseNode.ParentAs<MethodDeclarationSyntax>().Identifier.ValueText));
        }
        
        [Fact]
        public void JumpContextDown_MethodBodyUsingStatement_LowerContext()
        {
            AssertTransition<UsingStatementState>(
                ActionKind.JumpContextDown,
                x => Assert.Equal(
                    "Method7",
                    x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
                        .Identifier.ValueText));
        }

        [Fact]
        public void JumpContextDown_MethodBodyUsingStatement_LowerContextCorrectParent()
        {
            AssertTransition<UsingStatementState>(
                ActionKind.JumpContextDown,
                x => Assert.Equal(
                    "Method7",
                    x.ActiveBaseNode.ParentAs<BlockSyntax>().ParentAs<MethodDeclarationSyntax>()
                        .Identifier.ValueText));
        }

        private void AssertTransition<TExpectedState>(
            ActionKind action,
            Action<TExpectedState> assert)
            where TExpectedState : LocalContextState
        {
            this.AssertTransition(action, UsingStatementPredicate, assert);
        }
    }
}
