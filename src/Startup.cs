using System;
using System.IO;
using System.Windows;

namespace OStimConversionTool
{
    public partial class Startup
    {
        private string _sourceDir = string.Empty;

        public Startup()
        {
            InitializeComponent();
        }

        private void ChooseFiles_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "Havok Animation files (*.hkx)|*hkx|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            
            if (openFileDialog.ShowDialog() != true) return;
            
            _sourceDir = openFileDialog.FileName;
            _sourceDir = _sourceDir.Remove(_sourceDir.Length - openFileDialog.SafeFileName.Length);
            
            foreach (string filename in openFileDialog.FileNames)
                LbFiles.Items.Add(Path.GetFileName(filename));
            MessageBox.Show($"Source Dir is {_sourceDir}");
        }

        private void ChooseRootDir_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderBrowserDialog = new();
            {
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    RootDir.Content = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void ConversionProcess_Click(object sender, RoutedEventArgs e)
        {
            var moduleName = ModuleName.GetLineText(0);
            var animClass = AnimClass.GetLineText(0);
            var animName = AnimName.GetLineText(0);
            var rootDir = RootDir.Content?.ToString();

            if (moduleName == null)
                throw new NotImplementedException();
            if (animClass == null)
                throw new NotImplementedException();
            if (animName == null)
                throw new NotImplementedException();
            if (rootDir == null)
                throw new NotImplementedException();
            
            var animDir = Path.Combine(rootDir, @"meshes\0SA\mod\0Sex\anim\", moduleName, animClass, animName);
            var xmlDir = Path.Combine(rootDir, @"meshes\0SA\mod\0Sex\scene", moduleName, animClass);
            var fnisPath = Path.Combine(rootDir, @$"meshes\actors\character\animations\0Sex_{moduleName}_A");

            if (!Directory.Exists(animDir))
                Directory.CreateDirectory(animDir);

            if (!Directory.Exists(xmlDir))
            {
                Directory.CreateDirectory(xmlDir);
                File.WriteAllText(Path.Combine(xmlDir, $"{animName}"), "");
            }

            if (!Directory.Exists(fnisPath))
            {
                Directory.CreateDirectory(fnisPath);
                File.WriteAllText(Path.Combine(fnisPath, $"FNIS_0Sex_{moduleName}_A_List.txt"), "");
            }

            foreach (var lbFileItem in LbFiles.Items)
            {
                var oldName = lbFileItem?.ToString();
                if (oldName == null) continue;

                var stage = char.GetNumericValue(oldName[^5]) - 1;
                var actor = 0;
                if (Math.Abs(char.GetNumericValue(oldName[^8]) - 1) < double.Epsilon)
                    actor = 1;
                
                var newName = $"0Sx{moduleName}_{animClass}-{animName}_S{stage}_{actor}.hkx";
                
                File.Copy(Path.Combine(_sourceDir, oldName), Path.Combine(animDir, newName));
                var contents = @$"b -Tn {Path.GetFileName(newName)} ..\..\..\..\{animDir}\{newName}{Environment.NewLine}";
                File.AppendAllText(Path.Combine(fnisPath, $"FNIS_0Sex_{moduleName}_A_List.txt"), contents);
            }
            
            XmlScriber(Path.Combine(xmlDir, $"{animName}"));
            LbFiles.Items.Clear();
        }

        private static void XmlScriber(string xmlPath)
        {
        }
    }
}
