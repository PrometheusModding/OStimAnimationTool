using System;
using System.Windows;

namespace OStimConversionTool
{
    public partial class Startup : Window
    {
        public static string? rootDir;
        public static string? moduleName;

        public Startup()
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
            if (RootDir.Content.Equals("Choose Working Directory") || ModuleName.GetLineText(0) == "")
                MessageBox.Show("no");
            this.Close();
        }
    }
}
