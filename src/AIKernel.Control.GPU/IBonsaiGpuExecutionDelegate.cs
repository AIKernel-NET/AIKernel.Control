using AIKernel.Control.Core.Bonsai;

namespace AIKernel.Control.GPU;

/// <summary>EN: Documentation for public API. JA: IBonsaiGpuExecutionDelegate contract を定義します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.GPU.IBonsaiGpuExecutionDelegate']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.GPU.IBonsaiGpuExecutionDelegate']" />
public interface IBonsaiGpuExecutionDelegate : IBonsaiInferenceKernel
{
    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.GPU.IBonsaiGpuExecutionDelegate.DeviceId']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.GPU.IBonsaiGpuExecutionDelegate.DeviceId']" />
    string DeviceId { get; }

    /// <include file="docs.en.xml" path="doc/members/member[@name='P:AIKernel.Control.GPU.IBonsaiGpuExecutionDelegate.IsAvailable']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='P:AIKernel.Control.GPU.IBonsaiGpuExecutionDelegate.IsAvailable']" />
    bool IsAvailable { get; }
}
