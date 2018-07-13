using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScriptScripter.DesktopApp
{
    public class FileNameDropHandler : Contracts.IFileNameDropHandler
    {
        public Action<string> OnDropAction { get; set; }

        public void Init(UIElement element)
        {
            element.PreviewDragEnter += Element_PreviewDragEnter;
            element.PreviewDragOver += Element_PreviewDragOver;
            element.PreviewDrop += Element_PreviewDrop;
        }

        private void Element_PreviewDrop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, autoConvert: true);
            string name = filenames.FirstOrDefault();

            this.OnDropAction?.Invoke(name);

            e.Handled = true;
        }

        private void Element_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (this.IsDragDataASingleFile(e))
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void Element_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (this.IsDragDataASingleFile(e))
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }


        private bool IsDragDataASingleFile(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, autoConvert: true))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, autoConvert: true);
                return filenames != null && filenames.Count() == 1;
            }
            else
                return false;

        }
    }
}
