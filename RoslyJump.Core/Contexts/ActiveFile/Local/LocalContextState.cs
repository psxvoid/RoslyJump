using System;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Abstractions;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.Local.States;

namespace RoslyJump.Core.Contexts.Local
{
    public abstract class LocalContextState : ICanJumpNext, ICanJumpPrev
    {
        public LocalContextState(LocalContext context, SyntaxNode contextNode)
        {
            this.Context = context;
            this.ContextNode = contextNode;
        }

        protected LocalContext Context { get; private set; }
        public SyntaxNode ContextNode { get; private set; }

        protected SyntaxNode[] nodes = Array.Empty<SyntaxNode>();
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

        public virtual void TransitionTo(SyntaxNode syntaxNode, LocalContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            this.Context = context;
            this.ContextNode = syntaxNode;

            if (syntaxNode == null)
            {
                this.Context.State = new InactiveState(context);
            }
            else if (syntaxNode.GetType() == typeof(ParameterSyntax))
            {
                this.Context.State = new MethodParameterState(context, syntaxNode);
            }
        }

        protected abstract SyntaxNode[] QueryTargetNodesFunc();

        public virtual void JumpNext()
        {
            if (this.IsJumpTargetNodesSet)
            {
                this.JumpTargetIndex++;

                if (this.JumpTargetIndex == nodes.Length)
                {
                    this.JumpTargetIndex = 0;
                }

                SyntaxNode target = nodes[this.JumpTargetIndex];

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

                SyntaxNode target = nodes[this.JumpTargetIndex];

                this.SetJumpTarget(target);
            }
        }

        private void SetJumpTarget(SyntaxNode target)
        {
            var (lineStart, lineEnd, charStart, charEnd) = target.GetSourceTextBounds();


            this.JumpTargetStartLine = lineStart;
            this.JumpTargetEndLine = lineEnd;
            this.JumpTargetStartChar = charStart;
            this.JumpTargetEndChar = charEnd;
            this.IsJumpTargetSet = true;
        }
    }
}
