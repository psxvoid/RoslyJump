﻿using System;
using System.Linq;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class MethodMemberSiblingState : SiblingStateBase
    {
        public MethodMemberSiblingState(CombinedSyntaxNode baseNode) : base(baseNode)
        {

            if (!MethodMemberParentSyntaxNodeMatcher.Instance.Match(baseNode.BaseNode))
            {
                throw new ArgumentException(
                    "The provided base node is not a method body.",
                    nameof(baseNode));
            }
        }

        protected override CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode root)
        {
            return root.BaseNode
                ?.ChildNodes()
                .Where(x => !(x is TypeSyntax))
                .QueryVirtualAndCombine(MethodBodyVirtualQuery.Instance)
                .Where(MethodMemberSyntaxNodeMatcher.Instance.Match)
                .GroupBy(x => x.MixedNode.GetType())
                .Select(x => x.First())
                .ToArray()
                ?? Array.Empty<CombinedSyntaxNode>();
        }
    }
}
