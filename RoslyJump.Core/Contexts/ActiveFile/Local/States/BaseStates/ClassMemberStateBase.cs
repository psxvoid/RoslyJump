﻿using System;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates
{
    public abstract class ClassMemberStateBase<TNode>
        : LocalContextState<TNode, ClassMemberSiblingState>
        where TNode : SyntaxNode
    {
        protected ClassMemberStateBase(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override ClassMemberSiblingState InitSiblingState()
        {
            _ = this.ContextNode ?? throw new InvalidOperationException(
                "The context node should be initialized before initializing the sibling state.");

            ClassDeclarationSyntax? siblingParent = this.BaseNode
                .GetFirstParentOfType<ClassDeclarationSyntax>();

            _ = siblingParent ?? throw new InvalidOperationException(
                "Unable to get the parent ClassDeclarationSyntax node.");

            return new ClassMemberSiblingState(new CombinedSyntaxNode(siblingParent));
        }
    }
}
