using System;
using dngrep.core.Extensions.Nullable;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration.States.MethodBodyMembers
{
    [Collection(nameof(StateTransitionFixture))]
    public class LockStatementTransitionTests : StateTransitionTestBase
    {
        protected override SyntaxTree SyntaxTree { get; }

        public LockStatementTransitionTests(StateTransitionFixture fixture)
        {
            this.SyntaxTree = fixture.SyntaxTree;
        }

        private static readonly Func<StatementSyntax, bool> BaseStatementPredicate
            = x => x is LockStatementSyntax @lock
                    && @lock.Expression?.ToString() == "this"
                    && x.ParentAs<LabeledStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method8");

        [Fact]
        public void ContextSelection_LockKeyword_StatementState()
        {
            AssertTransition<StatementSyntax, StatementState>(
                ActionKind.JumpPrev,
                BaseStatementPredicate,
                null,
                null,
                x =>
                {
                    Assert.IsType<StatementState>(x.State);
                    Assert.Equal(
                        "this",
                        x.State.ActiveNodeAs<LockStatementSyntax>().Expression?.ToString());
                });
        }

        [Fact]
        public void JumpNext_MethodBodyBaseStatement_NextBaseStatement()
        {
            AssertTransition<StatementState>(
                ActionKind.JumpNext,
                x => Assert.Equal(
                    "ucheckedStart",
                    x.ActiveBaseNode.As<GotoStatementSyntax>().Expression?.ToString()));
        }

        [Fact]
        public void JumpNext_MethodBodyBaseStatement_ChildOfSameMethod()
        {
            AssertTransition<StatementState>(
                ActionKind.JumpNext,
                x => Assert.Equal(
                    "Method8",
                    x.ActiveBaseNode
                        .As<GotoStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>().Identifier.ValueText));
        }

        [Fact]
        public void JumpPrev_MethodBodyBaseStatement_NextBaseStatement()
        {
            AssertTransition<StatementState>(
                ActionKind.JumpPrev,
                x => Assert.Equal(
                    "x += int.MaxValue",
                    x.ActiveBaseNode.As<CheckedStatementSyntax>().Block.Statements
                        .First().As<ExpressionStatementSyntax>().Expression.ToString()));
        }

        [Fact]
        public void JumpPrev_MethodBodyBaseStatement_ChildOfSameMethod()
        {
            AssertTransition<StatementState>(
                ActionKind.JumpPrev,
                x => Assert.Equal(
                    "Method8",
                    x.ActiveBaseNode
                        .As<CheckedStatementSyntax>()
                        .ParentAs<LabeledStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>().Identifier.ValueText));
        }

        [Fact]
        public void JumpNextSibling_MethodBodyBaseStatement_NextSibling()
        {
            AssertTransition<LocalDeclarationState>(
                ActionKind.JumpNextSibling,
                x => Assert.Equal(
                    "int x = 5",
                    x.ActiveBaseNode.Declaration.ToString()));
        }

        [Fact]
        public void JumpPrevSibling_MethodBodyBaseStatement_PrevSibling()
        {
            AssertTransition<StatementState>(
                ActionKind.JumpPrevSibling,
                x => Assert.Equal(
                    "ucheckedStart",
                    x.ActiveBaseNode.As<GotoStatementSyntax>().Expression?.ToString()));
        }

        private void AssertTransition<TExpectedState>(
            ActionKind action,
            Action<TExpectedState> assert)
            where TExpectedState : LocalContextState
        {
            this.AssertTransition(action, BaseStatementPredicate, assert);
        }
    }
}
