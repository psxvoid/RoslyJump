using Microsoft.VisualStudio.Text.Editor;

namespace RoslyJump.Shared.Infrastructure.VisualStudio.TextEditor
{
    public interface IActiveViewAccessor
    {
        IWpfTextView? ActiveView { get; }
    }
}
