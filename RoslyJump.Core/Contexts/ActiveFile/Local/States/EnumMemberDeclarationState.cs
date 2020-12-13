using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class EnumMemberDeclarationState : LocalContextState<EnumMemberDeclarationSyntax>
    {
        public EnumMemberDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            CombinedSyntaxNode[]? results = this.BaseNode.Parent
                ?.ChildNodes()
                .Where(x => x.GetType() == typeof(EnumMemberDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            if (results == null || results.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    $"Unable to find target nodes for {nameof(EnumMemberDeclarationState)}.");
            }

            return results;
        }
    }
}
