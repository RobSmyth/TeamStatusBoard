using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NoeticTools.SystemsDashboard.Framework.Config;
using NoeticTools.SystemsDashboard.Framework.Registries;


namespace NoeticTools.SystemsDashboard.Framework.Plugins.PropertyEditControls
{
    public class CheckboxPropertyViewProvider : IPropertyViewProvider
    {
        public bool CanHandle(string elementType)
        {
            return elementType == "Checkbox";
        }

        public FrameworkElement Create(IPropertyViewModel propertyViewModel, int rowIndex, string elementName)
        {
            var value = (string) propertyViewModel.Value;
            var isChecked = !string.IsNullOrWhiteSpace(value) && bool.Parse(value);
            var checkbox = new CheckBox
            {
                IsChecked = isChecked,
                Name = elementName,
                DataContext = propertyViewModel
            };

            var binding = new Binding("Value");
            BindingOperations.SetBinding(checkbox, CheckBox.IsCheckedProperty, binding);

            return checkbox;
        }
    }
}