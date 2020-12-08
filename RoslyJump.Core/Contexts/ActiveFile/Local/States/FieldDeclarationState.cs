using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class FieldDeclarationState : ClassMemberStateBase
    {
        public FieldDeclarationState(LocalContext context, CombinedSyntaxNode? contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            var nodes = this.ContextNode
                ?.BaseNode
                ?.Parent
                ?.ChildNodes()
                ?.Where(x => x.GetType() == typeof(FieldDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            return nodes ?? throw new InvalidOperationException(
                "Unable to query nodes for FieldDeclarationState");
        }
    }
}
