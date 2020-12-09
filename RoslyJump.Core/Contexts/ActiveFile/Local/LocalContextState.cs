﻿using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Abstractions;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.Local.States;
using RoslyJump.Core.Infrastructure.Helpers.Reflection;

namespace RoslyJump.Core.Contexts.Local
{
    /// <summary>
    /// The state that support jumping between sibling nodes 
    /// of type <see cref="SiblingStateBase"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the sibling context, e.g. class members.
    /// For example, see <see cref="ClassMemberSiblingState"/>.
    /// </typeparam>
    public abstract class LocalContextState<T> : LocalContextState where T : SiblingStateBase
    {
        protected LocalContextState(LocalContext context, CombinedSyntaxNode? contextNode)
            : base(context, contextNode)
        {
            if (!(context.State is LocalContextState<T>))
            {
                this.SiblingState = this.InitSiblingState();
            }
        }

        protected T? SiblingState { get; private set; }

        protected abstract T InitSiblingState();

        protected void SetSiblingStateFrom(T siblingState)
        {
            this.SiblingState = siblingState;
        }

        public override void TransitionTo(CombinedSyntaxNode? node, LocalContext context)
        {
            LocalContextState stateBefore = context.State;

            base.TransitionTo(node, context);

            LocalContextState stateAfter = context.State;

            LocalContextState<T>? before = stateBefore as LocalContextState<T>;
            LocalContextState<T>? after = stateAfter as LocalContextState<T>;

            if (before != null && after != null
                && after.GetType().IsInheritedFromType(typeof(LocalContextState<T>)))
            {
                _ = before.SiblingState ?? throw new NullReferenceException(
                    "The sibling state isn't set for the previous state.");

                // states has the same sibling jump targets
                // we want to preserve them
                after.SetSiblingStateFrom(before.SiblingState);
            }
            else if (after != null)
            {
                // it means we are transitioning to a sibling state
                // but we have to initialize it first
                T newSiblingState = after.InitSiblingState();
                after.SetSiblingStateFrom(newSiblingState);
            }
        }

        public override void QueryTargetNodes()
        {
            base.QueryTargetNodes();
        }

        public override void JumpToNextSiblingContext()
        {
            _ = this.SiblingState ?? throw new NullReferenceException(
                "Sibling state should be initialized before jumping to next sibling.");

            this.SiblingState.QueryTargets();

            if (!this.SiblingState.HasTargets)
            {
                return;
            }

            this.SiblingState.Next();

            CombinedSyntaxNode target = this.SiblingState.Target;

            this.TransitionTo(target, this.Context);

            this.Context.State.QueryTargetNodes();
            this.Context.State.JumpNext();
        }
    }

    public abstract class LocalContextState : ICanJumpNext, ICanJumpPrev
    {
        public LocalContextState(
            LocalContext context,
            CombinedSyntaxNode? contextNode)
        {
            this.Context = context;
            this.ContextNode = contextNode;
        }

        protected LocalContext Context { get; private set; }
        public CombinedSyntaxNode? ContextNode { get; private set; }

        protected CombinedSyntaxNode[] nodes = Array.Empty<CombinedSyntaxNode>();
        protected bool IsJumpTargetNodesSet => !nodes.IsNullOrEmpty();
        protected int JumpTargetIndex = -1;

        public bool IsJumpTargetSet { get; private set; }
        public int JumpTargetStartLine { get; private set; }
        public int JumpTargetEndLine { get; private set; }
        public int JumpTargetStartChar { get; private set; }
        public int JumpTargetEndChar { get; private set; }


        public virtual void QueryTargetNodes()
        {
            this.nodes = this.QueryTargetNodesFunc();
        }

        protected virtual CombinedSyntaxNode[] QueryClassMembersTargets()
            => Array.Empty<CombinedSyntaxNode>();

        public virtual void TransitionTo(CombinedSyntaxNode? node, LocalContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            this.Context = context;
            this.ContextNode = node;

            if (node == null)
            {
                this.Context.State = new InactiveState(context);
                return;
            }

            if (this.nodes.Contains(node.Value))
            {
                // No need to transition to the same context.
                // Also prevents "resetting" the context
                // when focus is shifted to another node.
                return;
            }

            Type nodeType = node.Value.BaseNode.GetType();

            if (nodeType == typeof(ParameterSyntax))
            {
                this.Context.State = new MethodParameterState(context, node.Value);
            }
            else if (nodeType == typeof(MethodDeclarationSyntax))
            {
                this.Context.State = new MethodDeclarationState(context, node.Value);
            }
            else if(nodeType == typeof(FieldDeclarationSyntax))
            {
                this.Context.State = new FieldDeclarationState(context, node.Value);
            }
            else if(nodeType == typeof(PropertyDeclarationSyntax))
            {
                this.Context.State = new PropertyDeclarationState(context, node.Value);
            }
            else
            //else if (nodeType == typeof(BlockSyntax))
            {
                this.Context.State = new InactiveState(context);
                return;
            }
        }

        protected abstract CombinedSyntaxNode[] QueryTargetNodesFunc();
        protected virtual CombinedSyntaxNode? QueryParentContextNode() => null;

        public virtual void JumpNext()
        {
            if (this.IsJumpTargetNodesSet)
            {
                this.JumpTargetIndex++;

                if (this.JumpTargetIndex == nodes.Length)
                {
                    this.JumpTargetIndex = 0;
                }

                CombinedSyntaxNode target = nodes[this.JumpTargetIndex];

                this.SetJumpTarget(target);
            }
        }

        public virtual void JumpPrev()
        {
            if (this.IsJumpTargetNodesSet)
            {
                this.JumpTargetIndex--;

                if (this.JumpTargetIndex < 0)
                {
                    this.JumpTargetIndex = this.nodes.Length - 1;
                }

                CombinedSyntaxNode target = nodes[this.JumpTargetIndex];

                this.SetJumpTarget(target);
            }
        }

        public virtual void JumpContextUp()
        {
            CombinedSyntaxNode? parentContextNode = this.QueryParentContextNode();

            if (parentContextNode == null)
            {
                return;
            }

            this.TransitionTo(parentContextNode, this.Context);
            this.Context.State.QueryTargetNodes();
            this.Context.State.JumpNext();
        }

        public virtual void JumpToNextSiblingContext()
        {
            // do nothing
        }

        private void SetJumpTarget(CombinedSyntaxNode target)
        {
            var (lineStart, lineEnd, charStart, charEnd) = target.BaseNode.GetSourceTextBounds();


            this.JumpTargetStartLine = lineStart;
            this.JumpTargetEndLine = lineEnd;
            this.JumpTargetStartChar = charStart;
            this.JumpTargetEndChar = charEnd;
            this.IsJumpTargetSet = true;
        }
    }
}
