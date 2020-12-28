using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SourceTextExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxWalkers;
using dngrep.core.Queries.SyntaxWalkers.MatchStrategies;
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

            CombinedSyntaxTreeQuery query = SyntaxTreeQueryBuilder.From(span);
            var walker = new CombinedSyntaxTreeQueryWalker(
                query,
                new VirtualQueryRoutingFactory(),
                new VirtualQueryOverrideRouting(),
                new BaseScopedSyntaxNodeMatchStrategy(query.VirtualNodeSubQueries));

            walker.Visit(this.tree.GetRoot());

            IReadOnlyCollection<CombinedSyntaxNode> results = walker.Results
                .Where(IsKnownNodeType)
                .ToArray();

            if (results.Count <= 0)
            {
                this.State.TransitionTo(null, this);
                return;
            }

            LocalContextState stateBefore = this.State;

            this.State.TransitionTo(results.Last(), this);

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
