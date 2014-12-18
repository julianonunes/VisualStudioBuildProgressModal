// Guids.cs
// MUST match guids.h
using System;

namespace Vodia.Vodia_BuildProgress
{
    static class GuidList
    {
        public const string guidVodia_BuildProgressPkgString = "2563131b-ba85-4500-a71c-eef7e39a5e71";
        public const string guidVodia_BuildProgressCmdSetString = "5e00225c-8045-4061-af76-19e553e4b1a5";

        public static readonly Guid guidVodia_BuildProgressCmdSet = new Guid(guidVodia_BuildProgressCmdSetString);
    };
}