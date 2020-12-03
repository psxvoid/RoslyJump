using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Abstractions;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.Local.States;

namespace RoslyJump.Core.Contexts.Local
{
    public abstract class LocalContextState : ICanJumpNext, ICanJumpPrev
    {
        public LocalContextState(LocalContext context, CombinedSyntaxNode? contextNode)
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
            else if (nodeType == typeof(BlockSyntax))
            {
                this.Context.State = new InactiveState(context);
                return;
            }
        }

        protected abstract CombinedSyntaxNode[] QueryTargetNodesFunc();

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
