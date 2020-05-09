﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using HunterPie.Logger;

namespace HunterPie.GUI.Helpers
{
    public class Diamond : Shape
    {

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(Diamond), new PropertyMetadata(0.0, PercentageChanged));

        protected override Geometry DefiningGeometry => GetGeometry();

        private static void PercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var diamond = d as Diamond;
            if (diamond != null)
                diamond.InvalidateVisual();
        }

        private Geometry GetGeometry()
        {
            // Always start from the top middle
            Point start = new Point(RenderSize.Width , 0);
            StreamGeometry geom = new StreamGeometry();
            using (StreamGeometryContext context = geom.Open())
            {
                context.BeginFigure(start, false, false);
                for (int lIndex = 0; lIndex < 4; lIndex++)
                {
                    Point? nextPoint = CalculatePoint(lIndex);
                    if (nextPoint == null) break;
                    context.LineTo((Point)nextPoint, true, false);
                }
            }
            return geom;
        }

        private Point? CalculatePoint(int line)
        {
            double p = (100 / 4 * ((double)line)) / 100;
            if (Percentage < p) return null;
            else
            {
                double percentageOfLine = Math.Min(1, (Percentage * 100 - (25 * line)) / 25);
                switch (line)
                {
                    case 0:
                        return new Point(RenderSize.Width, RenderSize.Height * percentageOfLine);
                    case 1:
                        return new Point(RenderSize.Width - (RenderSize.Width * percentageOfLine), RenderSize.Height);
                    case 2:
                        return new Point(0, RenderSize.Height - (RenderSize.Height * percentageOfLine));
                    case 3:
                        return new Point(RenderSize.Width * percentageOfLine, 0);
                    default:
                        return null;
                }
            }
        }
    }
}
