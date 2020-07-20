using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace TcpCommunicator.TestGui
{
    public class XamlExtensions : AvaloniaObject
    {
        public static readonly AttachedProperty<bool> IsDefaultGridBehaviorActiveProperty =
            AvaloniaProperty.RegisterAttached<XamlExtensions, DataGrid, bool>(
                "IsDefaultGridBehaviorActive",
                validate: ValidateIsDefaultGridBehaviorActiveProperty);

        public static bool ValidateIsDefaultGridBehaviorActiveProperty(DataGrid targetControl, bool givenValue)
        {
            if (givenValue)
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
