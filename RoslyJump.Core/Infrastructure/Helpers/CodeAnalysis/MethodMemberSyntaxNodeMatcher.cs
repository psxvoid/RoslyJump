using System;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis
{
    // TODO: move to dngrep
    public class MethodMemberSyntaxNodeMatcher : ICombinedSyntaxNodeMatcher
    {
        private static readonly MethodMemberSyntaxNodeMatcher instance =
            new MethodMemberSyntaxNodeMatcher();

        private readonly static Type[] NonVirtualMemberTypes = new[]
        {
            typeof(ParameterListSyntax),
        };

        private readonly static Type[] VirtualMemberTypes = new[]
        {
            typeof(BlockSyntax),
            typeof(ExpressionSyntax),
        };

        private readonly static Type[] CombinedMemberTypes = new[]
        {
            typeof(MethodBodyDeclarationSyntax),    // VirtualNode
            typeof(ParameterListSyntax),            // Non-VirtualNode
        };

        private MethodMemberSyntaxNodeMatcher()
        {
        }

        public bool Match(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.GetType();

            if (NonVirtualMemberTypes.Contains(nodeType))
            {
                return true;
            }

            bool isPotenialMethodBody = VirtualMemberTypes.Contains(node.GetType());

            if (isPotenialMethodBody)
            {
                bool canQuery = MethodBodyVirtualQuery.Instance.CanQuery(node);

                if (canQuery)
                {
                    IVirtualSyntaxNode methodBody = MethodBodyVirtualQuery.Instance.Query(node);

                    return this.Match(new CombinedSyntaxNode(methodBody));
                }
            }

            return false;
        }

        public bool Match(CombinedSyntaxNode node)
        {
            if (CombinedMemberTypes.Contains(node.MixedNode.GetType()))
            {
                return true;
            }

            return false;
        }

        public static MethodMemberSyntaxNodeMatcher Instance => instance;
    }
}
