using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers
{
    public class UsingStatementState : MethodBodyMemberStateBase<UsingStatementSyntax>
    {
        public UsingStatementState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return BaseNode.GetContainerNode().ChildNodes()
                .Where(x => x.GetType() == typeof(UsingStatementSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();
        }
    }
}
