using System;
using System.IO;
using System.Windows;

namespace OStimConversionTool
{
    public partial class Startup : Window
    {
        private string _sourceDir = string.Empty;
        public AnimationDatabase animationDatabase = new AnimationDatabase();

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

            if (moduleName is null)
                throw new NotImplementedException();
            if (animClass is null)
                throw new NotImplementedException();
            if (animName is null)
                throw new NotImplementedException();
            if (rootDir is null)
                throw new NotImplementedException();

            var animDir = Path.Combine(rootDir, @"meshes\0SA\mod\0Sex\anim\", moduleName, animClass, animName);
            var xmlDir = Path.Combine(rootDir, @"meshes\0SA\mod\0Sex\scene", moduleName, animClass);
            var fnisPath = Path.Combine(rootDir, @$"meshes\actors\character\animations\0Sex_{moduleName}_A");

            if (!Directory.Exists(animDir))
                Directory.CreateDirectory(animDir);

            if (!Directory.Exists(xmlDir))
            {
                Directory.CreateDirectory(xmlDir);
                File.WriteAllText(Path.Combine(xmlDir, $"{animName}.xml"), "");
            }

            if (!Directory.Exists(fnisPath))
            {
                Directory.CreateDirectory(fnisPath);
                File.WriteAllText(Path.Combine(fnisPath, $"FNIS_0Sex_{moduleName}_A_List.txt"), "");
            }

            foreach (var lbFileItem in LbFiles.Items)
            {
                var oldName = lbFileItem?.ToString();
                if (oldName is null) continue;

                var stage = char.GetNumericValue(oldName[^5]) - 1;
                var actor = 0;
                if (Math.Abs(char.GetNumericValue(oldName[^8]) - 1) < double.Epsilon)
                    actor = 1;

                var newName = $"0Sx{moduleName}_{animClass}-{animName}_S{stage}_{actor}.hkx";
                animationDatabase.Add(animName, newName);

                File.Copy(Path.Combine(_sourceDir, oldName), Path.Combine(animDir, newName));
                var contents = @$"b -Tn {Path.GetFileName(newName)} ..\..\..\..\{animDir}\{newName}{Environment.NewLine}";
                File.AppendAllText(Path.Combine(fnisPath, $"FNIS_0Sex_{moduleName}_A_List.txt"), contents);
            }

            XmlScriber(Path.Combine(xmlDir, $"{animName}.xml"));
            LbFiles.Items.Clear();
        }

        private void XmlScriber(string xmlPath)
        {
            var moduleName = ModuleName.GetLineText(0);
            var animClass = AnimClass.GetLineText(0);
            var animName = AnimName.GetLineText(0);
            var animID = $"0Sx{moduleName}_{animClass}-{animName}";

            File.AppendAllText(xmlPath, string.Format(
                @$"<scene id=""{moduleName}||{animClass}|{animName}"" actors=""2"" style=""Oscene"">
<info name="""" animator=""""/>
<anim id=""{animID}_S1"" t=""L"" l=""2""/>
<speed a=""0"" min=""0"" max=""{LbFiles.Items.Count / 2}"" name=""thrusts"">{Environment.NewLine}"));

            for (int i = 0; i < LbFiles.Items.Count / 2; i++)
            {
                var speedUnit = "^idle";
                if (i > 0)
                    speedUnit = "^thrustsPerSec";

                File.AppendAllText(xmlPath, string.Format(
@$"     <sp mtx = ""{speedUnit}"" tar = ""1"" type = """" >
            <anim id=""{animID}_sourceDir{i}"" t=""L"" l=""2"" i0=""{i}"" i1=""{i}""/>"));

                if (i > 0)
                    File.AppendAllText(xmlPath, string.Format(
$@"
            <ae evty=""sexThrustR"" whodid=""0"" tar=""1"" ori=""pussy"" thrust=""cock"" sound=""pussy"" soundpow=""0""
            imp=""ass"" impside=""R"" impforce=""2"" impdmg="".01"" thrustforce=""2"" oridmg="".01"" oriopen="".01""/>
</anim>{Environment.NewLine}"));

                File.AppendAllText(xmlPath, $"</sp>{Environment.NewLine}");
            }
        }
    }
}
