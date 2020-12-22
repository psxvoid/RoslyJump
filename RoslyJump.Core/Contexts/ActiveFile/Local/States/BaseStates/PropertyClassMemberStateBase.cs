using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public class PropertyClassMemberStateBase<T>
        : PropOrEventClassMemberStateBase<PropertyDeclarationSyntax>
        where T : SyntaxNode
    {
        public PropertyClassMemberStateBase(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.BaseNode?.Parent?.ChildNodes()
                .Where(x => x.GetType() == typeof(PropertyDeclarationSyntax))
                .Select(x => x.QueryVirtualAndCombine(
                    AutoPropertyVirtualQuery.Instance,
                    ReadOnlyPropertyVirtualQuery.Instance))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(PropertyClassMemberStateBase<T>)}");
        }
    }
}
