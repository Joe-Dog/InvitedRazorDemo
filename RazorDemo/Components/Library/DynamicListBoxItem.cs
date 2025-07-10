using Microsoft.AspNetCore.Components;

namespace RazorDemo.Components.Library;

public class DynamicListBoxItem<TValue> : ComponentBase
{
    [CascadingParameter] private DynamicListBox<TValue>? Parent { get; set; }
    [Parameter] public required string Text { get; set; }
    [Parameter] public required TValue Value { get; set; }
    [Parameter] public required RenderFragment ChildContent { get; set; }

    protected override void OnInitialized()
    {
        Parent?.RegisterChild(Text, Value, ChildContent);
    }
}
