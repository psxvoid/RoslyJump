using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace RoslyJump.Core.xUnit.Integration.Fixtures
{
    [CollectionDefinition(nameof(StateTransitionFixture))]
    public class StateTransitionXUnitCollection : ICollectionFixture<StateTransitionFixture>
    {
    }

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
                        if (x == 12 || y == 11)
                        {
                            return 12345;
                        }
                        else if (x == y / x + 12 && x != 0)
                        {
                            return 123456;
                        }

                        int fu(int x) => x + 5;
                        int fuu(int x) => x + 6;

                        if (fu(5) < 10 && fuu(5) < 11)
                        {
                            return fu(5) + fuu(10);
                        }
                        else
                        {
                            return fu(14) + fuu(11);
                        }

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
                else
                {
                    return x * 8 + y;
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
                    try
                    {
                        x += 17;
                        x += 99;
                        x += 999;
                        throw new Exception();
                    }
                    catch (ArgumentNullException e)
                    {
                        x = y + 101;
                    }
                    catch (InvalidCastException e)
                    {
                        x = y + 111;
                    }
                    finally
                    {
                        x = x + 33 + 7;
                    }

                    try
                    {
                        x += 171;
                    }
                    finally
                    {
                        x = x + 33 + 71;
                    }

                    try
                    {
                        x += 1711;
                    }
                    finally
                    {
                        x = x + 33 + 711;
                    }
                }
                catch (InvalidOperationException e)
                {
                    x = y + 1;
                }
                catch (ArgumentException e)
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

            public void Method6(int x)
            {
                int y = 3;

                // nested BlockSyntax (NestedBlockState)
                {
                    var z = x + 4;
                    z++;
                    z--;
                    z += 3;
                    z += y;
                }

                // nested BlockSyntax (NestedBlockState)
                {
                    var z = x + 5;
                    z++;
                    z--;
                    z += 4;
                    z += y;
                }

                // nested BlockSyntax (NestedBlockState)
                {
                    var z = x + 6;
                    z++;
                    z--;
                    z += 5;
                    z += y;
                }

                y++;
                y--;
            }

            public void Method7(byte[] buffer)
            {
                using (Stream stream1 = new MemoryStream())
                {
                    stream1.Write(buffer);
                    stream1.Flush();
                }

                using Stream stream2 = new MemoryStream();

                stream2.Write(buffer);
                stream2.Flush();


                using (Stream stream3 = new MemoryStream())
                {
                    using (Stream stream31 = new MemoryStream())
                    {
                        stream31.Write(buffer);
                        stream31.Flush();
                    }

                    stream3.Write(buffer);
                    stream3.Flush();
                }

                var reader = new StringReader(string.Empty);

                using (reader)
                {
                    string? item;
                    do
                    {
                        item = reader.ReadLine();
                        Console.WriteLine(item);
                    } while (item != null);
                }

                using Stream stream4 = new MemoryStream();

                stream4.Write(buffer);
                stream4.Flush();

                using (Stream stream5 = new MemoryStream())
                using (var stream51 = new StreamWriter(stream5))
                {
                    stream51.Write(buffer);
                    stream51.Flush();
                }
            }

            int Method8()
            {
                int x = 5;

                start:
                goto lockStart;

                ucheckedStart:
                unchecked
                {
                    x += int.MaxValue;
                }

                lockStart:
                lock(this)
                {
                    return this.Method1(1, 2);
                }
                ; ; ;
                {
                    goto start;
                }

                goto ucheckedStart;
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
