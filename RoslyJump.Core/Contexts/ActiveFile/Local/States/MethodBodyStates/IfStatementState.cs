using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class IfStatementState : LocalContextState<IfStatementSyntax>
    {
        public IfStatementState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.BaseNode.Parent
                ?.ChildNodes()
                .Where(x => x.GetType() == typeof(IfStatementSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(IfStatementState)}");
        }
    }
}
