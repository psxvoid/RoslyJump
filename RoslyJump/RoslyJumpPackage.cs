﻿using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using RoslyJump.PackageD;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
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
    // [ProvideService(typeof(IActiveViewAccessor), IsAsyncQueryable = true)]
    public sealed class RoslyJumpPackage : AsyncPackage
    {
        #region MEF Providers
        private IComponentModel componentModel;
        private ExportProvider exportProvider;
        #endregion

        private IActiveViewAccessor viewAccessor;

        private TextAdornment1 Adorment = null;

        private void ContextJumpNext()
        {
            if (this.Adorment == null && this.viewAccessor?.ActiveView != null)
            {
                this.Adorment = new TextAdornment1(this.viewAccessor.ActiveView);
            }

            if (this.Adorment != null)
            {
                this.Adorment.EndorseActiveLine();
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
                    // Do stuff
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
