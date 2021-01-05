using dngrep.core.Extensions.Nullable;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates;
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

        [Fact]
        public void LocalFunctionReturnType_ContextSelection_LocalFunctionStatement()
        {
            AssertTransition<PredefinedTypeSyntax, LocalFunctionStatementState>(
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
        public void LocalFunction_ContextSelection_LocalFunctionStatement()
        {
            AssertTransition<LocalFunctionStatementSyntax, LocalFunctionStatementState>(
                ActionKind.JumpPrev,
                x => x is LocalFunctionStatementSyntax func && func.HasName("fu")
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
        public void LocalFunction_JumpNext_NextLocalFunction()
        {
            AssertTransition<LocalFunctionStatementSyntax, LocalFunctionStatementState>(
                ActionKind.JumpNext,
                x => x is LocalFunctionStatementSyntax func && func.HasName("fu")
                    && x.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"),
                x => x.ActiveBaseNode.HasName("fuu")
                    && x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"));
        }

        [Fact]
        public void LocalFunction_JumpPrev_PrevLocalFunction()
        {
            AssertTransition<LocalFunctionStatementSyntax, LocalFunctionStatementState>(
                ActionKind.JumpPrev,
                x => x is LocalFunctionStatementSyntax func && func.HasName("fu")
                    && x.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"),
                x => x.ActiveBaseNode.HasName("fuu")
                    && x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"));
        }

        [Fact]
        public void LocalFunction_JumpNextSibling_NextSibling()
        {
            AssertTransition<LocalFunctionStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpNextSibling,
                x => x is LocalFunctionStatementSyntax func && func.HasName("fu")
                    && x.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"),
                x => x.ActiveBaseNode.HasExpression("y = y + 4")
                    && x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"));
        }

        [Fact]
        public void LocalFunction_JumpPrevSibling_PrevSibling()
        {
            AssertTransition<LocalFunctionStatementSyntax, IfStatementState>(
                ActionKind.JumpPrevSibling,
                x => x is LocalFunctionStatementSyntax func && func.HasName("fu")
                    && x.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"),
                x => x.ActiveBaseNode.HasCondition("x == 12 || y == 11")
                    && x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"));
        }

        [Fact]
        public void LocalFunction_JumpUp_FirstParent()
        {
            AssertTransition<LocalFunctionStatementSyntax, IfBodyState>(
                ActionKind.JumpContextUp,
                x => x is LocalFunctionStatementSyntax func && func.HasName("fu")
                    && x.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"),
                x => x.ActiveBaseNode.ParentAs<IfStatementSyntax>().HasCondition("y == 11")
                    && x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"));
        }

        [Fact]
        public void LocalFunction_JumpDown_LocalFunctionParameterList()
        {
            AssertTransition<LocalFunctionStatementSyntax, ParameterListState>(
                ActionKind.JumpContextDown,
                x => x is LocalFunctionStatementSyntax func && func.HasName("fu")
                    && x.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"),
                x => x.ActiveBaseNode.ParentAs<LocalFunctionStatementSyntax>()
                        .HasName("fu")
                    && x.ActiveBaseNode.GetFirstParentOfType<MethodDeclarationSyntax>().NotNull()
                        .HasName("Method1"));
        }
    }
}
