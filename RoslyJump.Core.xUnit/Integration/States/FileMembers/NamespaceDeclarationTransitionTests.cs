using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration.States.FileMembers
{
    [Collection(nameof(StateTransitionFixture))]
    public class NamespaceDeclarationTransitionTests : StateTransitionTestBase
    {
        protected override SyntaxTree SyntaxTree { get; }

        public NamespaceDeclarationTransitionTests(StateTransitionFixture fixture)
        {
            SyntaxTree = fixture.SyntaxTree;
        }

        private static readonly Func<NamespaceDeclarationSyntax, bool> NamespaceDeclarationPredicate
            = x => x is NamespaceDeclarationSyntax u
                    && u.Name.ToString() == "RoslyJump.Core.xUnit.Integration.Fixtures"
                    && x.ParentAs<CompilationUnitSyntax>() != null;

        [Fact]
        public void ContextSelection_NamespaceDeclaration_NamespaceDeclaration()
        {
            AssertTransition<NamespaceDeclarationSyntax, NamespaceDeclarationState>(
                ActionKind.JumpPrev,
                NamespaceDeclarationPredicate,
                null,
                null,
                x =>
                {
                    Assert.Equal(
                        "RoslyJump.Core.xUnit.Integration.Fixtures",
                        x.State.ActiveNodeAs<NamespaceDeclarationSyntax>().Name.ToString());
                });
        }

        [Fact]
        public void ContextSelection_NamespaceDeclarationName_NamespaceDeclaration()
        {
            AssertTransition<IdentifierNameSyntax, NamespaceDeclarationState>(
                ActionKind.JumpPrev,
                x => x.GetFirstParentOfType<NamespaceDeclarationSyntax>() != null
                    && x.Identifier.ValueText == "xUnit",
                null,
                null,
                x =>
                {
                    Assert.Equal(
                        "RoslyJump.Core.xUnit.Integration.Fixtures",
                        x.State.ActiveNodeAs<NamespaceDeclarationSyntax>().Name.ToString());
                });
        }

        [Fact]
        public void JumpNext_NamespaceDeclaration_NextNamespaceDeclaration()
        {
            AssertTransition<NamespaceDeclarationState>(
                ActionKind.JumpNext,
                x => Assert.Equal(
                    "DummyNamespace1",
                    x.ActiveBaseNode.Name.ToString()));
        }

        [Fact]
        public void JumpPrev_NamespaceDeclaration_NextNamespaceDeclaration()
        {
            AssertTransition<NamespaceDeclarationState>(
                ActionKind.JumpPrev,
                x => Assert.Equal(
                    "DummyNamespace2",
                    x.ActiveBaseNode.Name.ToString()));
        }

        [Fact]
        public void JumpNextSibling_NamespaceDeclaration_NextSiblingState()
        {
            AssertTransition<UsingDirectiveState>(
                ActionKind.JumpNextSibling,
                x => Assert.Equal(
                    "System",
                    x.ActiveBaseNode.Name.ToString()));
        }

        [Fact]
        public void JumpPrevSibling_NamespaceDeclaration_PrevSiblingState()
        {
            AssertTransition<UsingDirectiveState>(
                ActionKind.JumpPrevSibling,
                x => Assert.Equal(
                    "System",
                    x.ActiveBaseNode.Name.ToString()));
        }

        [Fact]
        public void JumpUp_NamespaceDeclaration_CompilationUnit()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            AssertTransition<FileContextState>(ActionKind.JumpContextUp, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void JumpDown_NamespaceDeclaration_FirstMember()
        {
            AssertTransition<ClassDeclarationState>(
                ActionKind.JumpContextDown,
                x => Assert.Equal(
                    "StateTransitionXUnitCollection",
                    x.ActiveBaseNode.Identifier.ValueText));
        }

        private void AssertTransition<TExpectedState>(
            ActionKind action,
            Action<TExpectedState> assert)
            where TExpectedState : LocalContextState
        {
            AssertTransition(action, NamespaceDeclarationPredicate, assert);
        }
    }
}
