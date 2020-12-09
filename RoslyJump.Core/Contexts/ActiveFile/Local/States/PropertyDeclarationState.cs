using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class PropertyDeclarationState : ClassMemberStateBase
    {
        private const string NoNodesErrorMessage =
            "Unable to query nodes for FieldDeclarationState";

        public PropertyDeclarationState(LocalContext context, CombinedSyntaxNode? contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            var nodes = this.ContextNode
                ?.BaseNode
                ?.Parent
                ?.ChildNodes()
                ?.Where(x => x.GetType() == typeof(PropertyDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();
            
            _ = nodes ?? throw new NullReferenceException(NoNodesErrorMessage);

            if (nodes.IsNullOrEmpty())
            {
                throw new InvalidOperationException(NoNodesErrorMessage);
            }

            return nodes;
        }
    }
}
