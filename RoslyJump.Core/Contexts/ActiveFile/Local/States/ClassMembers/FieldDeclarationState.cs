﻿using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers
{
    public class FieldDeclarationState : ClassMemberStateBase<FieldDeclarationSyntax>
    {
        public FieldDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            CombinedSyntaxNode[]? nodes = this.BaseNode.Parent
                ?.ChildNodes()
                ?.Where(x => x.GetType() == typeof(FieldDeclarationSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray();

            if (nodes == null || nodes.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(FieldDeclarationState)}.");
            }

            return nodes;
        }
    }
}
