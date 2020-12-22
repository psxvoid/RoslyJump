using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslyJump.Core.Contexts.ActiveFile.Local.States.BaseStates;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.States
{
    public class EventFieldDeclarationState
        : EventClassMemberStateBase<EventFieldDeclarationSyntax>
    {
        public EventFieldDeclarationState(LocalContext context, CombinedSyntaxNode contextNode)
            : base(context, contextNode)
        {
        }
    }
}
