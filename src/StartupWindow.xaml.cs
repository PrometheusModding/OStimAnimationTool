using System;
using System.Windows;
using System.Windows.Forms;

namespace OStimConversionTool
{
    public partial class StartupWindow
    {
        //TODO: make this non-static
        public static string? rootDir;
        public static string? moduleName;
        public static string animator = string.Empty;

        public StartupWindow()
        {
            InitializeComponent();
            rootDir = Environment.SpecialFolder.Personal.ToString();
        }

        private void ChooseRootDir_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderBrowserDialog = new();
            {
                if (folderBrowserDialog.ShowDialog() != true) return;
                RootDir.Content = folderBrowserDialog.SelectedPath;
                rootDir = RootDir.Content?.ToString();
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            moduleName = ModuleName.GetLineText(0);
            animator = Animator.GetLineText(0);

            if (moduleName.Equals(string.Empty))
            {
                const string message = "Please choose a valid token for your Module";
                const string caption = "No Valid Module Token";

                System.Windows.Forms.MessageBox.Show(message, caption);
            }

            if (RootDir.Content.Equals("Choose Working Directory"))
            {
                string message = $"You haven't chosen a working Directory. Do You want to use the default {rootDir} Path as Working Directory instead?";
                const string caption = "No Valid Working Directory";
                const MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                var result = System.Windows.Forms.MessageBox.Show(message, caption, buttons);
                if (result != System.Windows.Forms.DialogResult.Yes) return;
                RootDir.Content = rootDir;
                if (!moduleName.Equals(string.Empty))
                    Close();
            }
            else if (!moduleName.Equals(string.Empty) && !RootDir.Content.Equals("Choose Working Directory"))
            {
                Close();
            }
        }
    }
}
