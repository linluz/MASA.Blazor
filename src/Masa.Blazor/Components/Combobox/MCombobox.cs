using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;

namespace Masa.Blazor;

public class MCombobox<TItem, TItemValue, TValue> : MAutocomplete<TItem, TItemValue, TValue>
{
    /// <summary>
    /// Format the input string to the item of type <typeparamref name="TItem"/>.
    /// </summary>
    /// <returns></returns>
    [Parameter] [EditorRequired] public Func<string, TItem> InputToItem { get; set; } = default!;

    private readonly Dictionary<string, int> _customInputs = new();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        InputToItem.ThrowIfNull(ComponentName);
    }

    protected override async Task HandleOnKeyDownNextAsync(KeyboardEventArgs args)
    {
        var keyCode = args.Code;

        if (Multiple && keyCode == KeyCodes.ArrowLeft)
        {
            var inputSelectionStart = await Js.InvokeAsync<int>(JsInteropConstants.GetProp, InputElement, "selectionStart");
            if (inputSelectionStart == 0)
            {
                UpdateSelf();
            }
        }
        else if (keyCode == KeyCodes.Enter)
        {
            await OnEnterDownAsync(args);
        }

        await base.HandleOnKeyDownNextAsync(args);
    }

    protected override Task OnTabDown(KeyboardEventArgs args)
    {
        if (Multiple
            && !string.IsNullOrEmpty(InternalSearch)
            && GetMenuIndex() == -1)
        {
            // TODO: e.preventDefault()
            // TODO: e.stopPropagation()

            return UpdateTags();
        }

        return base.OnTabDown(args);
    }

    private async Task OnEnterDownAsync(KeyboardEventArgs args)
    {
        // TODO: e.preventDefault();

        if (this.GetMenuIndex() > -1)
        {
            return;
        }

        NextTick(UpdateSelf);
    }

    private void UpdateSelf()
    {
        if (Multiple)
        {
            UpdateTags();
        }
        else
        {
            UpdateCombobox();
        }
    }

    private async Task UpdateTags()
    {
        var menuIndex = GetMenuIndex();

        if ((menuIndex < 0 && !SearchIsDirty) || string.IsNullOrEmpty(InternalSearch))
        {
            return;
        }

        var index = SelectedItems.ToList().FindIndex(item => InternalSearch == GetText(item));
        if (index == -1)
        {
            _customInputs.Add(InternalSearch, SelectedItems.Count);
        }

        var itemToSelect = index > -1 ? SelectedItems[index] : InputToItem(InternalSearch);

        // If it already exists, do nothing
        // but move to the end
        if (index > -1)
        {
            var internalValues = InternalValues.ToList();
            var value = ItemValue(itemToSelect);
            internalValues.Remove(value);
            internalValues.Add(value);
            SetValue(internalValues);
        }

        if (menuIndex > -1)
        {
            InternalSearch = null;
            return;
        }

        await SelectItem(itemToSelect);

        InternalSearch = null;
    }
    
    protected override async Task SelectItem(TItem item, bool closeOnSelect = true)
    {
        await base.SelectItem(item, closeOnSelect);

        if (!string.IsNullOrEmpty(InternalSearch)
            && Multiple
            && GetText(item).Contains(InternalSearch, StringComparison.CurrentCultureIgnoreCase))
        {
            InternalSearch = null;
        }
    }

    protected override void SetSelectedItems()
    {
        if (InternalValues.Count == 0)
        {
            SelectedItems = new List<TItem>();
        }
        else
        {
            var selectedItems = new List<TItem>();

            var values = InternalValues;

            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];
                var index = AllItems.FindIndex(v => EqualityComparer<TItemValue>.Default.Equals(value, GetValue(v)));

                if (index > -1)
                {
                    selectedItems.Add(AllItems[index]);
                }
                else
                {
                }
            }

            SelectedItems = selectedItems;
        }
    }

    private void UpdateCombobox()
    {
        // if search is not dirty, do nothing
        if (!SearchIsDirty)
        {
            return;
        }

        // The internal search is not matching
        // The internal value, update the input
        if (InternalSearch != GetText(SelectedItem))
        {
            SetValue(default);
        }

        // Reset search if using slot to avoid double input
        var isUsingSlot = SelectionContent is not null || HasChips;
        if (isUsingSlot)
        {
            InternalSearch = null;
        }
    }

    // protected override void SetSelectedItems()
    // {
    //     if (InternalValue == null)
    //     {
    //         SelectedItems = new List<TItem>();
    //     }
    //     else
    //     {
    //         SelectedItems = Multiple ? InternalValue : new List<TItem> { InternalValue };
    //     }
    // }

    protected override Task SetValue(TValue value)
    {
        if (value is null)
        {
            if (GetValue(InputToItem(InternalSearch)) is TValue val)
            {
                return base.SetValue(val);
            }
        }

        return base.SetValue(value);
    }
}
