using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace RoslyJump.Infrastructure.VisualStudio.TextEditor
{
#nullable enable
    public interface IActiveViewAccessor
    {
        IWpfTextView? ActiveView { get; }
    }

    [Export(typeof(IActiveViewAccessor))]
    internal sealed class ActiveViewAccessor : IActiveViewAccessor
    {
        private readonly SVsServiceProvider serviceProvider;
        private readonly IVsEditorAdaptersFactoryService editorAdaptersFactoryService;

        [ImportingConstructor]
        public ActiveViewAccessor(
            SVsServiceProvider vsServiceProvider,
            IVsEditorAdaptersFactoryService editorAdaptersFactoryService)
        {
            serviceProvider = vsServiceProvider;
            this.editorAdaptersFactoryService = editorAdaptersFactoryService;
        }

        public IWpfTextView? ActiveView
        {
            get
            {
                IVsTextManager2 textManager =
                    serviceProvider.GetService<SVsTextManager, IVsTextManager2>();

                if (textManager == null)
                {
                    return null;
                }

                int hr = textManager.GetActiveView2(
                    fMustHaveFocus: 1,
                    pBuffer: null,
                    grfIncludeViewFrameType: (uint)_VIEWFRAMETYPE.vftCodeWindow,
                    ppView: out IVsTextView vsTextView);

                if (ErrorHandler.Failed(hr))
                {
                    return null;
                }

                return editorAdaptersFactoryService.GetWpfTextView(vsTextView);
            }
        }
    }
#nullable disable
}
