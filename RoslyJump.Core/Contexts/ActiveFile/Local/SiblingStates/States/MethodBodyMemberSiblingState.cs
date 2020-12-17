﻿using System;
using System.Linq;
using dngrep.core.Queries.SyntaxNodeMatchers.Targets;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp;
using RoslyJump.Core.Infrastructure;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States
{
    public class MethodBodyMemberSiblingState : SiblingStateBase
    {
        public MethodBodyMemberSiblingState(CombinedSyntaxNode baseNode) : base(baseNode)
        {
            if (!MethodBodySyntaxNodeMatcher.Instance.Match(baseNode.BaseNode))
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
                .Where(MethodBodyMemberSyntaxNodeMatcher.Instance.Match)
                .GroupBy(x => x.Kind())
                .Select(x => new CombinedSyntaxNode(x.First()))
                .ToArray()
                ?? Array.Empty<CombinedSyntaxNode>();
        }
    }
}