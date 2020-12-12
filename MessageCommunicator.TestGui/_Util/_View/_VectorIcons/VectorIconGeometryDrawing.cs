using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class VectorIconGeometryDrawing : GeometryDrawing, IWeakMessageTarget<MessageThemeChanged>
    {
        private static readonly Dictionary<MessageCommunicatorTheme, IBrush> s_brushesPositive;
        private static readonly Dictionary<MessageCommunicatorTheme, IBrush> s_brushesNegative;
        private static readonly Dictionary<MessageCommunicatorTheme, IBrush> s_brushesNeutral;

        private IconBrushStyle _brushStyle;

        static VectorIconGeometryDrawing()
        {
            s_brushesPositive = new Dictionary<MessageCommunicatorTheme, IBrush>(2);
            s_brushesPositive[MessageCommunicatorTheme.Light] = Brushes.Green;
            s_brushesPositive[MessageCommunicatorTheme.Dark] = Brushes.Green;

            s_brushesNegative = new Dictionary<MessageCommunicatorTheme, IBrush>(2);
            s_brushesNegative[MessageCommunicatorTheme.Light] = Brushes.Red;
            s_brushesNegative[MessageCommunicatorTheme.Dark] = Brushes.Red;

            s_brushesNeutral = new Dictionary<MessageCommunicatorTheme, IBrush>(2);
            s_brushesNeutral[MessageCommunicatorTheme.Light] = new SolidColorBrush(new Color(255, 70, 70, 70));
            s_brushesNeutral[MessageCommunicatorTheme.Dark] = new SolidColorBrush(new Color(255, 170, 170, 170));
        }

        public VectorIconGeometryDrawing()
        {
            MessageBus.Current.ListenWeak<MessageThemeChanged>(this);
        }

        public IconBrushStyle BrushStyle
        {
            get => _brushStyle;
            set
            {
                _brushStyle = value;
                this.UpdateBrushes();
            }
        }

        private void UpdateBrushes()
        {
            var currentTheme = MessageCommunicatorGlobalProperties.Current.CurrentTheme;
            switch (_brushStyle)
            {
                case IconBrushStyle.Positive:
                    this.Brush = s_brushesPositive[currentTheme];
                    this.Pen = null;
                    break;

                case IconBrushStyle.Negative:
                    this.Brush = s_brushesNegative[currentTheme];
                    this.Pen = null;
                    break;

                case IconBrushStyle.Neutral:
                    this.Brush = s_brushesNeutral[currentTheme];
                    this.Pen = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(this.BrushStyle), _brushStyle, null);
            }
        }

        /// <inheritdoc />
        public void OnMessage(MessageThemeChanged message)
        {
            this.UpdateBrushes();
        }
    }
}
