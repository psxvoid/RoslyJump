using System;
using dngrep.core.VirtualNodes;
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
            _ = state ?? throw new ArgumentNullException(nameof(state));

            if (!(state.ActiveNode?.BaseNode is T castedValue))
            {
                throw new InvalidCastException("Unable to cast the target node.");
            }

            return castedValue;
        }

        /// <summary>
        /// The same as
        /// <see cref="LocalContextStateExtensions.ActiveNodeAs{T}(LocalContextState)"/>
        /// but for getting the virtual node.
        /// </summary>
        /// <typeparam name="T">The type of virtual node to get.</typeparam>
        /// <param name="state">
        /// The state from which the <see cref="LocalContextState.ActiveNode"/>
        /// should be retrieved and casted.
        /// </param>
        /// <returns>
        /// The casted <see cref="LocalContextState.ActiveNode"/> to the specified type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when active node of the <see cref="LocalContextState"/>
        /// is not initialized or isn't virtual.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown when the <see cref="LocalContextState.ActiveNode"/> cannot be
        /// casted to the specified type.
        /// </exception>
        public static T ActiveNodeAsVirtual<T>(this LocalContextState state)
            where T : IVirtualSyntaxNode
        {
            _ = state ?? throw new ArgumentNullException(nameof(state));

            if (state.ActiveNode == null)
            {
                throw new InvalidOperationException(
                    "The active node is missing on the provided state.");
            }

            if (!state.ActiveNode.Value.IsVirtual)
            {
                throw new InvalidOperationException(
                    "The active node of the provided state is not a virtual node.");
            }

            if (!(state.ActiveNode.Value.MixedNode is T castedValue))
            {
                throw new InvalidCastException(
                    "The active node type doesn't match the requested one. " +
                    $"The actual type: {state.ActiveNode.Value.MixedNode.GetType()}. " +
                    $"The expected type: {typeof(T)}.");
            }

            return castedValue;
        }
    }
}
