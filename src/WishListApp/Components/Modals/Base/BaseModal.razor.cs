using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WishListApp.Models.Modals;
using WishListApp.Models.Modals.Results;

namespace WishListApp.Components.Modals.Base;

public abstract class BaseModal<TContext, TResult>(IJSRuntime jsRuntime) : ComponentBase, IAsyncDisposable
    where TContext : ModalContext<TResult>
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private IJSObjectReference _jsModule = default!;
    private DotNetObjectReference<BaseModal<TContext, TResult>> _modalReference = default!;

    [Parameter, EditorRequired]
    public TContext Context { get; set; } = default!;

    [Parameter]
    public bool AutoShow { get; set; }

    [Parameter]
    public bool AllowFullscreen { get; set; }

    [Parameter]
    public EventCallback<TContext> OnShow { get; set; }

    [Parameter]
    public EventCallback<TContext> OnShown { get; set; }

    [Parameter]
    public EventCallback<TContext> OnHide { get; set; }

    [Parameter]
    public EventCallback<TContext> OnHidden { get; set; }

    [Parameter]
    public EventCallback<TContext> OnHidePrevented { get; set; }

    [JSInvokable]
    public async Task OnBootstrapModalShow()
    {
        await OnModalShow();
        await OnShow.InvokeAsync(Context);
    }

    [JSInvokable]
    public async Task OnBootstrapModalShown()
    {
        await OnModalShown();
        await OnShown.InvokeAsync(Context);
    }

    [JSInvokable]
    public async Task OnBootstrapModalHide()
    {
        await OnModalHide();
        await OnHide.InvokeAsync(Context);
    }

    [JSInvokable]
    public async Task OnBootstrapModalHidden()
    {
        await OnModalHidden();
        await OnHidden.InvokeAsync(Context);
    }

    [JSInvokable]
    public async Task OnBootstrapModalHidePrevented()
    {
        await OnModalHidePrevented();
        await OnHidePrevented.InvokeAsync(Context);
    }

    public async Task Show() => await _jsModule.InvokeVoidAsync("showModal", Context.Id);

    public async Task Hide() => await _jsModule.InvokeVoidAsync("hideModal", Context.Id);

    public virtual Task OnModalShow() => Task.CompletedTask;

    public virtual Task OnModalShown() => Task.CompletedTask;

    public virtual Task OnModalHide() => Task.CompletedTask;

    public virtual Task OnModalHidden()
    {
        Context.SetResult(new ModalCancelled());
        return Task.CompletedTask;
    }

    public virtual Task OnModalHidePrevented() => Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./Components/Modals/Base/BaseModal.razor.js");

            _modalReference = DotNetObjectReference.Create(this);

            await _jsModule.InvokeVoidAsync("initModal", Context.Id, _modalReference);

            if (AutoShow)
            {
                await Show();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _modalReference?.Dispose();

        if (_jsModule is not null)
        {
            try
            {
                await _jsModule.DisposeAsync();
            }
            catch { }
        }

        GC.SuppressFinalize(this);
    }
}
