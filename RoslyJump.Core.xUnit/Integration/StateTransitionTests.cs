using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using dngrep.core.Extensions.SyntaxTreeExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration
{
    public class StateTransitionTests
    {
#pragma warning disable
        private class C1
        {
            private int field1 = 2;

            public int Method1(int x, int y)
            {
                if (x == 3)
                {
                    return x * 3 + y;
                }
                else if (x == 4)
                {
                    return x * 6 + y;
                }
                else if (x == 5)
                {
                    return x * 7 + y;
                }

                return 0;
            }

            public (int x, int y) Method2(int x, int y)
            {
                return (x, y);
            }

            public void Method3()
            {
                return;
            }

            public int Prop1 => field1;
        }
#pragma warning restore

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
            AssertTransition<MethodDeclarationSyntax, PropertyDeclarationState>(
                ActionKind.JumpNextSibling,
                x => x.HasName("Method1")
                    && (x.GetFirstParentOfType<ClassDeclarationSyntax>()?.HasName("C1") ?? false),
                x => x.ActiveNodeAs<PropertyDeclarationSyntax>().HasName("Prop1"));
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


        private enum ActionKind
        {
            JumpNext,
            JumpPrev,
            JumpNextSibling,
            JumpPrevSibling,
            JumpContextUp,
            JumpContextDown,
        }

        private static void AssertTransition<TStartPositionNode, TExpectedState>(
            ActionKind action,
            Func<TStartPositionNode, bool>? startNodePredicate = null,
            Func<TExpectedState, bool>? statePredicate = null,
            [CallerFilePath] string sourceFilePath = "")
            where TStartPositionNode : SyntaxNode
            where TExpectedState : LocalContextState
        {
            string sourceText = File.ReadAllText(sourceFilePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            LocalContext context = new LocalContext(tree);

            TStartPositionNode? node = tree.GetRoot().ChildNodes()
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
