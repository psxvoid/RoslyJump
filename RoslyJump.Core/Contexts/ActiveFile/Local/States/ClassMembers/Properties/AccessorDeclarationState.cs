using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers.Properties
{
    public class AccessorDeclarationState : LocalContextState<AccessorDeclarationSyntax>
    {
        protected override int JumpDownCount => 2;

        public AccessorDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            // this.BaseNode.Parent = SyntaxList<AccessorDeclarationSyntax>
            // this.BaseNode.Parent.Parent = AccessorListSyntax
            // this.BaseNode.Parent.Parent.Parent = PropertyDeclarationSyntax
            AccessorListSyntax parent = this.BaseNode?.Parent
             as AccessorListSyntax ?? throw new InvalidOperationException(
                 $"Unable to query the parent node for {nameof(AccessorDeclarationState)}");

            return parent.Accessors.Select(x => new CombinedSyntaxNode(x)).ToArray();
        }

        protected override CombinedSyntaxNode? QueryChildContextNode()
        {
            SyntaxNode? body = this.BaseNode.TryGetBody();

            if (body == null)
            {
                // it means that the property or event declaration
                // doesn't have a defined body, e.g. auto property
                return null;
            }

            return body.QueryVirtualAndCombine(
                MethodBodyVirtualQuery.Instance,
                NestedBlockVirtualQuery.Instance);
        }

        protected override CombinedSyntaxNode? QueryParentContextNode()
        {
            PropertyDeclarationSyntax? propParent =
                this.BaseNode.GetFirstParentOfType<PropertyDeclarationSyntax>();

            EventDeclarationSyntax? eventParent =
                this.BaseNode.GetFirstParentOfType<EventDeclarationSyntax>();

            EventFieldDeclarationSyntax? eventFieldParent =
                this.BaseNode.GetFirstParentOfType<EventFieldDeclarationSyntax>();

            SyntaxNode? parent =
                (SyntaxNode?)propParent ??
                (SyntaxNode?)eventParent ??
                (SyntaxNode?)eventFieldParent ??
                throw new InvalidOperationException(
                    $"Unable to get the parent node for {nameof(AccessorDeclarationState)}.");

            return new CombinedSyntaxNode(parent);
        }
    }
}
