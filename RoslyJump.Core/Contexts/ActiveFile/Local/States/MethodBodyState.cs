using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.Infrastructure.Helpers.Reflection;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    // TODO: Also support properties lambdas and expressions.
    public class MethodBodyState : LocalContextState
    {
        // those types should have a method body
        private readonly static Type[] SupportedParentTypes = new[] {
            typeof(ConstructorDeclarationSyntax),
            typeof(MethodDeclarationSyntax),
            typeof(DestructorDeclarationSyntax),
        };

        public MethodBodyState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (!IsSupportedContextNode(contextNode.BaseNode))
            {
                throw new ArgumentException("Unsupported node type.", nameof(contextNode));
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            if (this.ActiveNode == null)
            {
                throw new NullReferenceException(
                    "Active node should be set before querying the parent.");
            }

            SyntaxNode parent = this.ActiveNode.Value.BaseNode.GetContainingParent();

            if (!SupportedParentTypes.Contains(parent.GetType()))
            {
                throw new InvalidOperationException("Unsupported parent node type.");
            }

            // any parent can only have a single body
            return new[] { new CombinedSyntaxNode(parent.GetBody()) };
        }

        public static bool IsSupportedContextNode(SyntaxNode? node)
        {
            return node != null && node.Parent != null
                && (node.GetType() == typeof(BlockSyntax)
                    || node.GetType().IsInheritedFromType(typeof(ExpressionSyntax)))
                && SupportedParentTypes.Contains(node.Parent.GetType());
        }
    }
}
