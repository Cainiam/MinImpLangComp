using MinImpLangComp.AST;
using MinImpLangComp.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace MinImpLangComp.ILGeneration
{
    /// <summary>
    /// Hosts dynamic-method execution paths used by the IL backend.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="RunScript(List{Statement})"/> is the execution path used by the public facade: it does <b>not</b> consume <see cref="RuntimeIO"/> and returns <c>void</c>.
    /// </para>
    /// <para>
    /// The <c>GenerateAndRunIL</c> overloads are legacy/testing helpers used by unit tests: if the last evaluated statement does not produce a value, they consume the buffered output from <see cref="RuntimeIO"/> and (for convenience) also echo it to <see cref="Console.Out"/>.
    /// </para>
    /// </remarks>
    public static class ILGeneratorRunner
    {
        /// <summary>
        /// Executes a list of statements as a script body (facade code path).
        /// </summary>
        /// <param name="statements">The statements to emit and run.</param>
        public static void RunScript(List<Statement> statements)
        {
            var method = new DynamicMethod("ScriptMain", typeof(void), Type.EmptyTypes, typeof(ILGeneratorRunner).Module, true);
            var il = method.GetILGenerator();
            var locals = new Dictionary<string, LocalBuilder>();
            var constants = new HashSet<string>();
            foreach (var statement in statements) ILGeneratorUtils.GenerateIL(statement, il, locals, constants);
            il.Emit(OpCodes.Ret);
            var action = (Action)method.CreateDelegate(typeof(Action));
            action();
        }

        /// <summary>
        /// Test helper: emits a dynamic method for a list of statements and returns either the value of the last variable/expression or the consumed <see cref="RuntimeIO"/> output (as string) when no value is produced.
        /// </summary>
        /// <param name="statements">The statement to generate and run.</param>
        /// <remarks>
        /// Also writes the consumed output to <see cref="Console.Out"/> (trimed) when returning the string branch.
        /// </remarks>
        /// <returns>Value of the <see cref="List{T}"/> <see cref="Statement"/> given.</returns>
        public static object? GenerateAndRunIL(List<Statement> statements)
        {
            // Set-up 
            Dictionary<string, MethodInfo>? fnRegistry = null;
            var method = new DynamicMethod("Eval", typeof(object), Type.EmptyTypes);
            var il = method.GetILGenerator();
            var locals = new Dictionary<string, LocalBuilder>();
            var constants = new HashSet<string>();

            // Track last assignable variable to return
            string? lastVariableToReturn = null;

            try
            {
                // Build functions and reset output buffer
                fnRegistry = ILGeneratorUtils.BuildAndRegisterFunctions(statements);
                RuntimeIO.Clear();

                // Emit statements
                for (int i = 0; i < statements.Count; i++)
                {
                    var statement = statements[i];
                    ILGeneratorUtils.GenerateIL(statement, il, locals, constants);
                    if (i == statements.Count - 1)
                    {
                        lastVariableToReturn = statement switch
                        {
                            VariableDeclaration variableDeclaration => variableDeclaration.Identifier,
                            Assignment assignment => assignment.Identifier,
                            ExpressionStatement expressionStatement when expressionStatement.Expression is VariableReference variableReference => variableReference.Name,
                            _ => null
                        };
                    }
                }

                // Load return value if present; otherwise push null
                if (lastVariableToReturn != null && locals.TryGetValue(lastVariableToReturn, out var local))
                {
                    il.Emit(OpCodes.Ldloc, local);
                    if (local.LocalType.IsValueType) il.Emit(OpCodes.Box, local.LocalType);
                }
                else il.Emit(OpCodes.Ldnull);

                // Return
                il.Emit(OpCodes.Ret);

                // Invoke
                var del = (Func<object?>)method.CreateDelegate(typeof(Func<object?>));
                var result = del();

                // If null result, consume buffered output and also echo to Console (trimmed)
                if(result == null)
                {
                    var output = RuntimeIO.Consume();
                    if (!string.IsNullOrEmpty(output)) Console.Write(output.TrimEnd('\r', '\n'));
                    return output;
                }
                return result;
            }
            finally
            {
                // Ensure function registry is cleared after the run
                ILGeneratorUtils.ClearFunctionRegistry();
            }
        }

        /// <summary>
        /// Test helper: same as <see cref="GenerateAndRunIL(List{Statement})", but with an optional redirected stdin content./>
        /// </summary>
        /// <param name="statements">The statemetns to generate and run</param>
        /// <param name="input">Emulated input from an user.</param>
        /// <returns>Value of the <see cref="List{T}"/> <see cref="Statement"/> given.</returns>
        public static object? GenerateAndRunIL(List<Statement> statements, string? input)
        {
            var originalIn = Console.In;
            try
            {
                if (input != null) Console.SetIn(new StringReader(input));
                return GenerateAndRunIL(statements);
            }
            finally
            {
                Console.SetIn(originalIn);
            }
        }

        /// <summary>
        /// Test helper: emits a dynamic method for a single expression and returns its value; when the evulation yields null, consumes the <see cref="RuntimeIO"/> output and echoes it to <see cref="Console.Out"/>.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Value of the <see cref="Expression"/> given.</returns>
        public static object? GenerateAndRunIL(Expression expression)
        {
            var method = new DynamicMethod("Eval", typeof(object), Type.EmptyTypes);
            var il = method.GetILGenerator();
            var locals = new Dictionary<string, LocalBuilder>();
            var constants = new HashSet<string>();
            ILGeneratorUtils.GenerateIL(expression, il, locals, constants);
            switch(expression)
            {
                case IntegerLiteral:
                    il.Emit(OpCodes.Box, typeof(int));
                    break;
                case FloatLiteral:
                    il.Emit(OpCodes.Box, typeof(double));
                    break;
                case BooleanLiteral:
                    il.Emit(OpCodes.Box, typeof(bool));
                    break;
                case BinaryExpression:
                    if (ContainsFloat(expression)) il.Emit(OpCodes.Box, typeof(double));
                    else il.Emit(OpCodes.Box, typeof(int));
                    break;
            }
            il.Emit(OpCodes.Ret);
            var del = (Func<object?>)method.CreateDelegate(typeof(Func<object?>));
            var result = del();
            if(result == null)
            {
                var output = RuntimeIO.Consume();
                if (!string.IsNullOrEmpty(output)) Console.Write(output.TrimEnd('\r', '\n'));
            }
            return result;
        }

        /// <summary>
        /// Returns true if <paramref name="expression"/> contains a float literal (directly or nested) - used to box arithmetic results properly.
        /// </summary>
        /// <param name="expression">Expression to check if it contains a float literal</param>
        /// <returns><c>True</c> if <see cref="Expression"/> contains a float lieral, if not <c>false</c></returns>
        public static bool ContainsFloat(Expression expression)
        {
            return expression switch
            {
                FloatLiteral => true,
                BinaryExpression binary => ContainsFloat(binary.Left) || ContainsFloat(binary.Right),
                _ => false
            };
        }
    }
}
