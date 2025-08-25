using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CvnetClient.Utils
{
    internal class StripButtonHelper
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached(
                "Icon",
                typeof(string),
                typeof(StripButtonHelper),
                new PropertyMetadata(null));

        public static readonly DependencyProperty StripColorProperty =
            DependencyProperty.RegisterAttached(
                "StripColor",
                typeof(Brush),
                typeof(StripButtonHelper),
                new PropertyMetadata(Brushes.Transparent));
        public static void SetIcon(UIElement element, string value)
        {
            element.SetValue(IconProperty, value);
        }

        public static string GetIcon(UIElement element)
        {
            return (string)element.GetValue(IconProperty);
        }

        public static void SetStripColor(UIElement element, Brush value)
        {
            element.SetValue(StripColorProperty, value);
        }

        public static Brush GetStripColor(UIElement element)
        {
            return (Brush)element.GetValue(StripColorProperty);
        }
    }
}