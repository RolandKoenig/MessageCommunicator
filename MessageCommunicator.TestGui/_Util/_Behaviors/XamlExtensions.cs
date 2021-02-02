using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace MessageCommunicator.TestGui
{
    public class XamlExtensions : AvaloniaObject
    {
        public static readonly AttachedProperty<bool> IsDefaultGridBehaviorActiveProperty =
            AvaloniaProperty.RegisterAttached<XamlExtensions, DataGrid, bool>(
                "IsDefaultGridBehaviorActive",
                coerce: ValidateIsDefaultGridBehaviorActiveProperty);

        public static bool ValidateIsDefaultGridBehaviorActiveProperty(IAvaloniaObject targetObject, bool givenValue)
        {
            if ((givenValue) && 
                (targetObject is DataGrid targetControl))
            {
                targetControl.CellPointerPressed += (sender, eArgs) =>
                {
                    targetControl.SelectedItem = eArgs.Row.DataContext;
                };
            }

            return true;
        }
    }
}
