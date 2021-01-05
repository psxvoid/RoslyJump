using System;
using dngrep.core.Extensions.Nullable;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration.States.MethodBodyMembers
{
    [Collection(nameof(StateTransitionFixture))]
    public class LocalFunctionStatementTransitionTests : StateTransitionTestBase
    {
        protected override SyntaxTree SyntaxTree { get; }

        public LocalFunctionStatementTransitionTests(
            StateTransitionFixture fixture)
        {
            this.SyntaxTree = fixture.SyntaxTree;
        }

        private static readonly Func<LocalFunctionStatementSyntax, bool> TargetPredicate =
            x => x is LocalFunctionStatementSyntax func && func.HasName("fu")
                && x.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                    .HasName("Method1");

        [Fact]
        public void ContextSelection_LocalFunctionReturnType_LocalFunctionStatement()
        {
            this.AssertTransition<PredefinedTypeSyntax, LocalFunctionStatementState>(
                ActionKind.JumpPrev,
                x => x.Parent is LocalFunctionStatementSyntax func && func.HasName("fu")
                    && x.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"),
                null,
                null,
                x =>
                {
                    Assert.IsType<LocalFunctionStatementState>(x.State);
                    Assert.Equal(
                        "fu",
                        x.State.ActiveNodeAs<LocalFunctionStatementSyntax>().Identifier.ValueText);
                });
        }

        [Fact]
        public void ContextSelection_LocalFunctionStatement_LocalFunctionStatement()
        {
            this.AssertTransition<LocalFunctionStatementSyntax, LocalFunctionStatementState>(
                ActionKind.JumpPrev,
                TargetPredicate,
                null,
                null,
                x =>
                {
                    Assert.IsType<LocalFunctionStatementState>(x.State);
                    Assert.Equal(
                        "fu",
                        x.State.ActiveNodeAs<LocalFunctionStatementSyntax>().Identifier.ValueText);
                });
        }

        [Fact]
        public void JumpNext_LocalFunction_NextLocalFunction()
        {
            this.AssertTransition<LocalFunctionStatementState>(
                ActionKind.JumpNext,
                x => Assert.Equal("fuu", x.ActiveBaseNode.Identifier.ValueText));
        }

        [Fact]
        public void JumpNext_LocalFunction_SameParentMethod()
        {
            this.AssertTransition<LocalFunctionStatementState>(
                ActionKind.JumpNext,
                x => x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                    .HasName("Method1"));
        }

        [Fact]
        public void JumpPrev_LocalFunction_PrevLocalFunction()
        {
            this.AssertTransition<LocalFunctionStatementState>(
                ActionKind.JumpPrev,
                x => Assert.Equal("fuu", x.ActiveBaseNode.Identifier.ValueText));
        }

        [Fact]
        public void JumpPrev_LocalFunction_SameParent()
        {
            this.AssertTransition<LocalFunctionStatementState>(
                ActionKind.JumpPrev,
                x => x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                    .HasName("Method1"));
        }

        [Fact]
        public void JumpNextSibling_LocalFunction_NextSibling()
        {
            this.AssertTransition<ExpressionStatementState>(
                ActionKind.JumpNextSibling,
                x => Assert.Equal("y = y + 4", x.ActiveBaseNode.Expression.ToString()));
        }

        [Fact]
        public void JumpNextSibling_LocalFunction_SameParent()
        {
            this.AssertTransition<ExpressionStatementState>(
                ActionKind.JumpNextSibling,
                x => x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                    .HasName("Method1"));
        }

        [Fact]
        public void JumpPrevSibling_LocalFunction_PrevSibling()
        {
            this.AssertTransition<IfStatementState>(
                ActionKind.JumpPrevSibling,
                x => Assert.Equal("x == 12 || y == 11", x.ActiveBaseNode.Condition.ToString()));
        }

        [Fact]
        public void JumpPrevSibling_LocalFunction_SameParent()
        {
            this.AssertTransition<LocalFunctionStatementSyntax, IfStatementState>(
                ActionKind.JumpPrevSibling,
                TargetPredicate,
                x => x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                    .HasName("Method1"));
        }

        [Fact]
        public void JumpUp_LocalFunction_FirstParent()
        {
            this.AssertTransition<IfBodyState>(
                ActionKind.JumpContextUp,
                x => Assert.Equal(
                    "y == 11",
                    x.ActiveBaseNode.ParentAs<IfStatementSyntax>().Condition.ToString()));
        }

        [Fact]
        public void JumpUp_LocalFunction_FirstParentCorrectParrent()
        {
            this.AssertTransition<IfBodyState>(
                ActionKind.JumpContextUp,
                x => Assert.Equal(
                    "Method1",
                    x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>()
                        .NotNull().GetIdentifierName()));
        }

        [Fact]
        public void JumpDown_LocalFunction_LocalFunctionParameterList()
        {
            this.AssertTransition<ParameterListState>(
                ActionKind.JumpContextDown,
                x => Assert.Equal(
                    "fu",
                    x.ActiveBaseNode.ParentAs<LocalFunctionStatementSyntax>()
                        .Identifier.ValueText));
        }

        [Fact]
        public void JumpDown_LocalFunction_LocalFunctionParameterListCorrectParent()
        {
            this.AssertTransition<ParameterListState>(
                ActionKind.JumpContextDown,
                x => Assert.Equal(
                    "Method1",
                    x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .GetIdentifierName()));
        }

        private void AssertTransition<TExpectedState>(
            ActionKind action,
            Action<TExpectedState> assert)
            where TExpectedState : LocalContextState
        {
            this.AssertTransition(action, TargetPredicate, assert);
        }
    }
}
