using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace hwatanab4546.Wpf
{
    public class WatermarkedTextBoxBehavior : Behavior<TextBox>
    {
        private const string DefaultWatermarkText = "入力してください";
        private const double DefaultWatermarkOpacity = 0.5;

        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register(
                nameof(WatermarkText), typeof(string), typeof(WatermarkedTextBoxBehavior),
                new PropertyMetadata(DefaultWatermarkText, (d, e) => (d as WatermarkedTextBoxBehavior)?.OnWatermarkTextPropertyChanged(e)));
        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        public static readonly DependencyProperty WatermarkOpacityProperty =
            DependencyProperty.Register(nameof(WatermarkOpacity), typeof(double), typeof(WatermarkedTextBoxBehavior),
                new PropertyMetadata(DefaultWatermarkOpacity, (d, e) => (d as WatermarkedTextBoxBehavior)?.OnWatermarkOpacityPropertyChanged(e)));
        public double WatermarkOpacity
        {
            get => (double)GetValue(WatermarkOpacityProperty);
            set => SetValue(WatermarkOpacityProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            MyAdorner = new(AssociatedObject, AssociatedObject);
            MyAdorner.SetWatermarkText(WatermarkText);
            MyAdorner.SetWatermarkOpacity(WatermarkOpacity);

            MyAdornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            MyAdornerLayer.Add(MyAdorner);

            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MyAdorner is not null)
            {
                if (string.IsNullOrEmpty(AssociatedObject.Text))
                {
                    MyAdorner.Visibility = Visibility.Visible;
                }
                else
                {
                    MyAdorner.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void OnWatermarkTextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            MyAdorner?.SetWatermarkText(e.NewValue.ToString());
        }

        private void OnWatermarkOpacityPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            MyAdorner?.SetWatermarkOpacity((double)e.NewValue);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            MyAdornerLayer?.Remove(MyAdorner);

            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }

        private WatermarkedAdorner? MyAdorner;
        private AdornerLayer? MyAdornerLayer;

        private class WatermarkedAdorner : Adorner
        {
            public WatermarkedAdorner(UIElement adornedElement, TextBox textBox) : base(adornedElement)
            {
                watermark = new()
                {
                    FontSize = textBox.FontSize,
                    FontFamily = textBox.FontFamily,
                    FontStretch = textBox.FontStretch,
                    FontStyle = textBox.FontStyle,
                    FontWeight = textBox.FontWeight,
                    Foreground = textBox.Foreground,
                    Margin = new Thickness(2, 0, 0, 0),
                    Opacity = DefaultWatermarkOpacity,
                    IsHitTestVisible = false,
                };
                visuals = new(this)
                {
                    watermark
                };
            }

            public void SetWatermarkText(string? text) => watermark.Text = text;

            public void SetWatermarkOpacity(double opacity) => watermark.Opacity = opacity;

            protected override Size ArrangeOverride(Size finalSize)
            {
                var element = AdornedElement as FrameworkElement;
                if (element is not null)
                {
                    watermark.Arrange(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
                }
                return finalSize;
            }

            protected override int VisualChildrenCount => visuals.Count;
            protected override Visual GetVisualChild(int index) => visuals[index];

            private readonly TextBlock watermark;
            private readonly VisualCollection visuals;
        }
    }
}
