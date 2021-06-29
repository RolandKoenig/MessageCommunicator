using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace MessageCommunicator.TestGui
{
    public class DefaultDataGridBehavior : AvaloniaObject
    {
        public static readonly AttachedProperty<bool> IsDefaultGridBehaviorActiveProperty =
            AvaloniaProperty.RegisterAttached<DefaultDataGridBehavior, DataGrid, bool>(
                "IsDefaultDataGridBehaviorActive",
                coerce: ValidateIsDefaultGridBehaviorActiveProperty);

        public static bool ValidateIsDefaultGridBehaviorActiveProperty(IAvaloniaObject targetObject, bool givenValue)
        {
            if ((givenValue) && 
                (targetObject is DataGrid targetControl))
            {
                var _lastPointerPosition = new Point(0, 0);
                targetControl.CellPointerPressed += (_, eArgs) =>
                {
                    targetControl.SelectedItem = eArgs.Row.DataContext;
                };
                targetControl.PointerMoved += (_, eArgs) =>
                {
                    _lastPointerPosition = eArgs.GetPosition(targetControl);
                };
                targetControl.DoubleTapped += (_, _) =>
                {
                    if ((targetControl.DataContext is IDoubleTabEnabledViewModel targetControlVM) &&
                        (targetControl.InputHitTest(_lastPointerPosition) is StyledElement {DataContext: { }} hitElement))
                    {
                        targetControlVM.NotifyDoubleTap(hitElement.DataContext);
                    }
                };
            }

            return true;
        }
    }
}
