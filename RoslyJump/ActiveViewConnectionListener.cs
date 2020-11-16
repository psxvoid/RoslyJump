using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace RoslyJump
{
    public interface IActiveViewAccessor
    {
        IWpfTextView ActiveView { get; }
    }

    [Export(typeof(IWpfTextViewConnectionListener))]
    [Export(typeof(IActiveViewAccessor))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class ActiveViewConnectionListener : IWpfTextViewConnectionListener, IActiveViewAccessor
    {
        public IWpfTextView ActiveView { get; private set; }

        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            this.ActiveView = textView;
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            this.ActiveView = null;
        }
    }
}
