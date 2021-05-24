using System;
using System.IO;
using System.Windows;
using System.Xml;

namespace OStimConversionTool
{
    public partial class MainWindow : Window
    {
        private string _sourceDir = string.Empty;
        private string _animationName = string.Empty;
        private string _animationClass = string.Empty;
        private AnimationDatabase _animationDatabase;

        public MainWindow()
        {
            InitializeComponent();
            _animationDatabase = (AnimationDatabase)this.Resources["animationDatabase"];
            StartupWindow startup = new();
            startup.Show();
        }

        private void ChooseFiles_Click(object sender, RoutedEventArgs e)
        {
            _animationName = AnimName.GetLineText(0);
            _animationClass = AnimClass.GetLineText(0);

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
            {
                Animation anim = new(_animationName, Path.GetFileName(filename), _animationClass);
                if (!_animationDatabase.Contains(anim))
                    _animationDatabase.Add(anim);
            }
        }

        private void ConversionProcess_Click(object sender, RoutedEventArgs e)
        {
            var moduleName = StartupWindow.moduleName;
            var rootDir = StartupWindow.rootDir;

            if (moduleName is null)
                throw new NotImplementedException();

            if (rootDir is null)
                throw new NotImplementedException();

            var fnisPath = Path.Combine(rootDir, @$"meshes\actors\character\animations\0Sex_{moduleName}_A");

            if (!Directory.Exists(fnisPath))
            {
                Directory.CreateDirectory(fnisPath);
                File.WriteAllText(Path.Combine(fnisPath, $"FNIS_0Sex_{moduleName}_A_List.txt"), "");
            }

            foreach (Animation anim in _animationDatabase)
            {
                var animClass = AnimClass.GetLineText(0);
                var animName = AnimName.GetLineText(0);

                if (animClass is null)
                    throw new NotImplementedException();

                if (animName is null)
                    throw new NotImplementedException();

                var animDir = Path.Combine(rootDir, @"meshes\0SA\mod\0Sex\anim\", moduleName, animClass, animName);
                var xmlDir = Path.Combine(rootDir, @"meshes\0SA\mod\0Sex\scene", moduleName, animClass);

                if (!Directory.Exists(animDir))
                    Directory.CreateDirectory(animDir);

                if (!Directory.Exists(xmlDir))
                {
                    Directory.CreateDirectory(xmlDir);
                    File.WriteAllText(Path.Combine(xmlDir, $"{animName}.xml"), "");
                }

                var oldName = anim.AnimationName;

                var stage = char.GetNumericValue(oldName[^5]) - 1;
                var actor = 0;
                if (Math.Abs(char.GetNumericValue(oldName[^8]) - 1) < double.Epsilon)
                    actor = 1;

                var newName = $"0Sx{moduleName}_{animClass}-{animName}_S{stage}_{actor}.hkx";
                anim.AnimationName = newName;

                File.Copy(Path.Combine(_sourceDir, oldName), Path.Combine(animDir, newName));
                var contents = @$"b -Tn {Path.GetFileName(newName)} ..\..\..\..\{animDir}\{newName}{Environment.NewLine}";
                File.AppendAllText(Path.Combine(fnisPath, $"FNIS_0Sex_{moduleName}_A_List.txt"), contents);

                XmlScriber(Path.Combine(xmlDir, $"{animName}.xml"), anim);
            }
        }

        private void XmlScriber(string xmlPath, Animation anim)
        {
            var moduleName = StartupWindow.moduleName;
            var setName = anim.SetName;
            var animClass = anim.AnimationClass;
            var isTransition = anim.IsTransition;
            var animID = $"0Sx{moduleName}_{animClass}-{setName}";

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            XmlWriter writer = XmlWriter.Create(xmlPath, settings);

            writer.WriteStartElement("scene");
            writer.WriteAttributeString("id", $"{moduleName}||{animClass}|{setName}");
            writer.WriteAttributeString("actors", "2");
            writer.WriteAttributeString("style", "Oscene");
            writer.WriteStartElement("info", "");
            writer.WriteAttributeString("name", $"|");
            writer.WriteAttributeString("animator", "Ceo");
            writer.WriteEndElement();
            writer.WriteStartElement("anim");
            writer.WriteAttributeString("id", $"{animID}");
            writer.WriteAttributeString("t", "L");
            writer.WriteAttributeString("l", "2");

            if (isTransition)
            {
                writer.WriteAttributeString("dest", $"myDocuments");
                writer.WriteStartElement("dfx");
                writer.WriteAttributeString("a", "0");
                writer.WriteAttributeString("fx", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("dfx");
                writer.WriteAttributeString("a", "1");
                writer.WriteAttributeString("fx", "1");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteEndElement();
                writer.WriteStartElement("speed");
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
        }
    }
}
