using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class InterfaceDeclarationState
        : MixedNestableMemberStateBase<InterfaceDeclarationSyntax>
    {
        public InterfaceDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.BaseNode.Parent
                ?.ChildNodes()
                .Where(x => x.GetType() == typeof(InterfaceDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to get target nodes for {nameof(InterfaceDeclarationState)}");
        }
    }
}
