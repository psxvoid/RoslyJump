using System;
using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers.Properties;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyStates;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Contexts.Local.States;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis.VirtualNodes;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration
{
    public class StateTransitionTests : IClassFixture<StateTransitionFixture>
    {
        private readonly StateTransitionFixture classFixture;

        public StateTransitionTests(StateTransitionFixture classFixture)
        {
            this.classFixture = classFixture;
        }


        [Fact]
        public void Indexer_JumpNext_NextIndexer()
        {
            AssertTransition<IndexerDeclarationSyntax, IndexerDeclarationState>(
                ActionKind.JumpNext,
                x => x.ParentAs<ClassDeclarationSyntax>().HasName("C1")
                    && x.HasIndexerParam("int i"),
                x => x.ActiveBaseNode.HasIndexerParam("string str"));
        }

        [Fact]
        public void Indexer_JumpPrev_PrevIndexer()
        {
            AssertTransition<IndexerDeclarationSyntax, IndexerDeclarationState>(
                ActionKind.JumpPrev,
                x => x.ParentAs<ClassDeclarationSyntax>().HasName("C1")
                    && x.HasIndexerParam("int i"),
                x => x.ActiveBaseNode.HasIndexerParam("object o"));
        }

        [Fact]
        public void Indexer_JumpNextSibling_NextClassMember()
        {
            AssertTransition<IndexerDeclarationSyntax, ReadOnlyPropertyDeclarationState>(
                ActionKind.JumpNextSibling,
                x => x.ParentAs<ClassDeclarationSyntax>().HasName("C1")
                    && x.HasIndexerParam("int i"),
                x => x.ActiveBaseNode.HasName("Prop1"));
        }

        [Fact]
        public void Indexer_JumpPrevSibling_PrevClassMember()
        {
            AssertTransition<IndexerDeclarationSyntax, MethodDeclarationState>(
                ActionKind.JumpPrevSibling,
                x => x.ParentAs<ClassDeclarationSyntax>().HasName("C1")
                    && x.HasIndexerParam("int i"),
                x => x.ActiveBaseNode.HasName("Method1"));
        }

        [Fact]
        public void Indexer_JumpUp_ClassDeclaration()
        {
            AssertTransition<IndexerDeclarationSyntax, ClassDeclarationState>(
                ActionKind.JumpContextUp,
                x => x.ParentAs<ClassDeclarationSyntax>().HasName("C1")
                    && x.HasIndexerParam("int i"),
                x => x.ActiveBaseNode.HasName("C1"));
        }

        [Fact]
        public void Indexer_JumpDown_SameIndexer()
        {
            AssertTransition<IndexerDeclarationSyntax, IndexerDeclarationState>(
                ActionKind.JumpContextDown,
                x => x.ParentAs<ClassDeclarationSyntax>().HasName("C1")
                    && x.HasIndexerParam("int i"),
                x => x.ActiveBaseNode.HasIndexerParam("int i"));
        }

        [Fact]
        public void MethodDeclaration_JumpDown_ParameterList()
        {
            AssertTransition<MethodDeclarationSyntax, ParameterListState>(
                ActionKind.JumpContextDown,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false));
        }

        [Fact]
        public void MethodDeclaration_JumpUp_ClassDeclaration()
        {
            AssertTransition<MethodDeclarationSyntax, ClassDeclarationState>(
                ActionKind.JumpContextUp,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false));
        }

        [Fact]
        public void MethodDeclaration_JumpNext_NextMethodDeclaration()
        {
            AssertTransition<MethodDeclarationSyntax, MethodDeclarationState>(
                ActionKind.JumpNext,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<MethodDeclarationSyntax>().HasName("Method2"));
        }

        [Fact]
        public void MethodDeclaration_JumpPrev_PrevMethodDeclaration()
        {
            AssertTransition<MethodDeclarationSyntax, MethodDeclarationState>(
                ActionKind.JumpPrev,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<MethodDeclarationSyntax>().HasName("Method3"));
        }

        [Fact]
        public void MethodDeclaration_JumpNextSibling_NextSibling()
        {
            AssertTransition<MethodDeclarationSyntax, IndexerDeclarationState>(
                ActionKind.JumpNextSibling,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveBaseNode.HasIndexerParam("int i"));
        }

        [Fact]
        public void MethodDeclaration_JumpPrevSibling_PrevSibling()
        {
            AssertTransition<MethodDeclarationSyntax, FieldDeclarationState>(
                ActionKind.JumpPrevSibling,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<FieldDeclarationSyntax>().HasName("field1"));
        }

        [Fact]
        public void ParameterList_JumpNext_MethodBody()
        {
            AssertTransition<ParameterListSyntax, ParameterListState>(
                ActionKind.JumpNext,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAs<ParameterListSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void ParameterList_JumpPrev_MethodBody()
        {
            AssertTransition<ParameterListSyntax, ParameterListState>(
                ActionKind.JumpPrev,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAs<ParameterListSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void ParameterList_JumpNextSibling_MethodBody()
        {
            AssertTransition<ParameterListSyntax, MethodBodyState>(
                ActionKind.JumpNextSibling,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>()
                    .BaseNodeAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void ParameterList_JumpPrevSibling_MethodBody()
        {
            AssertTransition<ParameterListSyntax, MethodBodyState>(
                ActionKind.JumpPrevSibling,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>()
                    .BaseNodeAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void ParameterList_JumpDown_FirstParameter()
        {
            AssertTransition<ParameterListSyntax, MethodParameterState>(
                ActionKind.JumpContextDown,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAs<ParameterSyntax>()
                    .ParentAs<ParameterListSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpNext_MethodBody()
        {
            AssertTransition<BlockSyntax, MethodBodyState>(
                ActionKind.JumpNext,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>()
                    .BaseNodeAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpPrev_MethodBody()
        {
            AssertTransition<BlockSyntax, MethodBodyState>(
                ActionKind.JumpPrev,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>()
                    .BaseNodeAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpNextSibling_ParameterList()
        {
            AssertTransition<BlockSyntax, ParameterListState>(
                ActionKind.JumpNextSibling,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAs<ParameterListSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpPrevSibling_ParameterList()
        {
            AssertTransition<BlockSyntax, ParameterListState>(
                ActionKind.JumpPrevSibling,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAs<ParameterListSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpUp_MethodDeclaration()
        {
            AssertTransition<BlockSyntax, MethodDeclarationState>(
                ActionKind.JumpContextUp,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.BaseNode.HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpDown_FirstIfStatement()
        {
            AssertTransition<BlockSyntax, IfStatementState>(
                ActionKind.JumpContextDown,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAs<IfStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpDown_FirstLocalDeclaration()
        {
            AssertTransition<BlockSyntax, LocalDeclarationState>(
                ActionKind.JumpContextDown,
                x => x.ParentAsOrNull<MethodDeclarationSyntax>()?.HasName("Method2") ?? false,
                x => x.ActiveNodeAs<LocalDeclarationStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method2"));
        }

        [Fact]
        public void MethodBody_JumpDown_FirstExpressionStatement()
        {
            AssertTransition<BlockSyntax, ExpressionStatementState>(
                ActionKind.JumpContextDown,
                x => x.ParentAsOrNull<MethodDeclarationSyntax>()?.HasName("Method3") ?? false,
                x => x.ActiveNodeAs<ExpressionStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method3"));
        }

        [Fact]
        public void ParameterList_JumpUp_MethodDeclaration()
        {
            AssertTransition<ParameterListSyntax, MethodDeclarationState>(
                ActionKind.JumpContextUp,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void IfStatementCondition_JumpNext_NextConditionStatement()
        {
            AssertTransition<IfStatementSyntax, IfStatementState>(
                ActionKind.JumpNext,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAs<IfStatementSyntax>().HasCondition("x == 6"));
        }

        [Fact]
        public void IfStatementCondition_JumpPrev_PrevConditionStatement()
        {
            AssertTransition<IfStatementSyntax, IfStatementState>(
                ActionKind.JumpPrev,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAs<IfStatementSyntax>().HasCondition("x == 6"));
        }

        [Fact]
        public void IfStatementCondition_JumpNextSibling_NextMethodMember()
        {
            AssertTransition<IfStatementSyntax, ReturnStatementState>(
                ActionKind.JumpNextSibling,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAs<ReturnStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void IfStatementCondition_JumpPrevSibling_PrevMethodMember()
        {
            AssertTransition<IfStatementSyntax, ReturnStatementState>(
                ActionKind.JumpPrevSibling,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAs<ReturnStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void IfStatementDeclaration_JumpDown_ConditionStatement()
        {
            AssertTransition<IfStatementSyntax, MethodBodyState>(
                ActionKind.JumpContextDown,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>().HasExpression("x == 3"));
        }

        [Fact]
        public void IfStatementDeclaration_JumpUp_ConditionStatement()
        {
            AssertTransition<IfStatementSyntax, MethodBodyState>(
                ActionKind.JumpContextUp,
                x => x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method1")
                    ?? false,
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>()
                    .BaseNodeAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void ExpressionStatement_JumpNext_NextMethodMember()
        {
            AssertTransition<ExpressionStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpNext,
                x => x.HasExpression("var (x, y) = this.Method2(2, 3)")
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method3")
                        ?? false),
                x => x.ActiveBaseNode.HasExpression("var (v1, v2) = this.Method2(3, 3)"));
        }

        [Fact]
        public void ExpressionStatement_JumpPrev_PrevMethodMember()
        {
            AssertTransition<ExpressionStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpPrev,
                x => x.HasExpression("var (x, y) = this.Method2(2, 3)")
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method3")
                        ?? false),
                x => x.ActiveBaseNode.HasExpression("var (v3, v4) = this.Method2(4, 3)"));
        }

        [Fact]
        public void ExpressionStatement_JumpNextSibling_NextMethodMember()
        {
            AssertTransition<ExpressionStatementSyntax, LocalDeclarationState>(
                ActionKind.JumpNextSibling,
                x => x.HasExpression("var (x, y) = this.Method2(2, 3)")
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method3")
                        ?? false),
                x => x.ActiveBaseNode.HasDeclaration("int z = 3")
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method3"));
        }

        [Fact]
        public void ExpressionStatement_JumpPrevSibling_PrevMethodMember()
        {
            AssertTransition<ExpressionStatementSyntax, ReturnStatementState>(
                ActionKind.JumpPrevSibling,
                x => x.HasExpression("var (x, y) = this.Method2(2, 3)")
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method3")
                        ?? false),
                x => x.ActiveBaseNode.HasExpression("y + v2 + z")
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method3"));
        }

        [Fact]
        public void ExpressionStatement_JumpUp_MethodBody()
        {
            AssertTransition<ExpressionStatementSyntax, MethodBodyState>(
                ActionKind.JumpContextUp,
                x => x.HasExpression("var (x, y) = this.Method2(2, 3)")
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method3")
                        ?? false),
                x => x.ActiveBaseNode
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method3"));
        }

        [Fact]
        public void ExpressionStatement_JumpDown_SameStatement()
        {
            AssertTransition<ExpressionStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpContextDown,
                x => x.HasExpression("var (x, y) = this.Method2(2, 3)")
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method3")
                        ?? false),
                x => x.ActiveBaseNode.HasExpression("var (x, y) = this.Method2(2, 3)")
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method3"));
        }

        private enum ActionKind
        {
            JumpNext,
            JumpPrev,
            JumpNextSibling,
            JumpPrevSibling,
            JumpContextUp,
            JumpContextDown,
        }

        private void AssertTransition<TStartPositionNode, TExpectedState>(
            ActionKind action,
            Func<TStartPositionNode, bool>? startNodePredicate = null,
            Func<TExpectedState, bool>? statePredicate = null)
            where TStartPositionNode : SyntaxNode
            where TExpectedState : LocalContextState
        {
            LocalContext context = new LocalContext(this.classFixture.SyntaxTree);

            TStartPositionNode? node = this.classFixture.SyntaxTree.GetRoot().ChildNodes()
                .GetNodesOfTypeRecursively<TStartPositionNode>()
                .Where(x => startNodePredicate == null || startNodePredicate(x))
                .First();

            var (lineStart, lineEnd, charStart, charEnd) = node.GetSourceTextBounds();

            context.TransitionTo(lineStart, charStart);

            switch (action)
            {
                case ActionKind.JumpNext:
                    context.State.JumpNext();
                    break;
                case ActionKind.JumpPrev:
                    context.State.JumpPrev();
                    break;
                case ActionKind.JumpNextSibling:
                    context.State.JumpToNextSiblingContext();
                    break;
                case ActionKind.JumpPrevSibling:
                    context.State.JumpToPrevSiblingContext();
                    break;
                case ActionKind.JumpContextUp:
                    context.State.JumpContextUp();
                    break;
                case ActionKind.JumpContextDown:
                    context.State.JumpContextDown();
                    break;
                default:
                    throw new ArgumentException("Unknown action kind.", nameof(action));
            }

            Assert.IsType<TExpectedState>(context.State);

            if (statePredicate != null)
            {
                Assert.True(statePredicate((TExpectedState)context.State));
            }
        }
    }
}
