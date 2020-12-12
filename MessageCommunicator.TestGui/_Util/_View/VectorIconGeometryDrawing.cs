using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace MessageCommunicator.TestGui
{
    public class VectorIconGeometryDrawing : GeometryDrawing
    {
        private static IBrush s_brushPositive = Brushes.Green;
        private static IPen s_penPositive = new Pen(s_brushPositive);
        private static IBrush s_brushNegative = Brushes.Red;
        private static IPen s_penNegative = new Pen(s_brushNegative);
        private static IBrush s_brushNeutral = new SolidColorBrush(new Color(255, 70, 70, 70));
        private static IPen s_penNeutral = new Pen(s_brushNeutral);

        private IconBrushStyle _brushStyle;

        public IconBrushStyle BrushStyle
        {
            get => _brushStyle;
            set
            {
                _brushStyle = value;
                switch (value)
                {
                    case IconBrushStyle.Positive:
                        this.Brush = s_brushPositive;
                        this.Pen = s_penPositive;
                        break;

                    case IconBrushStyle.Negative:
                        this.Brush = s_brushNegative;
                        this.Pen = s_penNegative;
                        break;

                    case IconBrushStyle.Neutral:
                        this.Brush = s_brushNeutral;
                        this.Pen = s_penNeutral;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
    }
}
