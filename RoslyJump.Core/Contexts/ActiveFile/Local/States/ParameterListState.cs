using System;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class ParameterListState : MethodMemberStateBase<ParameterListSyntax>
    {
        public ParameterListState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                "The context node isn't set.");

            // this state can only have a single node
            // because a method can only have a single ParamemterListSyntax
            return new[] { this.ContextNode.Value };
        }
    }
}
