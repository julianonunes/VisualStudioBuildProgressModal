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
    internal class BuildProgress
    {
        private bool _hasBuildFailed;
        private bool _isBuildActive;
        private bool _isProgressIndeterminate;
        private double _progressPercentage;
        private BuildStatusWindow _buildStatusWindow;
        

        /// <summary>
        /// Gets or sets the last known build action.
        /// </summary>
        private vsBuildAction BuildAction { get; set; }

        /// <summary>
        /// Gets or sets the last known build scope.
        /// </summary>
        private vsBuildScope BuildScope { get; set; }

        /// <summary>
        /// Gets or sets the number of projects built.
        /// </summary>
        private int NumberOfProjectsBuilt { get; set; }

        /// <summary>
        /// Gets or sets the number of projects to be built.
        /// </summary>
        private int NumberOfProjectsToBeBuilt { get; set; }

        private BuildProgressPackage Package { get; set; }

        /// <summary>
        /// Gets the progress percentage, otherwise zero if cannot be determined.
        /// </summary>
        private double ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                if (_progressPercentage != value)
                {
                    _progressPercentage = value;

                    updateTaskbarStatus();
                }
            }
        }

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Gets of sets a flag indicating if a build has failed.
        /// </summary>
        public bool HasBuildFailed
        {
            get { return _hasBuildFailed; }
            set
            {
                if (!_hasBuildFailed)
                {
                    if (_hasBuildFailed != value)
                    {
                        _hasBuildFailed = value;

                        //updateTaskbarStatus();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets a flag indicating if a build is active.
        /// </summary>
        public bool IsBuildActive
        {
            get { return _isBuildActive; }
            set
            {
                if (_isBuildActive != value)
                {
                    _isBuildActive = value;

                    updateTaskbarStatus();
                }
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating if the progress is indeterminate.
        /// </summary>
        public bool IsProgressIndeterminate
        {
            get { return _isProgressIndeterminate; }
            set
            {
                if (_isProgressIndeterminate != value)
                {
                    _isProgressIndeterminate = value;

                    updateTaskbarStatus();
                }
            }
        }

        #endregion

        public BuildProgress(BuildProgressPackage package)
        {
            Package = package;
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
        /// A build has begun.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="action">The action.</param>
        private void buildBegin(vsBuildScope scope, vsBuildAction action)
        {
            BuildAction = action;
            BuildScope = scope;
            NumberOfProjectsBuilt = 0;

            if (scope == vsBuildScope.vsBuildScopeSolution)
            {
                NumberOfProjectsToBeBuilt = getNumberOfProjectsToBeBuilt();
                IsProgressIndeterminate = false;
            }
            else
            {
                NumberOfProjectsToBeBuilt = 0;
                IsProgressIndeterminate = true;
            }

            // reset the field directly. the property won't allow a "true" to be changed to a "true".
            _hasBuildFailed = false;
            IsBuildActive = true;
            ProgressPercentage = 0;
            _buildStatusWindow = new BuildStatusWindow();
            _buildStatusWindow.Show();
        }

        /// <summary>
        /// A individual project build has begun.
        /// </summary>
        private void buildProjConfigBegin(string project)
        {
            if (_buildStatusWindow != null)
                _buildStatusWindow.SetProject(project);

            NumberOfProjectsBuilt++;
            ProgressPercentage = calculateBuildPercentage();
        }

        /// <summary>
        /// Individual project build is done.
        /// </summary>
        private void buildProjConfigDone(bool success)
        {
            if (!success)
            {
                HasBuildFailed = true;
            }
        }

        /// <summary>
        /// Build is done.
        /// </summary>
        private void buildDone()
        {
            if (_buildStatusWindow == null)
                _buildStatusWindow = new BuildStatusWindow();

            _buildStatusWindow.SetProgress(100);

            IsProgressIndeterminate = false;
            // a failed build needs to displayed in the taskbar.
            ProgressPercentage = HasBuildFailed ? 100 : 0;
            IsBuildActive = false;

            System.Threading.Thread.Sleep(1000);
            _buildStatusWindow.Close();
        }

        /// <summary>
        /// Gets the number of projects to be built based on the active solution configuration.
        /// </summary>
        private int getNumberOfProjectsToBeBuilt()
        {
            SolutionContexts solutionContexts = Package.IDE.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts;
            int count = 0;

            for (int i = 0; i < solutionContexts.Count; i++)
            {
                try
                {
                    if (solutionContexts.Item(i + 1).ShouldBuild)
                    {
                        count++;
                    }
                }
                catch (ArgumentException)
                {
                    // This is a work-around for a known issue with the SolutionContexts.GetEnumerator with unloaded projects in VS2010.
                }
            }

            return count;
        }

        /// <summary>
        /// Called when a build has begun.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="action">The action.</param>
        internal void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            buildBegin(scope, action);
        }

        /// <summary>
        /// Called when an individual project build has begun.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectConfig">The project config.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="solutionConfig">The solution config.</param>
        internal void OnBuildProjConfigBegin(string project, string projectConfig, string platform, string solutionConfig)
        {
            buildProjConfigBegin(project);
        }

        /// <summary>
        /// Called when an individual project build is done.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="projectConfig">The project config.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="solutionConfig">The solution config.</param>
        /// <param name="success">True if project build was successful, otherwise false.</param>
        internal void OnBuildProjConfigDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            buildProjConfigDone(success);
        }

        /// <summary>
        /// Called when a build is done.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="action">The action.</param>
        internal void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            buildDone();
        }

        private double calculateBuildPercentage()
        {
            double toBeBuilt = NumberOfProjectsToBeBuilt;

            return toBeBuilt > 0 ? (NumberOfProjectsBuilt * 100) / toBeBuilt : 0;
        }

        /// <summary>
        /// Updates the taskbar status based on the current build conditions.
        /// </summary>
        private void updateTaskbarStatus()
        {
            //TaskbarItemProgressState progressState = TaskbarItemProgressState.None;

            //a failed build should always be indicated in the taskbar.
            if (HasBuildFailed)
            {
                //progressState = TaskbarItemProgressState.Error;
            }
            else
            {
                if (IsBuildActive)
                {
                    if (IsProgressIndeterminate)
                    {
                        //progressState = TaskbarItemProgressState.Indeterminate;
                    }
                    else
                    {
                        //progressState = TaskbarItemProgressState.Normal;
                    }
                }
            }

            //TaskbarItemInfo.ProgressState = progressState;
            //TaskbarItemInfo.ProgressValue = ProgressPercentage;
            if (_buildStatusWindow == null)
                _buildStatusWindow = new BuildStatusWindow();
            
            _buildStatusWindow.SetProgress((int)ProgressPercentage);
        }
    }
}
