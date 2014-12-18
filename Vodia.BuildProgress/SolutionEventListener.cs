using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodia.Vodia_BuildProgress
{
    internal class SolutionEventListener : BaseEventListener
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        ///  Reference to the Projects SolutionEvents.
        /// </summary>
        /// <remarks>
        /// The SolutionEvents object can go out of scope and be garbage collected before the solution is closed. 
        /// To retain a reference to this object, declare a private variable in the class in which you implement the solution event handlers.
        /// </remarks>
        ///-------------------------------------------------------------------------------------------------
        private SolutionEvents SolutionEvents { get; set; }

        public SolutionEventListener(BuildProgressPackage package)
            : base(package)
        {
            SolutionEvents = package.IDE.Events.SolutionEvents;
            SolutionEvents.AfterClosing += SolutionEvents_AfterClosing;
        }

        internal event _dispSolutionEvents_OpenedEventHandler Opened;

        internal event _dispSolutionEvents_BeforeClosingEventHandler BeforeClosing;

        internal event _dispSolutionEvents_AfterClosingEventHandler AfterClosing;

        internal event _dispSolutionEvents_QueryCloseSolutionEventHandler QueryCloseSolution;

        internal event _dispSolutionEvents_RenamedEventHandler Renamed;

        internal event _dispSolutionEvents_ProjectAddedEventHandler ProjectAdded;

        internal event _dispSolutionEvents_ProjectRemovedEventHandler ProjectRemoved;

        internal event _dispSolutionEvents_ProjectRenamedEventHandler ProjectRenamed;

        private void SolutionEvents_AfterClosing()
        {
            if (AfterClosing != null)
            {
                AfterClosing();
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

                if (disposing && SolutionEvents != null)
                {
                    SolutionEvents.AfterClosing -= SolutionEvents_AfterClosing;
                }
            }
        }
    }
}
