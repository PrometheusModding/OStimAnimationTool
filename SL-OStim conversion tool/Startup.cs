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
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
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
            }
        }

        private void ChooseRootDir_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderBrowserDialog = new() { };
            {
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    rootDir.Content = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void ConversionProcess_Click(object sender, RoutedEventArgs e)
        {
            var ls = (mN: moduleName.GetLineText(0), aC: animClass.GetLineText(0), aN: animName.GetLineText(0), rD: rootDir.Content.ToString());
            var fnis_path = Path.Combine(ls.rD, @$"meshes\actors\character\animations\0Sex_{ls.mN}_A");
            if (!Directory.Exists(fnis_path))
            {
                Directory.CreateDirectory(fnis_path);
                File.WriteAllText(fnis_path + @$"\FNIS_0Sex_{ls.mN}_A_List.txt", "");
            }
            for (int i = 0; i < lbFiles.Items.Count; i++)
            {
                var oldName = lbFiles.Items[i].ToString();
                var stage = Char.GetNumericValue(oldName[^5]) - 1;
                var actor = Char.GetNumericValue(oldName[^8]) - 1;
                var newName = $"0Sx{ls.mN}_{ls.aC}-{ls.aN}_S{stage}_{actor}.hkx";
                var animDir = Path.Combine(ls.rD, @"meshes\0SA\mod\0Sex\anim\", ls.mN, ls.aC, ls.aN);
                Directory.CreateDirectory(animDir);
                File.Copy(Path.Combine(sourceDir, oldName), Path.Combine(animDir, newName));
                File.AppendAllText(fnis_path + @$"\FNIS_0Sex_{ls.mN}_A_List.txt", @$"b -Tn {newName} ..\..\..\..\{animDir}\{newName}{Environment.NewLine}");
            }
        }
    }
}