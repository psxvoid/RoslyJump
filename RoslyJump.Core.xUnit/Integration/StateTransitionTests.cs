using System.Linq;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using dngrep.core.VirtualNodes.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.ClassMembers.Properties;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.MethodBodyMembers;
using RoslyJump.Core.Contexts.Local.States;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis.VirtualNodes;
using RoslyJump.Core.xUnit.Integration.Fixtures;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration
{
    [Collection(nameof(StateTransitionFixture))]
    public class StateTransitionTests : StateTransitionTestBase
    {
        protected override SyntaxTree SyntaxTree { get; }

        public StateTransitionTests(StateTransitionFixture classFixture)
        {
            this.SyntaxTree = classFixture.SyntaxTree;
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
            AssertTransition<IndexerDeclarationSyntax, FieldDeclarationState>(
                ActionKind.JumpNextSibling,
                x => x.ParentAs<ClassDeclarationSyntax>().HasName("C1")
                    && x.HasIndexerParam("int i"),
                x => x.ActiveBaseNode.HasName("field1"));
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
        public void MethodDeclaration_ReturnValueTypeContextSelection_MethodDeclaration()
        {
            AssertTransition<TypeSyntax, MethodDeclarationState>(
                ActionKind.JumpNext,
                x => x.Parent is MethodDeclarationSyntax m && m.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                null,
                null,
                x => Assert.IsType<MethodDeclarationState>(x.State));
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
                x => x.ActiveNodeAs<MethodDeclarationSyntax>().HasName("Method8"));
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
            AssertTransition<MethodDeclarationSyntax, ReadOnlyPropertyDeclarationState>(
                ActionKind.JumpPrevSibling,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<PropertyDeclarationSyntax>().HasName("Prop1ReadonlyInt"));
        }

        [Fact]
        public void MethodDeclaration_JumpUp_ClassDeclaration()
        {
            AssertTransition<MethodDeclarationSyntax, ClassDeclarationState>(
                ActionKind.JumpContextUp,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                (System.Func<ClassDeclarationState, bool>?)null);
        }

        [Fact]
        public void MethodDeclaration_JumpDown_ParameterList()
        {
            AssertTransition<MethodDeclarationSyntax, ParameterListState>(
                ActionKind.JumpContextDown,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                null,
                null,
                x => Assert.IsType<MethodDeclarationState>(x.State));
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
        public void ParameterList_JumpUp_MethodDeclaration()
        {
            AssertTransition<ParameterListSyntax, MethodDeclarationState>(
                ActionKind.JumpContextUp,
                x => x.ParentAs<MethodDeclarationSyntax>().HasName("Method1"),
                x => x.ActiveNodeAs<MethodDeclarationSyntax>().HasName("Method1"));
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
                x => x.Parent is MethodDeclarationSyntax method && method.HasName("Method1"),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>()
                    .BaseNodeAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpPrev_MethodBody()
        {
            AssertTransition<BlockSyntax, MethodBodyState>(
                ActionKind.JumpPrev,
                x => x.Parent is MethodDeclarationSyntax method && method.HasName("Method1"),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>()
                    .BaseNodeAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpNextSibling_ParameterList()
        {
            AssertTransition<BlockSyntax, ParameterListState>(
                ActionKind.JumpNextSibling,
                x => x.Parent is MethodDeclarationSyntax method && method.HasName("Method1"),
                x => x.ActiveNodeAs<ParameterListSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpPrevSibling_ParameterList()
        {
            AssertTransition<BlockSyntax, ParameterListState>(
                ActionKind.JumpPrevSibling,
                x => x.Parent is MethodDeclarationSyntax method && method.HasName("Method1"),
                x => x.ActiveNodeAs<ParameterListSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpUp_MethodDeclaration()
        {
            AssertTransition<BlockSyntax, MethodDeclarationState>(
                ActionKind.JumpContextUp,
                x => x.Parent is MethodDeclarationSyntax method && method.HasName("Method1"),
                x => x.BaseNode.HasName("Method1"));
        }

        [Fact]
        public void MethodBody_JumpDown_FirstIfStatement()
        {
            AssertTransition<BlockSyntax, IfStatementState>(
                ActionKind.JumpContextDown,
                x => x.Parent is MethodDeclarationSyntax method && method.HasName("Method1"),
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
        public void MethodBody_JumpDownThisExpressionJumpUp_SameMethodBody()
        {
            AssertTransition<BlockSyntax, MethodBodyState>(
                ActionKind.JumpContextUp,
                x => x.ParentAsOrNull<MethodDeclarationSyntax>()?.HasName("Method31") ?? false,
                x => x.ActiveNodeAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>().HasName("Method31"),
                null,
                x =>
                {
                    Assert.IsType<MethodBodyState>(x.State);
                    x.State.JumpContextDown();
                    Assert.IsType<ExpressionStatementState>(x.State);

                    ExpressionStatementSyntax statement = x.State
                        .ActiveNodeAs<ExpressionStatementSyntax>();

                    FileLinePositionSpan line = statement.GetLocation().GetLineSpan();
                    int lineStart = line.StartLinePosition.Line;
                    int charStart = line.StartLinePosition.Character;

                    x.TransitionTo(lineStart, charStart);
                    Assert.IsType<ExpressionStatementState>(x.State);
                });
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
        public void IfStatementCondition_JumpNextSibling_IfBody()
        {
            AssertTransition<ExpressionSyntax, IfBodyState>(
                ActionKind.JumpNextSibling,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x =>
                {
                    Assert.Equal(
                        "x == 3",
                        x.ActiveBaseNode.ParentAs<IfStatementSyntax>().Condition.ToString());

                    Assert.Equal(
                        "Method1",
                        x.ActiveNodeAs<BlockSyntax>()
                            .ParentAs<IfStatementSyntax>()
                            .ParentAs<BlockSyntax>()
                            .ParentAs<MethodDeclarationSyntax>().Identifier.ValueText);
                },
                null,
                x =>
                {
                    Assert.IsType<IfConditionState>(x.State);
                });
        }

        [Fact]
        public void IfStatementCondition_JumpPrevSibling_ElseClause()
        {
            AssertTransition<ExpressionSyntax, ElseClauseState>(
                ActionKind.JumpPrevSibling,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x =>
                {
                    Assert.Equal(
                        "x == 3",
                        x.ActiveBaseNode.ParentAs<IfStatementSyntax>().Condition.ToString());

                    Assert.Equal(
                        "Method1",
                        x.ActiveBaseNode
                            .ParentAs<IfStatementSyntax>()
                            .ParentAs<BlockSyntax>()
                            .ParentAs<MethodDeclarationSyntax>().Identifier.ValueText);
                },
                null,
                x =>
                {
                    Assert.IsType<IfConditionState>(x.State);
                });
        }

        [Fact]
        public void IfStatementCondition_JumpDownTwiceAndNext_NextSiblingExpresssion()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpNext,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x =>
                {
                    Assert.Equal("3", x.ActiveBaseNode.ToString());
                    Assert.Equal(
                        "x == 3",
                        x.ActiveBaseNode
                            .ParentAs<BinaryExpressionSyntax>()
                            .ParentAs<IfStatementSyntax>().Condition.ToString());

                    Assert.Equal(
                        "Method1",
                        x.ActiveBaseNode
                            .ParentAs<ExpressionSyntax>()
                            .ParentAs<IfStatementSyntax>()
                            .ParentAs<BlockSyntax>()
                            .ParentAs<MethodDeclarationSyntax>().Identifier.ValueText);
                },
                null,
                x =>
                {
                    Assert.IsType<IfConditionState>(x.State);

                    x.State.JumpContextDown();

                    Assert.IsType<NestedBlockState>(x.State);
                    Assert.Equal("x == 3", x.State.ActiveNodeAs<ExpressionSyntax>().ToString());

                    x.State.JumpContextDown();

                    Assert.IsType<NestedBlockState>(x.State);
                    Assert.Equal("x", x.State.ActiveNodeAs<ExpressionSyntax>().ToString());
                });
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
        public void IfStatementDeclaration_JumpDown_ConditionStatement()
        {
            AssertTransition<IfStatementSyntax, IfConditionState>(
                ActionKind.JumpContextDown,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAsVirtual<IfConditionSyntax>()
                    .Expression.ToString() == "x == 3");
        }

        [Fact]
        public void IfCondition_JumpNext_SameCondition()
        {
            AssertTransition<IfStatementSyntax, IfConditionState>(
                ActionKind.JumpNext,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAsVirtual<IfConditionSyntax>()
                        .Expression.ToString() == "x == 3"
                    && x.ActiveNodeAsVirtual<IfConditionSyntax>()
                        .Expression
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                x => x.OpenParenToken.GetLocation().GetLineSpan());
        }

        [Fact]
        public void IfCondition_JumpPrev_SameCondition()
        {
            AssertTransition<IfStatementSyntax, IfConditionState>(
                ActionKind.JumpPrev,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAsVirtual<IfConditionSyntax>()
                        .Expression.ToString() == "x == 3"
                    && x.ActiveNodeAsVirtual<IfConditionSyntax>()
                        .Expression
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                x => x.OpenParenToken.GetLocation().GetLineSpan());
        }

        [Fact]
        public void IfCondition_JumpNextSibling_IfBody()
        {
            AssertTransition<IfStatementSyntax, IfBodyState>(
                ActionKind.JumpNextSibling,
                x => x.HasCondition("x == 3"),
                x => x.ActiveNodeAsVirtual<IfBodySyntax>()
                        .Statement
                        .GetFirstChildOfTypeRecursively<ReturnStatementSyntax>()
                        .HasExpression("x * 3 + y")
                    && x.ActiveNodeAsVirtual<IfBodySyntax>().Statement
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                x => x.OpenParenToken.GetLocation().GetLineSpan());
        }

        [Fact]
        public void IfCondition_JumpPrevSibling_ElseClause()
        {
            AssertTransition<IfStatementSyntax, ElseClauseState>(
                ActionKind.JumpPrevSibling,
                x => x.HasCondition("x == 3"),
                x => x.ActiveBaseNode.Statement
                        .GetFirstChildOfTypeRecursively<ExpressionSyntax>()
                        .ToString() == "x == 4"
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                x => x.OpenParenToken.GetLocation().GetLineSpan());
        }

        [Fact]
        public void IfCondition_JumpUp_IfStatement()
        {
            AssertTransition<IfStatementSyntax, IfStatementState>(
                ActionKind.JumpContextUp,
                x => x.HasCondition("x == 3"),
                x => x.ActiveBaseNode.HasCondition("x == 3")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                x => x.OpenParenToken.GetLocation().GetLineSpan());
        }

        [Fact]
        public void IfCondition_JumpDown_NestedBlock()
        {
            AssertTransition<IfStatementSyntax, NestedBlockState>(
                ActionKind.JumpContextDown,
                x => x.HasCondition("x == 3"),
                x => x.ActiveBaseNode.ToString() == "x == 3"
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                x => x.OpenParenToken.GetLocation().GetLineSpan());
        }

        [Fact]
        public void IfConditionComplexExpression_JumpNext_SameNestedBlock()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpNext,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x == 12 || y == 11"
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x => x.State.JumpContextDown());
        }

        [Fact]
        public void IfConditionComplexExpression_JumpPrev_SameNestedBlock()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpPrev,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x == 12 || y == 11"
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x => x.State.JumpContextDown());
        }

        [Fact]
        public void IfConditionComplexExpression_JumpNextSibling_SameExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpNextSibling,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x == 12 || y == 11"
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x => x.State.JumpContextDown());
        }

        [Fact]
        public void IfConditionComplexExpression_JumpPrevSibling_SameExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpPrevSibling,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ParentAs<IfStatementSyntax>()
                        .HasCondition("x == 12 || y == 11")
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x => x.State.JumpContextDown());
        }

        [Fact]
        public void IfConditionComplexExpression_JumpUp_IfCondition()
        {
            AssertTransition<ExpressionSyntax, IfConditionState>(
                ActionKind.JumpContextUp,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x == 12 || y == 11"
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x => x.State.JumpContextDown());
        }

        [Fact]
        public void IfConditionComplexExpression_JumpDown_FirstChildExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpContextDown,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x == 12"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    // we need it because on the first jump
                    // the state will ALWAYS be set to the top-most
                    // expression state (equal to if-expression in this case)
                    //
                    // it set's the state to the first expression
                    x.State.JumpContextDown();
                });
        }

        [Fact]
        public void IfConditionComplexExpression_JumpDownTwice_FirstChildExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpContextDown,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x"
                    && x.ActiveBaseNode
                        .ParentAs<BinaryExpressionSyntax>()
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("x == 12", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNested_JumpNext_NextExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpNext,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "y == 11"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("x == 12", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNested_JumpPrev_PrevExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpPrev,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "y == 11"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("x == 12", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNested_JumpNextSibling_SameExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpNextSibling,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x == 12"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("x == 12", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNested_JumpPrevSibling_SameExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpPrevSibling,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x == 12"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("x == 12", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNested_JumpUp_ParentExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpContextUp,
                x => x.ToString() == "x == 12 || y == 11",
                x =>
                {
                    Assert.Equal("x == 12 || y == 11", x.ActiveBaseNode.ToString());
                    Assert.Equal(
                        "Method1",
                        x.ActiveBaseNode
                            .ParentAs<IfStatementSyntax>()
                            .ParentAs<BlockSyntax>()
                            .ParentAs<IfStatementSyntax>()
                            .ParentAs<BlockSyntax>()
                            .ParentAs<IfStatementSyntax>()
                            .ParentAs<BlockSyntax>()
                            .ParentAs<MethodDeclarationSyntax>()
                            .Identifier.ValueText);
                },
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("x == 12", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNested_JumpDown_FirstChildExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpContextDown,
                x => x.ToString() == "x == 12 || y == 11",
                x => x.ActiveBaseNode.ToString() == "x"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("x == 12", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNestedNested_JumpNext_NextExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpNext,
                x => x.ToString() == "fu(5) < 10 && fuu(5) < 11",
                x => x.ActiveBaseNode.ToString() == "10"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("fu(5)", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNestedNested_JumpPrev_PrevExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpPrev,
                x => x.ToString() == "fu(5) < 10 && fuu(5) < 11",
                x => x.ActiveBaseNode.ToString() == "10"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("fu(5)", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNestedNested_JumpNextSibling_SameExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpNextSibling,
                x => x.ToString() == "fu(5) < 10 && fuu(5) < 11",
                x => x.ActiveBaseNode.ToString() == "fu(5)"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("fu(5)", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNestedNested_JumpPrevSibling_SameExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpPrevSibling,
                x => x.ToString() == "fu(5) < 10 && fuu(5) < 11",
                x => x.ActiveBaseNode.ToString() == "fu(5)"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("fu(5)", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNestedNested_JumpUp_ParentExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpContextUp,
                x => x.ToString() == "fu(5) < 10 && fuu(5) < 11",
                x => x.ActiveBaseNode.ToString() == "fu(5) < 10"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("fu(5)", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void IfConditionComplexExpressionNestedNested_JumpDown_FirstChildExpression()
        {
            AssertTransition<ExpressionSyntax, NestedBlockState>(
                ActionKind.JumpContextDown,
                x => x.ToString() == "fu(5) < 10 && fuu(5) < 11",
                x => x.ActiveBaseNode.ToString() == "fu"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<ExpressionSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"),
                null,
                x =>
                {
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    x.State.JumpContextDown();
                    Assert.Equal("fu(5)", x.State?.ContextNode?.BaseNode.ToString());
                });
        }

        [Fact]
        public void ElseClause_JumpNext_NextElseClause()
        {
            AssertTransition<ElseClauseSyntax, ElseClauseState>(
                ActionKind.JumpNext,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x => ((IfStatementSyntax)x.ActiveBaseNode.Statement).HasCondition("x == 5")
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpPrev_PrevElseClause()
        {
            AssertTransition<ElseClauseSyntax, ElseClauseState>(
                ActionKind.JumpPrev,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ReturnStatementSyntax>()
                        .HasExpression("x * 8 + y")
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpNextSibling_IfCondition()
        {
            AssertTransition<ElseClauseSyntax, IfConditionState>(
                ActionKind.JumpNextSibling,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x => x.ActiveBaseNode.ToString() == "x == 3"
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpNextSiblingAndNestedElseClause_IfCondition()
        {
            AssertTransition<ElseClauseSyntax, IfConditionState>(
                ActionKind.JumpNextSibling,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 4"),
                x => x.ActiveBaseNode.ToString() == "x == 4"
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpPrevSibling_IfBody()
        {
            AssertTransition<ElseClauseSyntax, IfBodyState>(
                ActionKind.JumpPrevSibling,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<IfStatementSyntax>()
                        .HasCondition("y == 3")
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpPrevSiblingAndNestedElseClause_IfBody()
        {
            AssertTransition<ElseClauseSyntax, IfBodyState>(
                ActionKind.JumpPrevSibling,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 4"),
                x => x.ActiveBaseNode.ChildNodes()
                        .OfType<ReturnStatementSyntax>().Single()
                        .HasExpression("x * 6 + y")
                    && x.ActiveBaseNode
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpUp_RootIfStatement()
        {
            AssertTransition<ElseClauseSyntax, IfStatementState>(
                ActionKind.JumpContextUp,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x => x.ActiveBaseNode.HasCondition("x == 3")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpUpAndNestedElseClause_RootIfStatement()
        {
            AssertTransition<ElseClauseSyntax, IfStatementState>(
                ActionKind.JumpContextUp,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 4"),
                x => x.ActiveBaseNode.HasCondition("x == 3")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpDown_ChildIfStatement()
        {
            AssertTransition<ElseClauseSyntax, IfStatementState>(
                ActionKind.JumpContextDown,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 3"),
                x => x.ActiveBaseNode.HasCondition("x == 4")
                    && x.ActiveBaseNode
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpDownAndNestedElseClause_ChildIfStatement()
        {
            AssertTransition<ElseClauseSyntax, IfStatementState>(
                ActionKind.JumpContextDown,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 4"),
                x => x.ActiveBaseNode.HasCondition("x == 5")
                    && x.ActiveBaseNode
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void ElseClause_JumpDownAndNestedElseBlock_ElseBody()
        {
            AssertTransition<ElseClauseSyntax, ElseBodyState>(
                ActionKind.JumpContextDown,
                x => x.Parent is IfStatementSyntax @if && @if.HasCondition("x == 5"),
                x => x.ActiveBaseNode.ChildNodes()
                        .OfType<ReturnStatementSyntax>().Single()
                        .HasExpression("x * 8 + y")
                    && x.ActiveBaseNode
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<ElseClauseSyntax>()
                        .ParentAs<IfStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method1"));
        }

        [Fact]
        public void NestedIfStatementDeclaration_JumpNext_NextNestedIfStatement()
        {
            AssertTransition<IfStatementSyntax, IfStatementState>(
                ActionKind.JumpNext,
                x => x.HasCondition("y == 3"),
                x => x.ActiveBaseNode.HasCondition("y == 7")
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<IfStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void NestedIfStatementDeclaration_JumpPrev_PrevNestedIfStatement()
        {
            AssertTransition<IfStatementSyntax, IfStatementState>(
                ActionKind.JumpPrev,
                x => x.HasCondition("y == 3"),
                x => x.ActiveBaseNode.HasCondition("y == 11")
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<IfStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void NestedIfStatementDeclaration_JumpNextSibling_NextNestedSibling()
        {
            AssertTransition<IfStatementSyntax, ReturnStatementState>(
                ActionKind.JumpNextSibling,
                x => x.HasCondition("y == 3"),
                x => x.ActiveBaseNode.HasExpression("x * 3 + y")
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<IfStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void NestedIfStatementDeclaration_JumpPrevSibling_PrevNestedSibling()
        {
            AssertTransition<IfStatementSyntax, ReturnStatementState>(
                ActionKind.JumpPrevSibling,
                x => x.HasCondition("y == 3"),
                x => x.ActiveBaseNode.HasExpression("x * 3 + y")
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<IfStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void NestedIfStatementDeclaration_JumpUp_ParentIfBody()
        {
            AssertTransition<IfStatementSyntax, IfBodyState>(
                ActionKind.JumpContextUp,
                x => x.HasCondition("y == 3"),
                x => x.ActiveBaseNode
                    .ParentAs<IfStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void NestedIfStatementDeclaration_JumpDown_ConditionMethodBody()
        {
            AssertTransition<IfStatementSyntax, IfConditionState>(
                ActionKind.JumpContextDown,
                x => x.HasCondition("y == 3"),
                x => x.ActiveNodeAsVirtual<IfConditionSyntax>()
                    .Expression.ToString() == "y == 3"
                    && x.ActiveBaseNode
                    .ParentAs<IfStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<IfStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method1"));
        }

        [Fact]
        public void TryStatement_JumpNext_NextTryStatement()
        {
            AssertTransition<TryStatementSyntax, TryStatementState>(
                ActionKind.JumpNext,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 3")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatement_JumpPrev_PrevTryStatement()
        {
            AssertTransition<TryStatementSyntax, TryStatementState>(
                ActionKind.JumpPrev,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatement_JumpNextSibling_NextSibling()
        {
            AssertTransition<TryStatementSyntax, ReturnStatementState>(
                ActionKind.JumpNextSibling,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatement_JumpPrevSibling_PrevSibling()
        {
            AssertTransition<TryStatementSyntax, ReturnStatementState>(
                ActionKind.JumpPrevSibling,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatement_JumpUp_MethodBody()
        {
            AssertTransition<TryStatementSyntax, MethodBodyState>(
                ActionKind.JumpContextUp,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatement_JumpDown_ExpressionStatement()
        {
            AssertTransition<TryStatementSyntax, TryBodyState>(
                ActionKind.JumpContextDown,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveNodeAsVirtual<TryBodySyntax>().Body
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatementNested_JumpNext_NextTryStatement()
        {
            AssertTransition<TryStatementSyntax, TryStatementState>(
                ActionKind.JumpNext,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 17"),
                x => x.ActiveBaseNode.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 171")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatementNested_JumpPrev_PrevTryStatement()
        {
            AssertTransition<TryStatementSyntax, TryStatementState>(
                ActionKind.JumpPrev,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 17"),
                x => x.ActiveBaseNode.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 1711")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatementNested_JumpNextSibling_NextSibling()
        {
            AssertTransition<TryStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpNextSibling,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 17"),
                x => x.ActiveBaseNode.HasExpression("x--")
                        && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatementNested_JumpPrevSibling_PrevSibling()
        {
            AssertTransition<TryStatementSyntax, ThrowStatementState>(
                ActionKind.JumpPrevSibling,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 17"),
                x => x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatementNested_JumpUp_ParentTryBody()
        {
            AssertTransition<TryStatementSyntax, TryBodyState>(
                ActionKind.JumpContextUp,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 17"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryStatementNested_JumpDown_ExpressionStatement()
        {
            AssertTransition<TryStatementSyntax, TryBodyState>(
                ActionKind.JumpContextDown,
                x => x.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 17"),
                x => x.ActiveNodeAsVirtual<TryBodySyntax>().Body
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void ExpressionStatementNestedInBodyTryTry_JumpNext_NextExpressionStatement()
        {
            AssertTransition<ExpressionStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpNext,
                x => x.HasExpression("x += 17"),
                x => x.ActiveBaseNode.HasExpression("x += 99")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void ExpressionStatementNestedInBodyTryTry_JumpPrev_PrevExpressionStatement()
        {
            AssertTransition<ExpressionStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpPrev,
                x => x.HasExpression("x += 17"),
                x => x.ActiveBaseNode.HasExpression("x += 999")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void ExpressionStatementNestedInBodyTryTry_JumpNextSibling_NextSibling()
        {
            AssertTransition<ExpressionStatementSyntax, ThrowStatementState>(
                ActionKind.JumpNextSibling,
                x => x.HasExpression("x += 17"),
                x => x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void ExpressionStatementNestedInBodyTryTry_JumpPrevSibling_PrevSibling()
        {
            AssertTransition<ExpressionStatementSyntax, ThrowStatementState>(
                ActionKind.JumpPrevSibling,
                x => x.HasExpression("x += 17"),
                x => x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void ExpressionStatementNestedInBodyTryTry_JumpUp_ParentTryBody()
        {
            AssertTransition<ExpressionStatementSyntax, TryBodyState>(
                ActionKind.JumpContextUp,
                x => x.HasExpression("x += 17"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x += 17")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void ExpressionStatementNestedInBodyTryTry_JumpDown_NestedExpression()
        {
            AssertTransition<ExpressionStatementSyntax, NestedBlockState>(
                ActionKind.JumpContextDown,
                x => x.HasExpression("x += 17"),
                x => x.ActiveBaseNode.ToString() == "x += 17"
                    && x.ActiveBaseNode
                        .ParentAs<ExpressionStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryBody_JumpNextAndTryFinally_SameTryBody()
        {
            AssertTransition<BlockSyntax, TryBodyState>(
                ActionKind.JumpNext,
                x => x.Parent is TryStatementSyntax
                    && x.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.ParentAs<TryStatementSyntax>().Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryBody_JumpPrevAndTryFinally_SameTryBody()
        {
            AssertTransition<BlockSyntax, TryBodyState>(
                ActionKind.JumpPrev,
                x => x.Parent is TryStatementSyntax
                    && x.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.ParentAs<TryStatementSyntax>().Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryBody_JumpNextSiblingAndTryFinally_FinallyClause()
        {
            AssertTransition<BlockSyntax, FinallyClauseState>(
                ActionKind.JumpNextSibling,
                x => x.Parent is TryStatementSyntax
                    && x.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.ParentAs<TryStatementSyntax>().Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryBody_JumpPrevSiblingAndTryFinally_FinallyClause()
        {
            AssertTransition<BlockSyntax, FinallyClauseState>(
                ActionKind.JumpPrevSibling,
                x => x.Parent is TryStatementSyntax
                    && x.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.ParentAs<TryStatementSyntax>().Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryBody_JumpUpAndTryFinally_FinallyClause()
        {
            AssertTransition<BlockSyntax, TryStatementState>(
                ActionKind.JumpContextUp,
                x => x.Parent is TryStatementSyntax
                    && x.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.Block
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void TryBody_JumpDownAndTryFinally_FirstMethodBodyMemberState()
        {
            AssertTransition<BlockSyntax, ExpressionStatementState>(
                ActionKind.JumpContextDown,
                x => x.Parent is TryStatementSyntax
                    && x.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpNextAndTryFinally_SameFinallyClause()
        {
            AssertTransition<FinallyClauseSyntax, FinallyClauseState>(
                ActionKind.JumpNext,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x = x + 1 + y")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpPrevAndTryFinally_SameFinallyClause()
        {
            AssertTransition<FinallyClauseSyntax, FinallyClauseState>(
                ActionKind.JumpPrev,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x = x + 1 + y")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpNextSiblingAndTryFinally_TryBody()
        {
            AssertTransition<FinallyClauseSyntax, TryBodyState>(
                ActionKind.JumpNextSibling,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpPrevSiblingAndTryFinally_TryBody()
        {
            AssertTransition<FinallyClauseSyntax, TryBodyState>(
                ActionKind.JumpPrevSibling,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpUpAndTryFinally_TryStatement()
        {
            AssertTransition<FinallyClauseSyntax, TryStatementState>(
                ActionKind.JumpContextUp,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpDownAndTryFinally_FirstNestedBodyMember()
        {
            AssertTransition<FinallyClauseSyntax, ExpressionStatementState>(
                ActionKind.JumpContextDown,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x++"),
                x => x.ActiveBaseNode.HasExpression("x = x + 1 + y")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<FinallyClauseSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpNextAndTryCatchFinally_SameFinallyClause()
        {
            AssertTransition<FinallyClauseSyntax, FinallyClauseState>(
                ActionKind.JumpNext,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x = x + 3 + y")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpPrevAndTryCatchFinally_SameFinallyClause()
        {
            AssertTransition<FinallyClauseSyntax, FinallyClauseState>(
                ActionKind.JumpPrev,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x = x + 3 + y")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpNextSiblingAndTryCatchFinally_TryBody()
        {
            AssertTransition<FinallyClauseSyntax, TryBodyState>(
                ActionKind.JumpNextSibling,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpPrevSiblingAndTryCatchFinally_FirstCatchClause()
        {
            AssertTransition<FinallyClauseSyntax, CatchClauseState>(
                ActionKind.JumpPrevSibling,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x = y + 1")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpUpAndTryCatchFinally_TryStatement()
        {
            AssertTransition<FinallyClauseSyntax, TryStatementState>(
                ActionKind.JumpContextUp,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void FinallyClause_JumpDownAndTryCatchFinally_FirstNestedBodyMember()
        {
            AssertTransition<FinallyClauseSyntax, ExpressionStatementState>(
                ActionKind.JumpContextDown,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode.HasExpression("x = x + 3 + y")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<FinallyClauseSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void CatchClause_JumpNextAndTryCatchFinally_NextCatchStatement()
        {
            AssertTransition<CatchClauseSyntax, CatchClauseState>(
                ActionKind.JumpNext,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x = y + 11")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void CatchClause_JumpPrevAndTryCatchFinally_PrevCatchStatement()
        {
            AssertTransition<CatchClauseSyntax, CatchClauseState>(
                ActionKind.JumpPrev,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode.GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x = y + 111")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void CatchClause_JumpNextSiblingAndTryCatchFinally_FinallyClause()
        {
            AssertTransition<CatchClauseSyntax, FinallyClauseState>(
                ActionKind.JumpNextSibling,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x = x + 3 + y")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void CatchClause_JumpPrevSiblingAndTryCatchFinally_TryBody()
        {
            AssertTransition<CatchClauseSyntax, TryBodyState>(
                ActionKind.JumpPrevSibling,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--")
                    && x.ActiveBaseNode
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void CatchClause_JumpUpAndTryCatchFinally_TryStatement()
        {
            AssertTransition<CatchClauseSyntax, TryStatementState>(
                ActionKind.JumpContextUp,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
        }

        [Fact]
        public void CatchClause_JumpDownAndTryCatchFinally_FirstNestedBodyMember()
        {
            AssertTransition<CatchClauseSyntax, ExpressionStatementState>(
                ActionKind.JumpContextDown,
                x => x.Parent is TryStatementSyntax tryStatement
                    && tryStatement
                        .GetFirstChildOfTypeRecursively<ExpressionStatementSyntax>()
                        .HasExpression("x--"),
                x => x.ActiveBaseNode.HasExpression("x = y + 1")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<CatchClauseSyntax>()
                        .ParentAs<TryStatementSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method5"));
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
        public void ExpressionStatement_JumpDown_NestedExpression()
        {
            AssertTransition<ExpressionStatementSyntax, NestedBlockState>(
                ActionKind.JumpContextDown,
                x => x.HasExpression("var (x, y) = this.Method2(2, 3)")
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method3")
                        ?? false),
                x => x.ActiveBaseNode.ToString() == "var (x, y) = this.Method2(2, 3)"
                    && x.ActiveBaseNode
                    .ParentAs<ExpressionStatementSyntax>()
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method3"));
        }

        [Fact]
        public void LocalDeclarationStatement_JumpNext_NextLocalDeclarationStatement()
        {
            AssertTransition<LocalDeclarationStatementSyntax, LocalDeclarationState>(
                ActionKind.JumpNext,
                x => x.ToString() == "var x = this.isTrue;"
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method4")
                        ?? false),
                x => x.ActiveBaseNode.ToString() == "var y = false;"
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method4"));
        }

        [Fact]
        public void LocalDeclarationStatement_JumpPrev_PrevLocalDeclarationStatement()
        {
            AssertTransition<LocalDeclarationStatementSyntax, LocalDeclarationState>(
                ActionKind.JumpPrev,
                x => x.ToString() == "var x = this.isTrue;"
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method4")
                        ?? false),
                x => x.ActiveBaseNode.ToString() == "var z = x && y;"
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method4"));
        }

        [Fact]
        public void LocalDeclarationStatement_JumpNextSibling_NextLocalDeclarationStatement()
        {
            AssertTransition<LocalDeclarationStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpNextSibling,
                x => x.ToString() == "var x = this.isTrue;"
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method4")
                        ?? false),
                x => x.ActiveBaseNode
                        .HasExpression("this.isTrue = string.IsNullOrWhiteSpace(\"\")")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method4"));
        }

        [Fact]
        public void LocalDeclarationStatement_JumpPrevSibling_NextLocalDeclarationStatement()
        {
            AssertTransition<LocalDeclarationStatementSyntax, ExpressionStatementState>(
                ActionKind.JumpPrevSibling,
                x => x.ToString() == "var x = this.isTrue;"
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method4")
                        ?? false),
                x => x.ActiveBaseNode
                        .HasExpression("this.isTrue = string.IsNullOrWhiteSpace(\"\")")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method4"));
        }

        [Fact]
        public void LocalDeclarationStatement_JumpUp_MethodBody()
        {
            AssertTransition<LocalDeclarationStatementSyntax, MethodBodyState>(
                ActionKind.JumpContextUp,
                x => x.ToString() == "var x = this.isTrue;"
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method4")
                        ?? false),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>() != null
                    && x.ActiveBaseNode
                        .ParentAs<MethodDeclarationSyntax>().HasName("Method4"));
        }

        [Fact]
        public void LocalDeclarationStatement_JumpDown_SameLocalDeclarationStatement()
        {
            AssertTransition<LocalDeclarationStatementSyntax, LocalDeclarationState>(
                ActionKind.JumpContextDown,
                x => x.ToString() == "var x = this.isTrue;"
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method4")
                        ?? false),
                x => x.ActiveBaseNode.ToString() == "var x = this.isTrue;"
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>().HasName("Method4"));
        }

        [Fact]
        public void NestedBlock_JumpNext_NextNestedBlock()
        {
            AssertTransition<BlockSyntax, NestedBlockState>(
                ActionKind.JumpNext,
                x => x.ChildNodes().OfType<ExpressionStatementSyntax>()
                        .SingleOrDefault(e => e.HasExpression("z += 3")) != null
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method6")
                        ?? false),
                x => x.ActiveNodeAsVirtual<NestedBlockSyntax>().BlockBody?.ChildNodes()
                        .OfType<ExpressionStatementSyntax>()
                        .SingleOrDefault(e => e.HasExpression("z += 4")) != null
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method6")); ;
        }

        [Fact]
        public void NestedBlock_JumpPrev_PrevNestedBlock()
        {
            AssertTransition<BlockSyntax, NestedBlockState>(
                ActionKind.JumpPrev,
                x => x.ChildNodes().OfType<ExpressionStatementSyntax>()
                        .SingleOrDefault(e => e.HasExpression("z += 3")) != null
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method6")
                        ?? false),
                x => x.ActiveNodeAsVirtual<NestedBlockSyntax>().BlockBody?.ChildNodes()
                        .OfType<ExpressionStatementSyntax>()
                        .SingleOrDefault(e => e.HasExpression("z += 5")) != null
                    && x.ActiveBaseNode
                    .ParentAs<BlockSyntax>()
                    .ParentAs<MethodDeclarationSyntax>()
                    .HasName("Method6")); ;
        }

        [Fact]
        public void NestedBlock_JumpNextSibling_NextLocalDeclarationStatement()
        {
            AssertTransition<BlockSyntax, ExpressionStatementState>(
                ActionKind.JumpNextSibling,
                x => x.ChildNodes().OfType<ExpressionStatementSyntax>()
                        .SingleOrDefault(e => e.HasExpression("z += 3")) != null
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method6")
                        ?? false),
                x => x.ActiveBaseNode.HasExpression("y++")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method6"));
        }

        [Fact]
        public void NestedBlock_JumpPrevSibling_NextLocalDeclarationStatement()
        {
            AssertTransition<BlockSyntax, LocalDeclarationState>(
                ActionKind.JumpPrevSibling,
                x => x.ChildNodes().OfType<ExpressionStatementSyntax>()
                        .SingleOrDefault(e => e.HasExpression("z += 3")) != null
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method6")
                        ?? false),
                x => x.ActiveBaseNode
                        .HasDeclaration("int y = 3")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>()
                        .HasName("Method6"));
        }

        [Fact]
        public void NestedBlock_JumpUp_MethodBody()
        {
            AssertTransition<BlockSyntax, MethodBodyState>(
                ActionKind.JumpContextUp,
                x => x.ChildNodes().OfType<ExpressionStatementSyntax>()
                        .SingleOrDefault(e => e.HasExpression("z += 3")) != null
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method6")
                        ?? false),
                x => x.ActiveNodeAsVirtual<MethodBodyDeclarationSyntax>() != null
                    && x.ActiveBaseNode
                        .ParentAs<MethodDeclarationSyntax>().HasName("Method6"));
        }

        [Fact]
        public void NestedBlock_JumpDown_FirstNestedMethodBodyMember()
        {
            AssertTransition<BlockSyntax, LocalDeclarationState>(
                ActionKind.JumpContextDown,
                x => x.ChildNodes().OfType<ExpressionStatementSyntax>()
                        .SingleOrDefault(e => e.HasExpression("z += 3")) != null
                    && (x.GetFirstParentOfType<MethodDeclarationSyntax>()?.HasName("Method6")
                        ?? false),
                x => x.ActiveBaseNode.HasDeclaration("var z = x + 4")
                    && x.ActiveBaseNode
                        .ParentAs<BlockSyntax>()
                        .ParentAs<BlockSyntax>()
                        .ParentAs<MethodDeclarationSyntax>().HasName("Method6"));
        }
    }
}
