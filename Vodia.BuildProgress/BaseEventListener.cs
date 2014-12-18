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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodia.Vodia_BuildProgress
{
    /// <summary>
    /// The base implementation of an event listener.
    /// </summary>
    internal abstract class BaseEventListener : IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        protected bool IsDisposed { get; set; }
        /// <summary>
        /// Gets the hosting package.
        /// </summary>
        protected BuildProgressPackage Package { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEventListener"/> class.
        /// </summary>
        /// <param name="package">The package hosting the event listener.</param>
        protected BaseEventListener(BuildProgressPackage package)
        {
            Package = package;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
            }
        }
    }
}
