// Guids.cs
// MUST match guids.h
using System;

namespace cyberoot.SimpleActions
{
    static class GuidList
    {
        public const string guidSimpleActionsPkgString = "f3c21ea9-d0ee-4597-aa0a-518f30a729ca";
        public const string guidSimpleActionsCmdSetString = "080085a3-3774-4867-b602-4bbf595fc25a";

        public static readonly Guid guidSimpleActionsCmdSet = new Guid(guidSimpleActionsCmdSetString);
    };
}