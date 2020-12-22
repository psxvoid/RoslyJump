using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class EventClassMemberStateBase<T> : PropOrEventClassMemberStateBase<T>
        where T : SyntaxNode
    {
        public EventClassMemberStateBase(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            SyntaxNode? parent = this.BaseNode.GetContainingParent();

            if (parent == null)
            {
                throw new InvalidOperationException(
                    $"Unable to find a parent for {nameof(EventClassMemberStateBase<T>)}.");
            }

            CombinedSyntaxNode[] targets = parent.ChildNodes()
                .Where(x => x.IsEvent())
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            if (targets.Length <= 0)
            {
                throw new InvalidOperationException(
                    "Unable to query target nodes for " +
                    $"{nameof(PropOrEventClassMemberStateBase<T>)}");
            }

            return targets;
        }
    }
}
