using dngrep.core.Extensions.Nullable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates;
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

        [Fact]
        public void ContextSelection_UsingStatement_UsingStatement()
        {
            AssertTransition<UsingStatementSyntax, UsingStatementState>(
                ActionKind.JumpPrev,
                x => x is UsingStatementSyntax u
                    && u.Declaration?.ToString() == "Stream stream1 = new MemoryStream()"
                    && x.ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method7"),
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
            AssertTransition<UsingStatementSyntax, UsingStatementState>(
                ActionKind.JumpNext,
                x => x is UsingStatementSyntax u
                    && u.Declaration?.ToString() == "Stream stream1 = new MemoryStream()"
                    && x.ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method7"),
                x => x.ActiveBaseNode
                        .Declaration?.ToString() == "Stream stream3 = new MemoryStream()"
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method7"));
        }

        [Fact]
        public void JumpPrev_MethodBodyUsingStatement_PrevUsingStatement()
        {
            AssertTransition<UsingStatementSyntax, UsingStatementState>(
                ActionKind.JumpPrev,
                x => x is UsingStatementSyntax u
                    && u.Declaration?.ToString() == "Stream stream1 = new MemoryStream()"
                    && x.ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method7"),
                x => x.ActiveBaseNode
                        .Declaration?.ToString() == "Stream stream5 = new MemoryStream()"
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method7"));
        }
    }
}
