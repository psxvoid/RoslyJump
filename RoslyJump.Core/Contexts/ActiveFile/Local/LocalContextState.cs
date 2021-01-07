using System;
using System.Linq;
using dngrep.core.Extensions.EnumerableExtensions;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Syntax;
using dngrep.core.VirtualNodes.VirtualQueries;
using dngrep.core.VirtualNodes.VirtualQueries.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates;
using RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers.Properties;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers;
using RoslyJump.Core.Contexts.Local.States;
using RoslyJump.Core.Infrastructure.Helpers.Reflection;

namespace RoslyJump.Core.Contexts.Local
{
    /// <summary>
    /// The state that support jumping between sibling nodes 
    /// of type <see cref="SiblingStateBase"/>.
    /// </summary>
    /// <typeparam name="TNode">
    /// See documentation for <see cref="LocalContextState{TNode}"/>.
    /// </typeparam>
    /// <typeparam name="T">
    /// The type of the sibling context, e.g. class members.
    /// For example, see <see cref="ClassMemberSiblingState"/>.
    /// </typeparam>
    public abstract class LocalContextState<TNode, T>
        : LocalContextState<TNode>
        where TNode : SyntaxNode
        where T : SiblingStateBase
    {
        protected LocalContextState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            // it was introduced as an optimization but caused
            // an error when TNode was set to the base SyntaxNode
            // for some states.
            //if (!(context.State is LocalContextState<TNode, T>))
            //{
            //    this.SiblingState = this.InitSiblingState();
            //}
            this.SiblingState = this.InitSiblingState();
        }

        protected T? SiblingState { get; private set; }

        protected abstract T InitSiblingState();

        protected void SetSiblingStateFrom(T siblingState)
        {
            this.SiblingState = siblingState;
        }

        public override void TransitionTo(CombinedSyntaxNode? node, LocalContext context)
        {
            LocalContextState stateBefore = context.State;

            base.TransitionTo(node, context);

            LocalContextState stateAfter = context.State;

            LocalContextState<TNode, T>? before = stateBefore as LocalContextState<TNode, T>;
            LocalContextState<TNode, T>? after = stateAfter as LocalContextState<TNode, T>;

            if (before != null && before.SiblingState == null)
            {
                throw new NullReferenceException(
                    "The sibling state isn't set for the previous state.");
            }

            if (before != null && after != null && before.SiblingState != null
                && after.SiblingState != null && node != null
                && after.GetType().IsInheritedFromType(typeof(LocalContextState<TNode, T>))
                && before.SiblingState.HasSibling((CombinedSyntaxNode)node)
                && before.SiblingState.BaseNode == after.SiblingState.BaseNode)
            {
                // states has the same sibling jump targets
                // we want to preserve them
                after.SetSiblingStateFrom(before.SiblingState);
            }
            else if (after != null)
            {
                // it means we are transitioning to a sibling state
                // but we have to initialize it first
                T newSiblingState = after.InitSiblingState();
                after.SetSiblingStateFrom(newSiblingState);
            }
        }

        public override void QueryTargetNodes()
        {
            base.QueryTargetNodes();
        }

        public override void JumpToNextSiblingContext()
        {
            _ = this.SiblingState ?? throw new NullReferenceException(
                "Sibling state should be initialized before jumping to next sibling.");

            this.PerformJumpAction(this.SiblingState.Next);
        }

        public override void JumpToPrevSiblingContext()
        {
            _ = this.SiblingState ?? throw new NullReferenceException(
                "Sibling state should be initialized before jumping to prev sibling.");

            this.PerformJumpAction(this.SiblingState.Prev);
        }

        private void PerformJumpAction(Action<CombinedSyntaxNode?> jumpAction)
        {
            _ = this.SiblingState ?? throw new NullReferenceException(
                "Sibling state should be initialized before jumping to a sibling.");

            this.SiblingState.QueryTargets();

            if (!this.SiblingState.HasTargets)
            {
                return;
            }

            jumpAction(this.Context.State.ActiveNode);

            CombinedSyntaxNode target = this.SiblingState.Target;

            this.TransitionTo(target, this.Context);

            this.Context.State.QueryTargetNodes();
            this.Context.State.SetJumpTargetToActiveNode();
        }
    }

    /// <summary>
    /// This is the same version as <see cref="LocalContextState"/>
    /// but it can access strongly-typed underlying context nodes.
    /// </summary>
    /// <typeparam name="TNode">
    /// The type of the underlying <see cref="SyntaxNode"/> that is
    /// used as a jump target. For example, when the context handles
    /// jumping between <see cref="MethodDeclarationSyntax"/> then
    /// the type should be also set to <see cref="MethodDeclarationSyntax"/>.
    /// </typeparam>
    public abstract class LocalContextState<TNode> : LocalContextState where TNode : SyntaxNode
    {
        private const string ConstraintMismatchError =
            "The context node type should be the same as the generic constraint.";

        protected LocalContextState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
            if (contextNode.BaseNode == null
                || !(contextNode.BaseNode is TNode))
            {
                throw new ArgumentException(
                    ConstraintMismatchError +
                    $"\nExpected: {typeof(TNode)}" +
                    $"\nActual  : {contextNode.BaseNode?.GetType()}",
                    nameof(contextNode));
            }
        }

        public TNode BaseNode
        {
            get
            {
                if (this.ContextNode == null || this.ContextNode?.BaseNode == null)
                {
                    throw new NullReferenceException(
                        "The context node should be initialized before accessing.");
                }


                if (!(this.ContextNode.Value.BaseNode is TNode node))
                {
                    throw new InvalidOperationException(ConstraintMismatchError);
                }

                return node;
            }
        }

        public TNode ActiveBaseNode
        {
            get
            {
                if (this.ActiveNode == null || this.ActiveNode?.BaseNode == null)
                {
                    throw new NullReferenceException(
                        "The context node should be initialized before accessing.");
                }


                if (!(this.ActiveNode.Value.BaseNode is TNode node))
                {
                    throw new InvalidOperationException(
                        ConstraintMismatchError +
                        $" Constraint type: {typeof(TNode)}. " +
                        $"Node type: {this.ActiveNode.Value.BaseNode.GetType()}.");
                }

                return node;
            }
        }
    }

    public abstract class LocalContextState
    {
        public LocalContextState(
            LocalContext context,
            CombinedSyntaxNode? contextNode)
        {
            this.Context = context;
            this.ContextNode = contextNode;
        }

        protected LocalContext Context { get; private set; }
        public CombinedSyntaxNode? ContextNode { get; private set; }

        protected CombinedSyntaxNode[] nodes = Array.Empty<CombinedSyntaxNode>();
        protected bool IsJumpTargetNodesSet => !nodes.IsNullOrEmpty();
        protected int JumpTargetIndex = -1;
        protected virtual int JumpDownCount => 1;

        public CombinedSyntaxNode? ActiveNode
        {
            get
            {
                if (this.IsJumpTargetNodesSet && this.JumpTargetIndex > -1)
                {
                    return this.nodes[this.JumpTargetIndex];
                }
                else if (this.ContextNode != null)
                {
                    return this.ContextNode;
                }

                return null;
            }
        }

        public bool IsJumpTargetSet { get; private set; }
        public int JumpTargetStartLine { get; private set; }
        public int JumpTargetEndLine { get; private set; }
        public int JumpTargetStartChar { get; private set; }
        public int JumpTargetEndChar { get; private set; }


        public virtual void QueryTargetNodes()
        {
            this.nodes = this.QueryTargetNodesFunc();
        }

        protected virtual CombinedSyntaxNode[] QueryClassMembersTargets()
            => Array.Empty<CombinedSyntaxNode>();

        public virtual void TransitionTo(CombinedSyntaxNode? node, LocalContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            this.Context = context;
            this.ContextNode = node;

            if (node == null)
            {
                this.Context.State = new InactiveState(context);
                return;
            }

            if (this.nodes.Contains(node.Value))
            {
                // No need to transition to the same context.
                // Also prevents "resetting" the context
                // when focus is shifted to another node.
                return;
            }

            Type nodeType = node.Value.MixedNode.GetType();

            if (nodeType == typeof(ParameterSyntax))
            {
                this.Context.State = new MethodParameterState(context, node.Value);
            }
            else if (nodeType == typeof(MethodDeclarationSyntax))
            {
                this.Context.State = new MethodDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(OperatorDeclarationSyntax))
            {
                this.Context.State = new OperatorDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(FieldDeclarationSyntax))
            {
                this.Context.State = new FieldDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(PropertyDeclarationSyntax))
            {
                this.Context.State = new PropertyDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(AutoPropertyDeclarationSyntax))
            {
                this.Context.State = new AutoPropertyDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(ReadOnlyPropertyDeclarationSyntax))
            {
                this.Context.State = new ReadOnlyPropertyDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(AccessorDeclarationSyntax))
            {
                this.Context.State = new AccessorDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(ConstructorDeclarationSyntax))
            {
                this.Context.State = new ConstructorDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(ClassDeclarationSyntax))
            {
                this.Context.State = new ClassDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(EnumDeclarationSyntax))
            {
                this.Context.State = new EnumDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(EnumMemberDeclarationSyntax))
            {
                this.Context.State = new EnumMemberDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(StructDeclarationSyntax))
            {
                this.Context.State = new StructDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(InterfaceDeclarationSyntax))
            {
                this.Context.State = new InterfaceDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(NamespaceDeclarationSyntax))
            {
                this.Context.State = new NamespaceDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(UsingDirectiveSyntax))
            {
                this.Context.State = new UsingDirectiveState(context, node.Value);
            }
            else if (nodeType == typeof(CompilationUnitSyntax))
            {
                this.Context.State = new FileContextState(context, node.Value);
            }
            else if (nodeType == typeof(DestructorDeclarationSyntax))
            {
                this.Context.State = new DestructorSyntaxState(context, node.Value);
            }
            else if (nodeType == typeof(IndexerDeclarationSyntax))
            {
                this.Context.State = new IndexerDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(ParameterListSyntax))
            {
                this.Context.State = new ParameterListState(context, node.Value);
            }
            else if (nodeType == typeof(MethodBodyDeclarationSyntax))
            {
                this.Context.State = new MethodBodyState(context, node.Value);
            }
            else if (nodeType == typeof(NestedBlockSyntax))
            {
                this.Context.State = new NestedBlockState(context, node.Value);
            }
            else if (nodeType == typeof(IfStatementSyntax))
            {
                this.Context.State = new IfStatementState(context, node.Value);
            }
            else if (nodeType == typeof(IfConditionSyntax))
            {
                this.Context.State = new IfConditionState(context, node.Value);
            }
            else if (nodeType == typeof(IfBodySyntax))
            {
                this.Context.State = new IfBodyState(context, node.Value);
            }
            else if (nodeType == typeof(ElseBodySyntax))
            {
                this.Context.State = new ElseBodyState(context, node.Value);
            }
            else if (nodeType == typeof(ElseClauseSyntax))
            {
                this.Context.State = new ElseClauseState(context, node.Value);
            }
            else if (nodeType == typeof(LocalDeclarationStatementSyntax))
            {
                this.Context.State = new LocalDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(LocalFunctionStatementSyntax))
            {
                this.Context.State = new LocalFunctionStatementState(context, node.Value);
            }
            else if (nodeType == typeof(ReturnStatementSyntax))
            {
                this.Context.State = new ReturnStatementState(context, node.Value);
            }
            else if (nodeType == typeof(ForStatementSyntax))
            {
                this.Context.State = new ForStatementState(context, node.Value);
            }
            else if (nodeType == typeof(ForEachStatementSyntax))
            {
                this.Context.State = new ForEachStatementState(context, node.Value);
            }
            else if (nodeType == typeof(WhileStatementSyntax))
            {
                this.Context.State = new WhileStatementState(context, node.Value);
            }
            else if (nodeType == typeof(EventDeclarationSyntax))
            {
                this.Context.State = new EventDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(EventFieldDeclarationSyntax))
            {
                this.Context.State = new EventFieldDeclarationState(context, node.Value);
            }
            else if (nodeType == typeof(ExpressionStatementSyntax))
            {
                this.Context.State = new ExpressionStatementState(context, node.Value);
            }
            else if (nodeType == typeof(TryStatementSyntax))
            {
                this.Context.State = new TryStatementState(context, node.Value);
            }
            else if (nodeType == typeof(TryBodySyntax))
            {
                this.Context.State = new TryBodyState(context, node.Value);
            }
            else if (nodeType == typeof(FinallyClauseSyntax))
            {
                this.Context.State = new FinallyClauseState(context, node.Value);
            }
            else if (nodeType == typeof(CatchClauseSyntax))
            {
                this.Context.State = new CatchClauseState(context, node.Value);
            }
            else if (nodeType == typeof(ThrowStatementSyntax))
            {
                this.Context.State = new ThrowStatementState(context, node.Value);
            }
            else if (nodeType == typeof(UsingStatementSyntax))
            {
                this.Context.State = new UsingStatementState(context, node.Value);
            }
            else if (node.Value.MixedNode is ExpressionSyntax)
            {
                this.Context.State = new ExpressionState(context, node.Value);
            }
            else if (node.Value.MixedNode is StatementSyntax)
            {
                this.Context.State = new StatementState(context, node.Value);
            }
            else
            {
                this.Context.State = new InactiveState(context);
                return;
            }
        }

        protected abstract CombinedSyntaxNode[] QueryTargetNodesFunc();

        protected virtual CombinedSyntaxNode? QueryParentContextNode()
        {
            SyntaxNode? parent = this.ContextNode?.BaseNode?.Parent;

            return parent == null
                ? (CombinedSyntaxNode?)null
                : parent.QueryVirtualAndCombine(NestedBlockVirtualQuery.Instance);
        }

        protected virtual CombinedSyntaxNode? QueryChildContextNode()
        {
            SyntaxNode? firstKnownChild = this.ContextNode?.BaseNode
                ?.ChildNodes()
                .FirstOrDefault(x => LocalContext.IsKnownNodeType(x)
                    || x is ExpressionSyntax);

            return firstKnownChild == null
                ? (CombinedSyntaxNode?)null
                : firstKnownChild.QueryVirtualAndCombine(
                    NestedBlockVirtualQuery.Instance);
        }

        public virtual void JumpNext()
        {
            if (this.IsJumpTargetNodesSet)
            {
                bool isCurrentFound = false;

                if (this.ContextNode != null)
                {
                    for (int i = 0; i < this.nodes.Length; i++)
                    {
                        if (this.nodes[i] == this.ActiveNode)
                        {
                            this.JumpTargetIndex = i + 1;
                            isCurrentFound = true;
                            break;
                        }
                    }
                }

                if (this.ContextNode == null || !isCurrentFound)
                {
                    this.JumpTargetIndex++;
                }

                if (this.JumpTargetIndex >= nodes.Length)
                {
                    this.JumpTargetIndex = 0;
                }

                CombinedSyntaxNode target = nodes[this.JumpTargetIndex];

                this.SetJumpTarget(target);
            }
        }

        public virtual void JumpPrev()
        {
            if (this.IsJumpTargetNodesSet)
            {
                bool isCurrentFound = false;

                if (this.ActiveNode != null)
                {
                    for (int i = 0; i < this.nodes.Length; i++)
                    {
                        if (this.nodes[i] == this.ActiveNode)
                        {
                            this.JumpTargetIndex = i - 1;
                            isCurrentFound = true;
                            break;
                        }
                    }
                }

                if (this.ActiveNode == null || !isCurrentFound)
                {
                    this.JumpTargetIndex--;
                }

                if (this.JumpTargetIndex < 0)
                {
                    this.JumpTargetIndex = this.nodes.Length - 1;
                }

                CombinedSyntaxNode target = nodes[this.JumpTargetIndex];

                this.SetJumpTarget(target);
            }
        }

        public virtual void JumpContextUp()
        {
            CombinedSyntaxNode? parentContextNode = this.QueryParentContextNode();

            if (parentContextNode == null)
            {
                return;
            }

            this.TransitionTo(parentContextNode, this.Context);
            this.Context.State.QueryTargetNodes();
            this.Context.State.SetJumpTarget(
                this.Context.State.ContextNode ?? throw new InvalidOperationException(
                    "Jump target for the upper context is missing."));
        }

        public virtual void JumpContextDown()
        {
            for (int i = 0; i < this.JumpDownCount; i++)
            {
                CombinedSyntaxNode? firstChildContextNode =
                    this.Context.State.QueryChildContextNode();

                if (firstChildContextNode == null)
                {
                    return;
                }

                this.TransitionTo(firstChildContextNode, this.Context);
                this.Context.State.QueryTargetNodes();
                this.Context.State.SetJumpTarget(
                    this.Context.State.ContextNode ?? throw new InvalidOperationException(
                        "Jump target for the lower context is missing."));
            }
        }

        public virtual void JumpToNextSiblingContext()
        {
            // do nothing
        }

        public virtual void JumpToPrevSiblingContext()
        {
            // do nothing
        }

        protected internal void SetJumpTargetToActiveNode()
        {
            if (this.ActiveNode != null)
            {
                this.SetJumpTarget(this.ActiveNode.Value);
            }
        }

        protected void SetJumpTarget(CombinedSyntaxNode target)
        {
            int lineStart, lineEnd, charStart, charEnd;

            if (target.IsVirtual && target.MixedNode is IVirtualSyntaxNodeWithSpanOverride node)
            {
                (lineStart, lineEnd, charStart, charEnd) =
                    this.Context.SyntaxTree.GetMappedLineSpan(node.SourceSpan)
                    .GetSourceTextBounds();
            }
            else
            {
                (lineStart, lineEnd, charStart, charEnd) = target.BaseNode.GetSourceTextBounds();
            }

            this.JumpTargetStartLine = lineStart;
            this.JumpTargetEndLine = lineEnd;
            this.JumpTargetStartChar = charStart;
            this.JumpTargetEndChar = charEnd;
            this.IsJumpTargetSet = true;
        }
    }
}
