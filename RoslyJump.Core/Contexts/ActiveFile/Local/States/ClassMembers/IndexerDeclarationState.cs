using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers
{
    public class IndexerDeclarationState : ClassMemberStateBase<IndexerDeclarationSyntax>
    {
        public IndexerDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.BaseNode.GetContainingParent()
                .ChildNodes()
                .Where(x => x.GetType() == typeof(IndexerDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            return null;
        }
    }
}
