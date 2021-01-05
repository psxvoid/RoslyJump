using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using RoslyJump.Core.Contexts.Local;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration
{
    public abstract class StateTransitionTestBase
    {
        protected abstract SyntaxTree SyntaxTree { get; }

        protected enum ActionKind
        {
            JumpNext,
            JumpPrev,
            JumpNextSibling,
            JumpPrevSibling,
            JumpContextUp,
            JumpContextDown,
        }

        protected void AssertTransition<TStartPositionNode, TExpectedState>(
            ActionKind action,
            Func<TStartPositionNode, bool>? startNodePredicate = null,
            Func<TExpectedState, bool>? statePredicate = null,
            Func<TStartPositionNode, FileLinePositionSpan>? startPositionFunctor = null,
            Action<LocalContext>? preJumpAction = null)
            where TStartPositionNode : SyntaxNode
            where TExpectedState : LocalContextState
        {
            LocalContext context = new LocalContext(this.SyntaxTree);

            TStartPositionNode? node = this.SyntaxTree.GetRoot().ChildNodes()
                .GetNodesOfTypeRecursively<TStartPositionNode>()
                .Where(x => startNodePredicate == null || startNodePredicate(x))
                .First();

            SyntaxNode startNode = node;

            var (lineStart, lineEnd, charStart, charEnd) = startNode.GetSourceTextBounds();

            if (startPositionFunctor != null)
            {
                FileLinePositionSpan line = startPositionFunctor(node);

                lineStart = line.StartLinePosition.Line;
                charStart = line.StartLinePosition.Character;
            }

            context.TransitionTo(lineStart, charStart);

            preJumpAction?.Invoke(context);

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
