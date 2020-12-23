using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration
{
    public class StateTransitionTests : IClassFixture<StateTransitionFixture>
    {
        private readonly StateTransitionFixture classFixture;

        public StateTransitionTests(StateTransitionFixture classFixture)
        {
            this.classFixture = classFixture;
        }

        [Fact]
        public void MethodDeclaration_JumpDown_ParameterList()
        {
            AssertTransition<MethodDeclarationSyntax, ParameterListState>(
                ActionKind.JumpContextDown,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false));
        }

        [Fact]
        public void MethodDeclaration_JumpUp_ClassDeclaration()
        {
            AssertTransition<MethodDeclarationSyntax, ClassDeclarationState>(
                ActionKind.JumpContextUp,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false));
        }

        [Fact]
        public void MethodDeclaration_JumpNext_NextMethodDeclaration()
        {
            AssertTransition<MethodDeclarationSyntax, MethodDeclarationState>(
                ActionKind.JumpNext,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<MethodDeclarationSyntax>().HasName("Method2"));
        }

        [Fact]
        public void MethodDeclaration_JumpPrev_PrevMethodDeclaration()
        {
            AssertTransition<MethodDeclarationSyntax, MethodDeclarationState>(
                ActionKind.JumpPrev,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<MethodDeclarationSyntax>().HasName("Method3"));
        }
        
        [Fact]
        public void MethodDeclaration_JumpNextSibling_NextSibling()
        {
            AssertTransition<MethodDeclarationSyntax, PropertyDeclarationState>(
                ActionKind.JumpNextSibling,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<PropertyDeclarationSyntax>().HasName("Prop1"));
        }

        [Fact]
        public void MethodDeclaration_JumpPrevSibling_PrevSibling()
        {
            AssertTransition<MethodDeclarationSyntax, FieldDeclarationState>(
                ActionKind.JumpPrevSibling,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<FieldDeclarationSyntax>().HasName("field1"));
        }

        private enum ActionKind
        {
            JumpNext,
            JumpPrev,
            JumpNextSibling,
            JumpPrevSibling,
            JumpContextUp,
            JumpContextDown,
        }

        private void AssertTransition<TStartPositionNode, TExpectedState>(
            ActionKind action,
            Func<TStartPositionNode, bool>? startNodePredicate = null,
            Func<TExpectedState, bool>? statePredicate = null)
            where TStartPositionNode : SyntaxNode
            where TExpectedState : LocalContextState
        {
            LocalContext context = new LocalContext(this.classFixture.SyntaxTree);

            TStartPositionNode? node = this.classFixture.SyntaxTree.GetRoot().ChildNodes()
                .GetNodesOfTypeRecursively<TStartPositionNode>()
                .Where(x => startNodePredicate == null || startNodePredicate(x))
                .First();

            var (lineStart, lineEnd, charStart, charEnd) = node.GetSourceTextBounds();

            context.TransitionTo(lineStart, charStart);

            switch (action)
            {
                case ActionKind.JumpNext:
                    context.State.JumpNext();
                    break;
                case ActionKind.JumpPrev:
                    context.State.JumpPrev();
                    break;
                case ActionKind.JumpNextSibling:
                    context.State.JumpToNextSiblingContext();
                    break;
                case ActionKind.JumpPrevSibling:
                    context.State.JumpToPrevSiblingContext();
                    break;
                case ActionKind.JumpContextUp:
                    context.State.JumpContextUp();
                    break;
                case ActionKind.JumpContextDown:
                    context.State.JumpContextDown();
                    break;
                default:
                    throw new ArgumentException("Unknown action kind.", nameof(action));
            }

            Assert.IsType<TExpectedState>(context.State);

            if (statePredicate != null)
            {
                Assert.True(statePredicate((TExpectedState)context.State));
            }
        }
    }
}
