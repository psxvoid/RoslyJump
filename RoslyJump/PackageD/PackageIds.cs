using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslyJump.PackageD
{
    /// <summary>
    /// Contains package ids. Those values should match values in .vsct file.
    /// </summary>
    internal static class PackageIds
    {

        internal const string PackageGuidString = "20835a5e-c89f-4dac-aaa6-8bc279eeb0ef";
        internal const string CommandGroupGuidString = "9770c766-52be-419a-85f1-f839c5f36c94";

        internal static readonly Guid PackageGuid = new Guid(PackageGuidString);
        internal static readonly Guid CommandGroup = new Guid(CommandGroupGuidString);
    }
}
