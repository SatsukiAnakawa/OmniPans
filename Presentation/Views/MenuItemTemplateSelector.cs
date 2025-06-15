// Presentation/Views/MenuItemTemplateSelector.cs
// コンテキストメニューのアイテムに応じて、表示するデータテンプレートを動的に選択します。
namespace OmniPans.Presentation.Views;

using System.Windows;
using System.Windows.Controls;

public class MenuItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? HeaderTemplate { get; set; }
    public DataTemplate? SeparatorTemplate { get; set; }
    public DataTemplate? DeviceTemplate { get; set; }

    // アイテムの種類に応じて適切なDataTemplateを返します。
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            HeaderMenuItemViewModel => HeaderTemplate,
            SeparatorMenuItemViewModel => SeparatorTemplate,
            HiddenDeviceMenuItemViewModel => DeviceTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}