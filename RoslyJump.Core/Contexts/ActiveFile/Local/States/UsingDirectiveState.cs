using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class UsingDirectiveState : MixedNestableMemberStateBase<UsingDirectiveSyntax>
    {
        public UsingDirectiveState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                $"The context node is not set for {nameof(UsingDirectiveState)}.");

            CombinedSyntaxNode[]? results = this.BaseNode.Parent
                ?.ChildNodes()
                .Where(x => x.GetType() == typeof(UsingDirectiveSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            if (results == null || results.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(UsingDirectiveState)}.");
            }

            return results;
        }
    }
}
