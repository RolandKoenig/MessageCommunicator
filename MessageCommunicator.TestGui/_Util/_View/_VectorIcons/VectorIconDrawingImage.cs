using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class VectorIconDrawingImage : DrawingImage, IWeakMessageTarget<MessageThemeChanged>
    {
        public VectorIconDrawingImage()
        {
            MessageBus.Current.ListenWeak(this);
        }

        public void UpdateBrushes()
        {
            if (this.Drawing is VectorIconGeometryDrawing vectorIconDrawing)
            {
                vectorIconDrawing.UpdateBrushes();
            }
        }

        /// <inheritdoc />
        public void OnMessage(MessageThemeChanged message)
        {
            this.UpdateBrushes();
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DrawingProperty)
            {
                this.UpdateBrushes();
            }
        }
    }
}
