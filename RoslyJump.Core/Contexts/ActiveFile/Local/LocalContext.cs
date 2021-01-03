using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SourceTextExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxWalkers;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core
{
    public class LocalContext
    {
        private readonly SyntaxTree tree;

        internal SyntaxTree SyntaxTree => this.tree;

        private readonly static HashSet<Type> SupportedNodeTypes =
            new HashSet<Type>
        {
            typeof(CompilationUnitSyntax),
            typeof(NamespaceDeclarationSyntax),
            typeof(UsingDirectiveSyntax),
            typeof(EnumDeclarationSyntax),
            typeof(EnumMemberDeclarationSyntax),
            typeof(StructDeclarationSyntax),
            typeof(ClassDeclarationSyntax),
            typeof(ConstructorDeclarationSyntax),
            typeof(FieldDeclarationSyntax),
            typeof(MethodDeclarationSyntax),
            typeof(OperatorDeclarationSyntax),
            typeof(ParameterListSyntax),
            typeof(ParameterSyntax),
            typeof(InterfaceDeclarationSyntax),
            typeof(DestructorDeclarationSyntax),
            typeof(IndexerDeclarationSyntax),
            typeof(MethodBodyDeclarationSyntax),        // virtual

            // property types
            typeof(PropertyDeclarationSyntax),
            typeof(ReadOnlyPropertyDeclarationSyntax),  // virtual
            typeof(AutoPropertyDeclarationSyntax),      // virtual

            // event types
            typeof(EventDeclarationSyntax),
            typeof(EventFieldDeclarationSyntax),

            // property or event types
            typeof(AccessorDeclarationSyntax),

            // method body types
            typeof(IfStatementSyntax),
            typeof(IfConditionSyntax),                  // virtual
            typeof(IfBodySyntax),                       // virtual
            typeof(ElseBodySyntax),                     // virtual
            typeof(ElseClauseSyntax),
            typeof(LocalDeclarationStatementSyntax),
            typeof(LocalFunctionStatementSyntax),
            typeof(ReturnStatementSyntax),
            typeof(ForStatementSyntax),
            typeof(ForEachStatementSyntax),
            typeof(WhileStatementSyntax),
            typeof(NestedBlockSyntax),                  // virtual
            typeof(ExpressionStatementSyntax),
            typeof(TryStatementSyntax),
            typeof(TryBodySyntax),                      // virtual
            typeof(FinallyClauseSyntax),
            typeof(CatchClauseSyntax),
            typeof(ThrowStatementSyntax),
        };

        public LocalContext(SyntaxTree tree)
        {
            this.tree = tree;
            this.State = new InactiveState(this);
        }

        public LocalContextState State { get; internal set; }

        public void TransitionTo(int line, int lineChar)
        {
            TextSpan span = this.tree.GetText().GetSingleCharSpan(line, lineChar);

            BasicSyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(span);

            var walker = new BasicSyntaxTreeQueryWalker(query,
                new BasicVirtualQueryRouting(
                    new VirtualQueryOverrideRouting(),
                    query.VirtualQueries));

            walker.Visit(this.tree.GetRoot());

            CombinedSyntaxNode[] results = walker.Results
                .Where(x => IsKnownNodeType(x) || x.MixedNode is ExpressionSyntax)
                .ToArray();

            if (results.Length <= 0)
            {
                this.State.TransitionTo(null, this);
                return;
            }

            CombinedSyntaxNode last = results[results.Length - 1];

            // Child ExpressionSyntax nodes can have the same start position
            // as their parents. Therefore, for some of them the last result
            // will be always the last child, despite the active expression.
            // This check will set the "last" active ExpressionSyntax as the
            // last result, allowing navigating between them "up" and "down".
            if (last.BaseNode is ExpressionSyntax expression
                && this.State.ActiveNode != null
                && (this.State.ContextNode != null
                    && this.State.ContextNode.Value.MixedNode.GetType() == typeof(NestedBlockSyntax))
                && this.State.ActiveNode.Value.BaseNode is ExpressionSyntax stateExpression
                && results.Any(
                    x => x.BaseNode is ExpressionSyntax expr && expr == stateExpression))
            {
                int index = -1;

                for (int i = results.Length - 1; i >= 0; i--)
                {
                    if (results[i].BaseNode is ExpressionSyntax e && e == stateExpression)
                    {
                        index = i;
                        break;
                    }
                }

                if (index < 0)
                {
                    throw new InvalidOperationException("Unable to find the active expression.");
                }

                if (index >= results.Length)
                {
                    index = results.Length - 1;
                }

                last = results[index];
            }
            // When the active node isn't set we want the active node to be
            // set to the top-most expression syntax instead of the last.
            else if (last.BaseNode is ExpressionSyntax && this.State.ActiveNode == null)
            {
                int lastIndex = results.Length - 1;

                while (lastIndex > 0 && results[lastIndex].BaseNode is ExpressionSyntax)
                {
                    lastIndex--;
                }

                last = results[++lastIndex];

                int prev = lastIndex - 1;

                if (prev > 0
                        && (results[prev].BaseNode is ExpressionStatementSyntax
                            || results[prev].BaseNode is LocalDeclarationStatementSyntax))
                {
                    last = results[prev];
                }
            }

            LocalContextState stateBefore = this.State;

            this.State.TransitionTo(last, this);

            if (stateBefore != this.State || stateBefore.ContextNode != this.State.ContextNode)
            {
                // query nodes
                this.State.QueryTargetNodes();
            }
        }

        internal static bool IsKnownNodeType(CombinedSyntaxNode node)
        {
            _ = node.BaseNode ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.MixedNode.GetType();

            return SupportedNodeTypes.Contains(nodeType);
        }

        internal static bool IsKnownNodeType(SyntaxNode node)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return SupportedNodeTypes.Contains(node.GetType());
        }
    }
}
