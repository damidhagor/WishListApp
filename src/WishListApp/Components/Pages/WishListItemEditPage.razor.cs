using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using WishListApp.Services;

namespace WishListApp.Components.Pages;

public partial class WishListItemEditPage(
    NavigationManager navigationManager,
    IStringLocalizer<Localization> localizer,
    IUserService userService,
    IWishListRepository listRepository,
    IWishListItemRepository itemRepository)
{
    private readonly NavigationManager _navigationManager = navigationManager;
    private readonly IStringLocalizer<Localization> _localizer = localizer;
    private readonly IUserService _userService = userService;
    private readonly IWishListRepository _listRepository = listRepository;
    private readonly IWishListItemRepository _itemRepository = itemRepository;

    private WishListItem? _item;
    private ItemEditComponent _itemEditComponent = default!;

    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public string? ItemId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await LoadWishListAndValidateAccess(default);
    }

    private async Task LoadWishListAndValidateAccess(CancellationToken cancellationToken)
    {
        var user = await _userService.GetLoggedInWishListUser();

        var itemId = ObjectId.TryParse(ItemId, out var parsedItemId)
                ? parsedItemId
                : (ObjectId?)null;

        var list = itemId is not null
            ? await _listRepository.GetByItemId(itemId.Value, cancellationToken)
            : null;

        var item = list?.Items.FirstOrDefault(i => i.Id == itemId);

        if (list is null || item is null)
        {
            _navigationManager.NavigateTo("/not-found");
            return;
        }

        if (user is null || list.OwnerId != user.Identifier)
        {
            _navigationManager.NavigateTo("/");
            return;
        }

        _item = item.ToModel(list.Id);
    }

    private async Task LoadProductInformation() => await _itemEditComponent.LoadProductInformation();

    private async Task SaveWishListItem()
    {
        await _itemEditComponent.SaveWishListItem();
        _navigationManager.NavigateTo($"/wishlist?id={_item?.WishListId}");
    }
}
