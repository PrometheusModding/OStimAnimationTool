using System;
using System.ComponentModel;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Prism.Services.Dialogs;

namespace OStimAnimationTool.Core.Controls
{
    public class MaterialDesignDialogWindow : DialogHost, IDialogWindow
    {
        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public bool? ShowDialog()
        {
            Show(nameof(this));
        }

        public Window? Owner { get; set; }
        public IDialogResult? Result { get; set; }
        public event EventHandler? Closed;
        public event CancelEventHandler? Closing;
    }
}
