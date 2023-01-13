using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace hwatanab4546.Wpf
{
    public class DragDropTextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            (bkup, AssociatedObject.AllowDrop) = (AssociatedObject.AllowDrop, true);
            AssociatedObject.PreviewDragOver += AssociatedObject_PreviewDragOver;
            AssociatedObject.PreviewDrop += AssociatedObject_PreviewDrop;
        }

        private void AssociatedObject_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = TryGetData(e.Data, out _) ? DragDropEffects.All : DragDropEffects.None;

            e.Handled = true;
        }

        private void AssociatedObject_PreviewDrop(object sender, DragEventArgs e)
        {
            if (TryGetData(e.Data, out string path))
            {
                AssociatedObject.Text = path;
            }

            e.Handled = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.AllowDrop = bkup;
            AssociatedObject.PreviewDragOver -= AssociatedObject_PreviewDragOver;
            AssociatedObject.PreviewDrop -= AssociatedObject_PreviewDrop;
        }

        private bool bkup;

        private static bool TryGetData(IDataObject data, out string path)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                if (data.GetData(DataFormats.FileDrop) is string[] paths && paths.Length == 1)
                {
                    path = paths[0];
                    return true;
                }
            }

            path = string.Empty;
            return false;
        }
    }
}
