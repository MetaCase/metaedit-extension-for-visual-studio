// Guids.cs
// MUST match guids.h
using System;

namespace MetaCase.GraphBrowser
{
    static class GuidList
    {
        public const string guidGraphBrowserPkgString = "ef5741cb-f689-4ec5-9277-1178d6522749";
        public const string guidGraphBrowserCmdSetString = "41899404-d6dc-4aef-b017-3c32f8428610";
        public const string guidToolWindowPersistanceString = "21551479-d8da-4cee-a2cc-3779967c5a58";

        public static readonly Guid guidGraphBrowserCmdSet = new Guid(guidGraphBrowserCmdSetString);
    };
}