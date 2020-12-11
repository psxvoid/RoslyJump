using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class ClassDeclarationState : ClassMemberStateBase
    {
        public ClassDeclarationState(LocalContext context, CombinedSyntaxNode? contextNode)
            : base(context, contextNode)
        {
            if (contextNode == null || contextNode?.BaseNode.GetType() != typeof(ClassDeclarationSyntax))
            {
                throw new InvalidOperationException(
                    "The context node is invalid for the nested class state.");
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            _ = this.ContextNode ?? throw new NullReferenceException(
                "The context node is not set for MethodDeclarationState");

            ClassDeclarationSyntax? parent = this.ContextNode
                .Value.BaseNode.GetFirstParentOfType<ClassDeclarationSyntax>();

            _ = parent ?? throw new InvalidOperationException(
                "Unable to find the parent class of the nested one.");

            CombinedSyntaxNode[] results = parent.ChildNodes()
                .Where(x => x.GetType() == typeof(ClassDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            return results;
        }
    }
}
