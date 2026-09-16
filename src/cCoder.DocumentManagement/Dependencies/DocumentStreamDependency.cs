// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.DocumentManagement.Dependencies;

internal sealed class DocumentStreamDependency : MemoryStream
{
    internal DocumentStreamDependency()
    {
    }

    internal DocumentStreamDependency(byte[] buffer)
        : base(buffer: buffer)
    {
    }

    protected override void Dispose(bool disposing) =>
        base.Dispose(disposing: disposing);
}