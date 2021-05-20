using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace OStimConversionTool
{
    public partial class Startup : Window
    {
        public string sourceDir = string.Empty;

        public Startup()
        {
            InitializeComponent();
        }

        private void ChooseFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "Text files (*.hkx)|*hkx|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                sourceDir = openFileDialog.FileName;
                sourceDir = sourceDir.Remove(sourceDir.Length - openFileDialog.SafeFileName.Length);
                foreach (string filename in openFileDialog.FileNames)
                    lbFiles.Items.Add(Path.GetFileName(filename));
                MessageBox.Show(sourceDir);
            }
        }

        private void ChooseRootDir_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new() { };
            {
                if (openFileDialog.ShowDialog() == true) { rootDir.Content = openFileDialog.FileName; }
            }
        }

        private void ConversionProcess_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < lbFiles.Items.Count; i++)
            {
                var ls = (mN: moduleName.GetLineText(0), aC: animClass.GetLineText(0), aN: animName.GetLineText(0), rD: rootDir.Content.ToString());
                var oldName = lbFiles.Items[i].ToString();
                var stage = Char.GetNumericValue(oldName[^5]) - 1;
                var actor = Char.GetNumericValue(oldName[^8]) - 1;
                var newName = $"0Sx{ls.mN}_{ls.aC}-{ls.aN}_S{stage}_{actor}.hkx";
                Directory.CreateDirectory(Path.Combine(ls.rD, @"meshes\0SA\mod\0Sex\anim\", ls.mN, ls.aC, ls.aN));
                File.Copy(Path.Combine(sourceDir, oldName), Path.Combine(ls.rD, @"meshes\0SA\mod\0Sex\anim\", ls.mN, ls.aC, ls.aN, newName));
            }
        }
    }
}