using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class MethodBodyMemberSiblingState : SiblingStateBase
    {
        public MethodBodyMemberSiblingState(CombinedSyntaxNode baseNode) : base(baseNode)
        {
            Type mixedNodeType = baseNode.MixedNode.GetType();

            if (mixedNodeType != typeof(MethodBodyDeclarationSyntax)
                && mixedNodeType != typeof(NestedBlockSyntax))

            {
                throw new ArgumentException(
                    "The provided node is not a method body nor a nested block.");
            }

            if (!MethodBodySyntaxNodeMatcher.Instance.Match(baseNode.BaseNode)
                && !baseNode.BaseNode.IsContainer())
            {
                throw new ArgumentException(
                    "The provided base node is not a method body.",
                    nameof(baseNode));
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode root)
        {
            return root.BaseNode
                ?.ChildNodes()
                .QueryVirtualAndCombine(NestedBlockVirtualQuery.Instance)
                .Where(MethodBodyMemberSyntaxNodeMatcher.Instance.Match)
                .GroupBy(x => x.MixedNode.GetType())
                .Select(x => x.First())
                .ToArray()
                ?? Array.Empty<CombinedSyntaxNode>();
        }
    }
}
