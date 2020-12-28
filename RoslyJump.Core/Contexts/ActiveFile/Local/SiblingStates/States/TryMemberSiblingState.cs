using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class TryMemberSiblingState : SiblingStateBase
    {
        public TryMemberSiblingState(CombinedSyntaxNode baseNode) : base(baseNode)
        {
            if (!(baseNode.BaseNode is TryStatementSyntax))
            {
                throw new ArgumentException(
                    "The provided base node is not supported by " +
                    $"{nameof(TryMemberSiblingState)}",
                    nameof(baseNode)); ; ;
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode baseNode)
        {
            CombinedSyntaxNode[] members = baseNode.BaseNode.ChildNodes()
                .Where(x =>
                    x is BlockSyntax
                    || x is FinallyClauseSyntax
                    || x is CatchClauseSyntax)
                .GroupBy(x => x.Kind())
                .Select(x => x.First())
                .QueryVirtualAndCombine(
                    TryBodyVirtualQuery.Instance)
                .ToArray();

            return members;
        }
    }
}
