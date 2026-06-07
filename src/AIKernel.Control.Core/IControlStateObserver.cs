namespace AIKernel.Control;

public interface IControlStateObserver
{
    ValueTask ObserveAsync(
        ControlStateSnapshot snapshot,
        CancellationToken cancellationToken = default);
}
