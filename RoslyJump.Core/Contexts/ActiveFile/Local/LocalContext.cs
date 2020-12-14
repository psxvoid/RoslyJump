using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Extensions.SourceTextExtensions;
using dngrep.core.Queries;
using dngrep.core.Queries.SyntaxWalkers;
using dngrep.core.Queries.SyntaxWalkers.MatchStrategies;
using dngrep.core.VirtualNodes;
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
            typeof(PropertyDeclarationSyntax),
            typeof(EventDeclarationSyntax),
            typeof(MethodDeclarationSyntax),
            typeof(OperatorDeclarationSyntax),
            typeof(ParameterSyntax),
            typeof(InterfaceDeclarationSyntax),
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

        private static bool IsKnownNodeType(CombinedSyntaxNode node)
        {
            _ = node.BaseNode ?? throw new ArgumentNullException(nameof(node));

            Type nodeType = node.BaseNode.GetType();

            return SupportedNodeTypes.Contains(nodeType);
        }
    }
}
