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

            public int Method1(int x, int y)
            {
                if (x == 3)                                 // IfDeclarationStatementSyntax
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
