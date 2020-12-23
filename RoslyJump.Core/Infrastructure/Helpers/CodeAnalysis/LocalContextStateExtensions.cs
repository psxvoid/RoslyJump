using System;
using Microsoft.CodeAnalysis;
using RoslyJump.Core.Contexts.Local;

namespace RoslyJump.Core.Infrastructure.Helpers.CodeAnalysis
{
    public static class LocalContextStateExtensions
    {
        /// <summary>
        /// Retrieves the <see cref="LocalContextState.ActiveNode"/>
        /// for the specified <see cref="LocalContextState"/> and cast it
        /// to the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type to which <see cref="LocalContextState.ActiveNode"/>
        /// should be casted.
        /// </typeparam>
        /// <param name="state">
        /// The state from which the <see cref="LocalContextState.ActiveNode"/>
        /// should be retrieved and casted.
        /// </param>
        /// <returns>
        /// The casted <see cref="LocalContextState.ActiveNode"/> to the specified type.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Thrown when the <see cref="LocalContextState.ActiveNode"/> cannot be
        /// casted to the specified type.
        /// </exception>
        public static T ActiveNodeAs<T>(this LocalContextState state)
            where T : SyntaxNode
        {
            if (!(state.ActiveNode?.BaseNode is T castedValue))
            {
                throw new InvalidCastException("Unable to cast the target node.");
            }

            return castedValue;
        }
    }
}
