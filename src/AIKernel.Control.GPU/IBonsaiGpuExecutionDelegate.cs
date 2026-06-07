using AIKernel.Control.Core.Bonsai;

namespace AIKernel.Control.GPU;

public interface IBonsaiGpuExecutionDelegate : IBonsaiInferenceKernel
{
    string DeviceId { get; }

    bool IsAvailable { get; }
}
