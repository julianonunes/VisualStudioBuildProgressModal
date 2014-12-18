using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using EnvDTE;

namespace Vodia.Vodia_BuildProgress
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidVodia_BuildProgressPkgString)]
    public sealed class BuildProgressPackage : Package
    {private BuildProgress _buildProgress;
        private DTE2 _ide;

        private BuildEventListener BuildEventListener { get; set; }
        private SolutionEventListener SolutionEventListener { get; set; }

        private BuildProgress Progress
        {
            get { return _buildProgress ?? (_buildProgress = new BuildProgress(this)); }
        }

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Gets the top level application instance of the VS IDE that is executing this package.
        /// </summary>
        public DTE2 IDE
        {
            get { return _ide ?? (_ide = (DTE2) GetService(typeof(DTE))); }
        }

        #endregion

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public BuildProgressPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));
            base.Initialize();

            registerBuildEvents();
            registerSolutionEvents();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        ///  Registers the package build events.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------
        private void registerBuildEvents()
        {
            BuildEventListener = new BuildEventListener(this);

            BuildEventListener.BuildBegin += Progress.OnBuildBegin;
            BuildEventListener.BuildProjConfigBegin += Progress.OnBuildProjConfigBegin;
            BuildEventListener.BuildProjConfigDone += Progress.OnBuildProjConfigDone;
            BuildEventListener.BuildDone += Progress.OnBuildDone;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        ///  Registers the package solution events.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------
        private void registerSolutionEvents()
        {
            SolutionEventListener = new SolutionEventListener(this);

            SolutionEventListener.AfterClosing += solutionEventListener_AfterClosing;
        }

        private void solutionEventListener_AfterClosing()
        {
            //Progress.RemoveTaskbarProgressState();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Dispose of any event listeners.
            if (BuildEventListener != null)
            {
                BuildEventListener.Dispose();
            }

            if (SolutionEventListener != null)
            {
                SolutionEventListener.Dispose();
            }
        }

    }
}
