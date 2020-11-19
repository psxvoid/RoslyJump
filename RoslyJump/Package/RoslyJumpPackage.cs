using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using RoslyJump.Core;
using RoslyJump.Core.Contexts.Local;
using RoslyJump.Package;
using Task = System.Threading.Tasks.Task;

namespace RoslyJump
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [Guid(PackageIds.PackageGuidString)]
    // VSConstants.UICONTEXT.NoSolution_string
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class RoslyJumpPackage : AsyncPackage
    {
        #region MEF Providers
        private IComponentModel componentModel;
        private ExportProvider exportProvider;
        #endregion

        private IActiveViewAccessor viewAccessor;

        private TextAdornment1 Adorment = null;


        LocalContext LocalContext;
        private IWpfTextView lastActiveView;

        private void ContextJumpNext()
        {
            if (this.viewAccessor.ActiveView != null && this.lastActiveView != this.viewAccessor?.ActiveView)
            {
                if (this.Adorment != null)
                {
                    this.Adorment.Remove();
                }

                this.Adorment = new TextAdornment1(this.viewAccessor.ActiveView);
                this.lastActiveView = this.viewAccessor.ActiveView;

                string text = this.viewAccessor.ActiveView.TextSnapshot.GetText();
                SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
                this.LocalContext = new LocalContext(tree);
            }

            var view = this.viewAccessor?.ActiveView;

            if (this.Adorment != null && view != null)
            {
                this.Adorment.Remove();

                SnapshotPoint caret = this.viewAccessor
                    .ActiveView.Caret.Position.BufferPosition;
                IWpfTextViewLine textViewLine = view.GetTextViewLineContainingBufferPosition(caret);
                int line = caret.GetContainingLine().LineNumber;
                int startChar = textViewLine.Start.Difference(caret);

                this.LocalContext.TransitionTo(line, startChar);

                LocalContextState state = this.LocalContext.State;

                state.JumpNext();

                if (state.IsJumpTargetSet)
                {
                    // TODO: support cross-line definitions
                    this.Adorment.EndorseLine(
                        state.JumpTargetStartLine,
                        state.JumpTargetStartChar,
                        state.JumpTargetEndChar);

                    Microsoft.VisualStudio.Text.Formatting.IWpfTextViewLine viewLine =
                        this.viewAccessor.ActiveView.TextViewLines[state.JumpTargetStartLine];

                    Microsoft.VisualStudio.Text.SnapshotPoint jumpPoint = viewLine.Start.Add(state.JumpTargetStartChar);
                    this.viewAccessor.ActiveView.Caret.MoveTo(jumpPoint);
                }
            }
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            object componentModel = await GetServiceAsync(typeof(SComponentModel));

            OleMenuCommandService mcs = await this.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Debug.Assert(mcs != null);

            if (null != mcs)
            {
                //// Create the command for the menu item.
                var menuCommandID = new CommandID(PackageIds.CommandGroup, (int)CommandIds.ContextJumpNext);
                var menuItem = new MenuCommand((sender, evt) =>
                {
                    ContextJumpNext();
                }, menuCommandID);

                mcs.AddCommand(menuItem);
            }

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            this.componentModel = (IComponentModel)componentModel;
            this.exportProvider = this.componentModel.DefaultExportProvider;

            this.viewAccessor = this.exportProvider.GetExportedValue<IActiveViewAccessor>();
        }

        #endregion
    }
}
