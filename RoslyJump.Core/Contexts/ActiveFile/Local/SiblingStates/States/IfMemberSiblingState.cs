using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Infrastructure.Helpers.Generics;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class IfMemberSiblingState : SiblingStateBase
    {
        public IfMemberSiblingState(CombinedSyntaxNode baseNode) : base(baseNode)
        {
            if (baseNode.MixedNode.GetType() != typeof(IfStatementSyntax))
            {
                throw new ArgumentException(
                    "The provided base node can not be used for " +
                    $"{nameof(IfMemberSiblingState)}. " +
                    $"The actual mixed node type is {baseNode.MixedNode.GetType()}.",
                    nameof(baseNode));
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode root)
        {
            IfStatementSyntax? ifRoot = root.BaseNode as IfStatementSyntax;

            if (ifRoot == null)
            {
                throw new InvalidOperationException(
                    $"The provided root is not {nameof(IfStatementSyntax)}.");
            }

            var members = new List<SyntaxNode>();

            members.AddIfNotNull(ifRoot.Condition);
            members.AddIfNotNull(ifRoot.Statement);
            members.AddIfNotNull(ifRoot.Else);

            return members.QueryVirtualAndCombine(
                    IfConditionVirtualQuery.Instance,
                    IfBodyVirtualQuery.Instance)
                .ToArray();
        }
    }
}
