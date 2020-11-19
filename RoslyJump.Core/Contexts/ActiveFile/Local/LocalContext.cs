using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SourceTextExtensions;
using dngrep.core.Queries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core
{
    public class LocalContext
    {
        private readonly SyntaxTree tree;

        public LocalContext(SyntaxTree tree)
        {
            this.tree = tree;
            this.State = new InactiveState(this);
        }

        public LocalContextState State { get; internal set; }

        public void TransitionTo(int line, int lineChar)
        {
            TextSpan span = this.tree.GetText().GetSingleCharSpan(line, lineChar);

            var query = SyntaxTreeQueryBuilder.From(span);
            var walker = new SyntaxTreeQueryWalker(query);
            walker.Visit(this.tree.GetRoot());

            IReadOnlyCollection<SyntaxNode> results = walker.Results;
            
            if (results.Count <= 0)
            {
                this.State.TransitionTo(null, this);
                return;
            }

            LocalContextState stateBefore = this.State;

            this.State.TransitionTo(results.Last(), this);

            if (stateBefore != this.State || stateBefore.ContextNode != this.State.ContextNode)
            {
                // query nodes
                this.State.QueryTargetNodes();
            }
        }
    }
}
