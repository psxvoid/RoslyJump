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
#nullable enable

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
        private IComponentModel? componentModel;
        private ExportProvider? exportProvider;
        #endregion

        private IActiveViewAccessor? viewAccessor;

        private TextAdornment1? Adornment = null;


        private LocalContext? LocalContext;
        private IWpfTextView? lastActiveView;

        private void ContextJumpNext()
        {
            UpdateContextAndJump((state) => state.JumpNext());
        }

        private void ContextJumpPrev()
        {
            UpdateContextAndJump((state) => state.JumpPrev());
        }

        private void ContextJumpUp()
        {
            UpdateContextAndJump((state) => state.JumpContextUp());
        }

        private void ContextJumpNextSubling()
        {
            UpdateContextAndJump((state) => state.JumpToNextSiblingContext());
        }

        private void UpdateContextAndJump(Action<LocalContextState> jumpAction)
        {

            var view = this.viewAccessor?.ActiveView;

            if (view != null && this.lastActiveView != view)
            {
                if (this.Adornment != null)
                {
                    this.Adornment.Remove();
                }

                this.Adornment = new TextAdornment1(view);
                this.lastActiveView = view;

                string text = view.TextSnapshot.GetText();
                SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
                this.LocalContext = new LocalContext(tree);
            }

            if (this.Adornment != null && view != null)
            {
                _ = this.LocalContext ?? throw new NullReferenceException(
                    "The local context should be initialized first.");

                this.Adornment.Remove();

                SnapshotPoint caret = view.Caret.Position.BufferPosition;
                IWpfTextViewLine textViewLine = view.GetTextViewLineContainingBufferPosition(caret);
                int line = caret.GetContainingLine().LineNumber;
                int startChar = textViewLine.Start.Difference(caret);

                this.LocalContext.TransitionTo(line, startChar);

                jumpAction(this.LocalContext.State);

                LocalContextState state = this.LocalContext.State;

                if (state.IsJumpTargetSet)
                {
                    //this.Adornment.EndorseTextBounds(
                    //    state.JumpTargetStartLine,
                    //    state.JumpTargetEndLine,
                    //    state.JumpTargetStartChar,
                    //    state.JumpTargetEndChar);

                    ITextSnapshotLine jumpTargetLine = view.TextSnapshot
                        .GetLineFromLineNumber(state.JumpTargetStartLine);
                    SnapshotPoint jumpPoint = jumpTargetLine.Start.Add(state.JumpTargetStartChar);

                    view.Caret.MoveTo(new SnapshotPoint(view.TextSnapshot, jumpPoint));
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

            OleMenuCommandService? mcs = await this.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Debug.Assert(mcs != null);

            if (null != mcs)
            {
                mcs.AddCommand(
                    CreateMenuCommand((int)CommandIds.ContextJumpNext, ContextJumpNext));
                mcs.AddCommand(
                    CreateMenuCommand((int)CommandIds.ContextJumpPrev, ContextJumpPrev));
                mcs.AddCommand(
                    CreateMenuCommand((int)CommandIds.ContextJumpUp, ContextJumpUp));
                mcs.AddCommand(
                    CreateMenuCommand((int)CommandIds.ContextJumpNextSibling, ContextJumpNextSubling));
            }

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            this.componentModel = (IComponentModel)componentModel;
            this.exportProvider = this.componentModel.DefaultExportProvider;

            this.viewAccessor = this.exportProvider.GetExportedValue<IActiveViewAccessor>();
        }

        #endregion

        private static MenuCommand CreateMenuCommand(int commandId, Action action)
        {
            var menuCommandID = new CommandID(PackageIds.CommandGroup, commandId);

            return new MenuCommand((sender, evt) => action(), menuCommandID);
        }
    }

#nullable disable
}
