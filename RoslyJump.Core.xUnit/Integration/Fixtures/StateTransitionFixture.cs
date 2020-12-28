using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslyJump.Core.xUnit.Integration.Fixtures
{
    public class StateTransitionFixture
    {
        public SyntaxTree SyntaxTree { get; }

        #region CodeUnderTheTest

#pragma warning disable
        private class C1
        {
            private int field1 = 2;

            private bool isTrue = false;

            public int Method1(int x, int y)
            {
                if (x == 3)                                 // IfDeclarationStatementSyntax
                {
                    if (y == 3)
                    {
                        y = y + 1;
                    }

                    if (y == 7)
                    {
                        y = y + 2;
                    }

                    if (y == 11)
                    {
                        y = y + 4;
                    }

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

                if (x == 6)
                {
                    return -1;
                }

                return 0;                                   // ReturnStatementSyntax
            }

            public (int x, int y) Method2(int x, int y)
            {
                var z = 5;                                  // LocalDeclarationStatementSyntax
                return (x, y);
            }

            public int Method3()
            {
                var (x, y) = this.Method2(2, 3);            // ExpressionStatementSyntax
                var (v1, v2) = this.Method2(3, 3);          // ExpressionStatementSyntax
                var (v3, v4) = this.Method2(4, 3);          // ExpressionStatementSyntax
                int z = 3;                                  // LocalDeclarationSyntax
                return y + v2 + z;
            }

            public void Method4()
            {
                var x = this.isTrue;                        // LocalDeclarationStatementSyntax
                var y = false;                              // LocalDeclarationStatementSyntax
                var z = x && y;                             // LocalDeclarationStatementSyntax

                this.isTrue = string.IsNullOrWhiteSpace("");// ExpressionStatementSyntax
            }

            public int Method5(int x, int y)
            {
                try
                {
                    x++;
                }
                finally
                {
                    x = x + 1 + y;
                }

                try
                {
                    x += 3;
                }
                catch
                {
                    x = x + 2 + y;
                }

                try
                {
                    x--;
                    throw new InvalidDataException();
                }
                catch(InvalidOperationException e)
                {
                    x = y + 1;
                }
                catch(ArgumentException e)
                {
                    x = y + 11;
                }
                catch (Exception e)
                {
                    x = y + 111;
                }
                finally
                {
                    x = x + 3 + y;
                }

                return x;
            }

            public string this[int i]
            {
                get
                {
                    return $"Indexer1: {i}";
                }

                set
                {
                    this.field1 = i;
                }
            }

            public string this[string str] => $"Indexer2: {str}";

            public string this[object o]
            {
                get
                {
                    return $"Indexer3: {o.GetType()}";
                }
            }

            public int Prop1 => field1;
        }
#pragma warning restore

        #endregion

        public StateTransitionFixture()
        {
            string path = GetSourceTextFilePath();
            string sourceText = File.ReadAllText(path);
            this.SyntaxTree = CSharpSyntaxTree.ParseText(sourceText);
        }

        private static string GetSourceTextFilePath([CallerFilePath] string sourceFilePath = "")
        {
            return sourceFilePath;
        }
    }
}
