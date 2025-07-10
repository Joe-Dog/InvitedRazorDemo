using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RazorDemo.Components.Library
{
    public class DynamicListBox<TValue> : ComponentBase
    {
        [Parameter] public required RenderFragment ChildContent { get; set; }
        [Parameter] public string Width { get; set; } = "200px";
        [Parameter] public string Height { get; set; } = "200px";
        [Parameter] public required TValue SelectedValue { get; set; }
        [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

        private class ItemModel
        {
            public required string Text { get; set; }
            public required TValue Value { get; set; }
            public bool Removing { get; set; }
            public RenderFragment? Template { get; set; }
        }

        private readonly List<ItemModel> _items = [];
        private bool _staticChildrenStamped;

        internal void RegisterChild(string text, TValue value, RenderFragment template)
        {
            _items.Add(new ItemModel { Text = text, Value = value, Template = template });
        }

        public void AddItem(string text, TValue value)
        {
            _items.Add(new ItemModel { Text = text, Value = value });
            StateHasChanged();
        }

        public async Task RemoveSelectedAsync()
        {
            var item = _items.FirstOrDefault(x => EqualityComparer<TValue>.Default.Equals(x.Value, SelectedValue));
            if (item == null) return;

            item.Removing = true;
            StateHasChanged();

            await Task.Delay(1000);

            _items.Remove(item);
            await SelectedValueChanged.InvokeAsync(default);
            StateHasChanged();
        }

        private async Task OnItemClicked(ItemModel itemModel)
        {
            SelectedValue = itemModel.Value;
            await SelectedValueChanged.InvokeAsync(itemModel.Value);
            StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!_staticChildrenStamped && ChildContent != null)
            {
                builder.OpenComponent<CascadingValue<DynamicListBox<TValue>>>(0);
                builder.AddAttribute(1, "Value", this);
                builder.AddAttribute(2, "ChildContent", ChildContent);
                builder.CloseComponent();
                _staticChildrenStamped = true;
            }

            var seq = 10;
            var style = $"width:{Width};height:{Height};border:1px solid #ccc;overflow-y:auto;";

            builder.OpenElement(seq++, "div");
            builder.AddAttribute(seq++, "style", style);

            foreach (var item in _items)
            {
                var itemStyle = "padding:6px;cursor:pointer;";
                if (EqualityComparer<TValue>.Default.Equals(item.Value, SelectedValue))
                    itemStyle += "background:lightgray;";
                if (item.Removing)
                    itemStyle += "background:blue;color:white;";

                builder.OpenElement(seq++, "div");
                builder.AddAttribute(seq++, "style", itemStyle);
                builder.AddAttribute(seq++, "onclick",
                  EventCallback.Factory.Create(this, () => OnItemClicked(item)));

                if (item.Template != null)
                    builder.AddContent(seq++, item.Template);
                else
                    builder.AddContent(seq++, item.Text);

                builder.CloseElement();
            }

            builder.CloseElement();
        }
    }
}
