﻿using System;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates
{
    public class LocalFunctionStatementState
        : MethodBodyMemberStateBase<LocalFunctionStatementSyntax>
    {
        public LocalFunctionStatementState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }

        protected override CombinedSyntaxNode[] QueryTargetNodesFunc()
        {
            return this.BaseNode.GetContainerNode()
                ?.ChildNodes()
                .Where(x => x.GetType() == typeof(LocalFunctionStatementSyntax))
                .Select(x => new CombinedSyntaxNode(x))
                .ToArray()
                ?? throw new InvalidOperationException(
                    $"Unable to query target nodes for {nameof(LocalFunctionStatementState)}");
        }
    }
}