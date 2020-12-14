﻿using System.Collections.Generic;

namespace RoslyJump.Core.Infrastructure.Helpers.Generics
{
    public static class CollectionNullableOperations
    {
        public static void AddIfNotNull<T>(this ICollection<T> collection, T item)
        {
            if (item != null)
            {
                collection.Add(item);
            }
        }
    }
}
