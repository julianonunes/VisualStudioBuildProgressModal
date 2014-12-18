#region Copyright (C) 2014, Juliano Nunes Silva Oliveira

// This is free software: you can redistribute it and/or modify
// it under the terms of the Apache License 2.0 (Apache)
// as published by the Free Software Foundation.
// It is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the Apache License 2.0 for more details <http://www.apache.org/licenses/>

// Code based on the TaskbarBuildProgress extension by Cory Cissel (https://taskbarbuildprogress.codeplex.com).
#endregion

using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodia.Vodia_BuildProgress
{
    internal class BuildEventListener : BaseEventListener
    {
        /// <summary>
        /// Gets or sets a pointer to the IDE build events.
        /// </summary>
        private BuildEvents BuildEvents { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventListener"/> class.
        /// </summary>
        /// <param name="package">The package hosting the event listener.</param>
        internal BuildEventListener(BuildProgressPackage package)
            : base(package)
        {
            // Store access to the build events, otherwise events will not register properly via DTE.
            BuildEvents = Package.IDE.Events.BuildEvents;
            BuildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;
            BuildEvents.OnBuildProjConfigBegin += BuildEvents_OnBuildProjConfigBegin;
            BuildEvents.OnBuildProjConfigDone += BuildEvents_OnBuildProjConfigDone;
            BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
        }

        /// <summary>
        /// An event raised when a build has begun.
        /// </summary>
        internal event _dispBuildEvents_OnBuildBeginEventHandler BuildBegin;

        /// <summary>
        /// An event raised when an individual project build has begun.
        /// </summary>
        internal event _dispBuildEvents_OnBuildProjConfigBeginEventHandler BuildProjConfigBegin;

        /// <summary>
        /// An event raised when an individual project build is done.
        /// </summary>
        internal event _dispBuildEvents_OnBuildProjConfigDoneEventHandler BuildProjConfigDone;

        /// <summary>
        /// An event raised when a build is done.
        /// </summary>
        internal event _dispBuildEvents_OnBuildDoneEventHandler BuildDone;

        /// <summary>
        /// Event raised when a build begins.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="action">The action.</param>
        private void BuildEvents_OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            if (BuildBegin != null)
            {
                BuildBegin(scope, action);
            }
        }

        /// <summary>
        /// Event raised when the build of an individual project begins.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectConfig">The project config.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="solutionConfig">The solution config.</param>
        private void BuildEvents_OnBuildProjConfigBegin(string project, string projectConfig, string platform, string solutionConfig)
        {
            if (BuildProjConfigBegin != null)
            {
                BuildProjConfigBegin(project, projectConfig, platform, solutionConfig);
            }
        }

        /// <summary>
        /// Event raised when the build of an individual project is done.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectConfig">The project config.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="solutionConfig">The solution config.</param>
        /// <param name="success">True if project build was successful, otherwise false.</param>
        private void BuildEvents_OnBuildProjConfigDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (BuildProjConfigDone != null)
            {
                BuildProjConfigDone(project, projectConfig, platform, solutionConfig, success);
            }
        }

        /// <summary>
        /// Event raised when a build is done.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="action">The action.</param>
        private void BuildEvents_OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            if (BuildDone != null)
            {
                BuildDone(scope, action);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                if (disposing && BuildEvents != null)
                {
                    BuildEvents.OnBuildBegin -= BuildEvents_OnBuildBegin;
                    BuildEvents.OnBuildProjConfigBegin -= BuildEvents_OnBuildProjConfigBegin;
                    BuildEvents.OnBuildProjConfigDone -= BuildEvents_OnBuildProjConfigDone;
                    BuildEvents.OnBuildDone -= BuildEvents_OnBuildDone;
                }
            }
        }
    }
}
