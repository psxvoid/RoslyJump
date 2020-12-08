using System;

namespace RoslyJump.Core.Infrastructure.Helpers.Reflection
{
    internal static class GenericTypeMatching
    {
        public static bool IsInheritedFromType(this Type target, Type baseType)
        {
            _ = target ?? throw new ArgumentNullException(nameof(target));

            while (target != null)
            {
                if (target == baseType)
                {
                    return true;
                }

                target = target.BaseType;
            }

            return false;
        }
    }
}
