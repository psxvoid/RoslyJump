using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers.Properties;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers;
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
                    && x.ParentAs<ClassDeclarationSyntax>().HasName("C1");

        private static readonly Func<AccessorDeclarationSyntax, bool> BlockGetAccessorPredicate
            = x => x.Parent is AccessorListSyntax accessors
                    && accessors.Parent is PropertyDeclarationSyntax prop
                    && prop.HasName("Prop2GetSetBlockBodyString")
                    && prop.ParentAs<ClassDeclarationSyntax>().HasName("C1")
                    && x.Keyword.ValueText == "get";

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

        [Fact]
        public void JumpNext_PropertyWithGetSetBlockGet_NextAccessor()
        {
            AssertTransition<AccessorDeclarationSyntax, AccessorDeclarationState>(
                ActionKind.JumpNext,
                BlockGetAccessorPredicate,
                x => Assert.Equal(
                    "set",
                    x.ActiveBaseNode.Keyword.ValueText));
        }

        [Fact]
        public void JumpPrev_PropertyWithGetSetBlockGet_PrevAccessor()
        {
            AssertTransition<AccessorDeclarationSyntax, AccessorDeclarationState>(
                ActionKind.JumpPrev,
                BlockGetAccessorPredicate,
                x => Assert.Equal(
                    "set",
                    x.ActiveBaseNode.Keyword.ValueText));
        }

        [Fact]
        public void JumpNextSibling_PropertyWithGetSetBlockGet_SameAccessor()
        {
            AssertTransition<AccessorDeclarationSyntax, AccessorDeclarationState>(
                ActionKind.JumpNextSibling,
                BlockGetAccessorPredicate,
                x => Assert.Equal(
                    "get",
                    x.ActiveBaseNode.Keyword.ValueText));
        }

        [Fact]
        public void JumpPrevSibling_PropertyWithGetSetBlockGet_SameAccessor()
        {
            AssertTransition<AccessorDeclarationSyntax, AccessorDeclarationState>(
                ActionKind.JumpPrevSibling,
                BlockGetAccessorPredicate,
                x => Assert.Equal(
                    "get",
                    x.ActiveBaseNode.Keyword.ValueText));
        }

        [Fact]
        public void JumpUp_PropertyWithGetSetBlockGet_PropertyDeclaration()
        {
            AssertTransition<AccessorDeclarationSyntax, PropertyDeclarationState>(
                ActionKind.JumpContextUp,
                BlockGetAccessorPredicate,
                x => Assert.Equal(
                    "Prop2GetSetBlockBodyString",
                    x.ActiveBaseNode.Identifier.ValueText));
        }

        [Fact]
        public void JumpDown_PropertyWithGetSetBlockGet_FirstNestedStatement()
        {
            AssertTransition<AccessorDeclarationSyntax, ReturnStatementState>(
                ActionKind.JumpContextDown,
                BlockGetAccessorPredicate,
                x => Assert.Equal(
                    "return this.field4string;",
                    x.ActiveBaseNode.ToString()));
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
