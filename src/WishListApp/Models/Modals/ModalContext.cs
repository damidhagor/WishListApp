using WishListApp.Models.Modals.Results;

namespace WishListApp.Models.Modals;

public abstract record ModalContext<T> : IModalContext
{
    protected readonly TaskCompletionSource<ModalResult<T>> _tcs = new();

    public string Id { get; } = Guid.NewGuid().ToString();

    public void SetResult(ModalCancelled result) => _tcs.TrySetResult(result);

    public void SetResult(T result) => _tcs.TrySetResult(result);

    public async Task<ModalResult<T>> WaitForResult() => await _tcs.Task;
}
