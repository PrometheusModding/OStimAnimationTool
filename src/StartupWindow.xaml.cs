using System;
using System.Windows;
using System.Windows.Forms;

namespace OStimConversionTool
{
    public partial class StartupWindow : Window
    {
        public static string? rootDir;
        public static string? moduleName;

        public StartupWindow()
        {
            InitializeComponent();
            rootDir = Environment.SpecialFolder.Personal.ToString();
        }

        private void ChooseRootDir_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderBrowserDialog = new();
            {
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    RootDir.Content = folderBrowserDialog.SelectedPath;
                    rootDir = RootDir.Content?.ToString();
                }
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            moduleName = ModuleName.GetLineText(0);
            if (RootDir.Content.Equals("Choose Working Directory"))
            {
                string message = $"You haven't chosen a woring Directory. Do You want to use the defaul {rootDir} Path as Working Directory instead?";
                string caption = "No Valid Working Directory";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = System.Windows.Forms.MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }

            if (ModuleName.GetLineText(0) == "")
            {
                string message = $"Please choose a valid token for your Module";
                string caption = "No Valid Module Token";

                System.Windows.Forms.MessageBox.Show(message, caption);
            }
            else
                this.Close();
        }
    }
}
