using System;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers.Properties
{
    public class AutoPropertyDeclarationState :
        PropertyClassMemberStateBase<PropertyDeclarationSyntax>
    {
        public AutoPropertyDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.MixedNode.GetType() != typeof(AutoPropertyDeclarationSyntax))
            {
                throw new ArgumentException(
                    $"Unsupported base node type for {nameof(AutoPropertyDeclarationState)}");
            }
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            AccessorDeclarationSyntax? firstAccessor = this.BaseNode?.AccessorList?.Accessors
                .FirstOrDefault();

            if (firstAccessor == null)
            {
                throw new ArgumentException(
                    $"Unable to query a child node for {nameof(AutoPropertyDeclarationState)}");
            }

            return new CombinedSyntaxNode(firstAccessor);
        }
    }
}
