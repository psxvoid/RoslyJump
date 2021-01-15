using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers.Properties;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration.States.MethodBodyMembers
{
    [Collection(nameof(StateTransitionFixture))]
    public class PropertyTransitionTests : StateTransitionTestBase
    {
        protected override SyntaxTree SyntaxTree { get; }

        public PropertyTransitionTests(StateTransitionFixture fixture)
        {
            this.SyntaxTree = fixture.SyntaxTree;
        }

        private static readonly Func<PropertyDeclarationSyntax, bool> Predicate
            = x => x is PropertyDeclarationSyntax prop
                    && prop.HasName("Prop2GetSetBlockBodyString")
                    && x.ParentAs<ClassDeclarationSyntax>()
                        .HasName("C1");

        [Fact]
        public void ContextSelection_PropertyDeclaration_PropertyDeclarationState()
        {
            AssertTransition<PropertyDeclarationSyntax, PropertyDeclarationState>(
                ActionKind.JumpPrev,
                Predicate,
                null,
                null,
                x =>
                {
                    Assert.IsType<PropertyDeclarationState>(x.State);
                    Assert.Equal(
                        "Prop2GetSetBlockBodyString",
                        x.State.ActiveNodeAs<PropertyDeclarationSyntax>().Identifier.ValueText);
                });
        }

        [Fact]
        public void JumpNext_PropertyWithGetSetBlock_NextProperty()
        {
            AssertTransition<PropertyDeclarationState>(
                ActionKind.JumpNext,
                x => Assert.Equal(
                    "Prop3GetSetExpressionBodyString",
                    x.ActiveBaseNode.Identifier.ValueText));
        }

        [Fact]
        public void JumpPrev_PropertyWithGetSetBlock_PrevProperty()
        {
            AssertTransition<PropertyDeclarationState>(
                ActionKind.JumpPrev,
                x => Assert.Equal(
                    "Prop1ReadonlyInt",
                    x.ActiveBaseNode.Identifier.ValueText));
        }

        [Fact]
        public void JumpNextSibling_PropertyWithGetSetBlock_NextSibling()
        {
            AssertTransition<FieldDeclarationState>(
                ActionKind.JumpNextSibling,
                x => Assert.Equal(
                    "field1",
                    x.ActiveBaseNode.GetIdentifierName()));
        }

        [Fact]
        public void JumpPrevSibling_PropertyWithGetSetBlock_PrevSibling()
        {
            AssertTransition<IndexerDeclarationState>(
                ActionKind.JumpPrevSibling,
                x => Assert.Equal(
                    "[int i]",
                    x.ActiveBaseNode.ParameterList.ToString()));
        }

        [Fact]
        public void JumpUp_PropertyWithGetSetBlock_ClassDeclaration()
        {
            AssertTransition<ClassDeclarationState>(
                ActionKind.JumpContextUp,
                x => Assert.Equal(
                    "C1",
                    x.ActiveBaseNode.Identifier.ValueText));
        }

        [Fact]
        public void JumpDown_PropertyWithGetSetBlock_FirstAccessor()
        {
            AssertTransition<AccessorDeclarationState>(
                ActionKind.JumpContextDown,
                x => Assert.Equal(
                    "get",
                    x.ActiveBaseNode.Keyword.ValueText));
        }

        private void AssertTransition<TExpectedState>(
            ActionKind action,
            Action<TExpectedState> assert)
            where TExpectedState : LocalContextState
        {
            this.AssertTransition(action, Predicate, assert);
        }
    }
}
