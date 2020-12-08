using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class ClassMemberStateBase : LocalContextState<ClassMemberSiblingState>
    {
        protected ClassMemberStateBase(LocalContext context, CombinedSyntaxNode? contextNode)
            : base(context, contextNode)
        {
        }

        protected override ClassMemberSiblingState InitSiblingState()
        {
            _ = this.ContextNode ?? throw new InvalidOperationException(
                "The context node should be initialized.");

            ClassDeclarationSyntax? siblingParent = this.ContextNode.Value.BaseNode
                .GetFirstParentOfType<ClassDeclarationSyntax>();

            _ = siblingParent ?? throw new InvalidOperationException(
                "Unable to get the class parent node.");

            return new ClassMemberSiblingState(
                new CombinedSyntaxNode(siblingParent));
        }
    }
}
