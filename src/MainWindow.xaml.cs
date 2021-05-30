using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;

namespace OStimConversionTool
{
    public partial class MainWindow
    {
        private string _sourceDir = string.Empty;
        private string _animationName = string.Empty;
        private string _animationClass = string.Empty;
        private string _animator = string.Empty;
        private readonly AnimationDatabase _animationDatabase = new();
        internal CollectionViewSource AnimationDatabaseViewSource { get; set; } = new CollectionViewSource();
        public ICollectionView AnimationDatabaseView { get { return AnimationDatabaseViewSource.View; } }

        public MainWindow()
        {
            AnimationDatabaseViewSource.Source = _animationDatabase;
            AnimationDatabaseView.GroupDescriptions.Add(new PropertyGroupDescription("SetName"));

            InitializeComponent();
            DataContext = AnimationDatabaseView;

            StartupWindow startup = new();
            startup.ShowDialog();
        }

        /*private void UngroupButton_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView cvTasks = CollectionViewSource.GetDefaultView(animationDatabaseGrid.ItemsSource);
            if (cvTasks != null)
            {
                cvTasks.GroupDescriptions.Clear();
            }
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView cvTasks = CollectionViewSource.GetDefaultView(animationDatabaseGrid.ItemsSource);
            if (cvTasks != null && cvTasks.CanGroup == true)
            {
                cvTasks.GroupDescriptions.Clear();
                cvTasks.GroupDescriptions.Add(new PropertyGroupDescription("SetName"));
            }
        }*/

        private void ChooseFiles_Click(object sender, RoutedEventArgs e)
        {
            _animator = StartupWindow.animator;

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
                _animationName = !string.IsNullOrEmpty(AnimationSetnameTextbox.Text) ? AnimationSetnameTextbox.Text : Path.GetFileName(filename).Remove(openFileDialog.SafeFileName.Length - 10);
                _animationClass = !string.IsNullOrEmpty(AnimationClassTextbox.Text) ? AnimationClassTextbox.Text : string.Empty;
                Animation anim = new(_animationName, Path.GetFileName(filename), _animationClass, _animator);

                if (!_animationDatabase.Contains(anim))
                    _animationDatabase.Add(anim);
            }

            if (AnimationDatabaseView != null)
            {
                AnimationDatabaseView.GroupDescriptions.Clear();
                AnimationDatabaseView.GroupDescriptions.Add(new PropertyGroupDescription("SetName"));
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
                var animClass = anim.AnimationClass;
                var animName = anim.AnimationName;
                var setName = anim.SetName;

                if (animClass is null)
                    throw new NotImplementedException();

                if (animName is null)
                    throw new NotImplementedException();

                var animDir = Path.Combine(rootDir, @"meshes\0SA\mod\0Sex\anim\", moduleName, animClass, setName);
                var xmlDir = Path.Combine(rootDir, @"meshes\0SA\mod\0Sex\scene", moduleName, animClass);

                if (!Directory.Exists(animDir))
                    Directory.CreateDirectory(animDir);

                if (!Directory.Exists(xmlDir))
                {
                    Directory.CreateDirectory(xmlDir);
                    File.WriteAllText(Path.Combine(xmlDir, $"{setName}.xml"), "");
                }

                var oldName = anim.AnimationName;

                var stage = char.GetNumericValue(oldName[^5]) - 1;
                var actor = 0;
                if (Math.Abs(char.GetNumericValue(oldName[^8]) - 1) < double.Epsilon)
                    actor = 1;

                var newName = $"0Sx{moduleName}_{animClass}-{setName}_S{stage}_{actor}.hkx";
                anim.AnimationName = newName;

                File.Copy(Path.Combine(_sourceDir, oldName), Path.Combine(animDir, newName));
                var contents = @$"b -Tn {newName.Remove(newName.Length - 4)} ..\..\..\..\{Path.Combine(@"0SA\mod\0Sex\anim\", moduleName, animClass, newName)}{Environment.NewLine}";
                File.AppendAllText(Path.Combine(fnisPath, $"FNIS_0Sex_{moduleName}_A_List.txt"), contents);

                XmlScriber(Path.Combine(xmlDir, $"{setName}.xml"), anim, _animationDatabase);
            }
        }

        private static void XmlScriber(string xmlPath, Animation anim, AnimationDatabase animationDatabase)
        {
            var moduleName = StartupWindow.moduleName;
            var setName = anim.SetName;
            var animClass = anim.AnimationClass;
            var animInfo = anim.AnimationInfo;
            var animator = anim.Animator;
            var isTransition = anim.IsTransition;
            var setSize = anim.GetSetSize(animationDatabase);
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
            writer.WriteAttributeString("name", $"{animInfo}");
            writer.WriteAttributeString("animator", $"{animator}");
            writer.WriteEndElement();
            writer.WriteStartElement("anim");
            writer.WriteAttributeString("id", animID);
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
                writer.WriteAttributeString("a", "0");
                writer.WriteAttributeString("min", "0");
                writer.WriteAttributeString("max", $"{setSize}");
                writer.WriteAttributeString("name", "thrusts");

                for (var i = 0; i <= setSize; i++)
                {
                    if (i is 0)
                    {
                        writer.WriteStartElement("sp");
                        writer.WriteAttributeString("mtx", "^idle");
                        writer.WriteAttributeString("tar", "1");
                        writer.WriteAttributeString("type", "");
                        writer.WriteStartElement("anim");
                        writer.WriteAttributeString("id", $"{animID}_S{i}");
                        writer.WriteAttributeString("t", "L");
                        writer.WriteAttributeString("l", "2");
                        writer.WriteAttributeString("i0", $"{i}");
                        writer.WriteAttributeString("i1", $"{i}");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteStartElement("sp");
                        writer.WriteAttributeString("mtx", "^thrustsPerSec");
                        writer.WriteAttributeString("qnt", $"{i / 2}");
                        writer.WriteStartElement("anim");
                        writer.WriteAttributeString("id", $"{animID}_S{i}");
                        writer.WriteAttributeString("t", "L");
                        writer.WriteAttributeString("l", "2");
                        writer.WriteAttributeString("i0", $"{i}");
                        writer.WriteAttributeString("i1", $"{i}");
                        writer.WriteStartElement("ae");
                        writer.WriteAttributeString("evty", "sexThrustR");
                        writer.WriteAttributeString("whodid", "0");
                        writer.WriteAttributeString("tar", "1");
                        writer.WriteAttributeString("ori", "pussy");
                        writer.WriteAttributeString("thrust", "cock");
                        writer.WriteAttributeString("sound", "assimpact");
                        writer.WriteAttributeString("soundpow", "0");
                        writer.WriteAttributeString("imp", "ass");
                        writer.WriteAttributeString("impside", "R");
                        writer.WriteAttributeString("impforce", "2");
                        writer.WriteAttributeString("impdmg", ".01");
                        writer.WriteAttributeString("thrustforce", "2");
                        writer.WriteAttributeString("oridmg", ".01");
                        writer.WriteAttributeString("oriopen", ".01");
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
        }
    }

    public class SetNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is not string) throw new NotImplementedException();
            object[] lol = new object[targetTypes.Length];
            for (var o = 1; o < lol.Length; o++)
            {
                lol[o] = value;
            }

            return lol;
        }
    }

    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }

        public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    FieldInfo? fi = value.GetType().GetField(value.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                    }
                }

                return string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type? _enumType;

        public Type? EnumType
        {
            get => _enumType;
            set
            {
                if (value != _enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }
                    _enumType = value;
                }
            }
        }

        public EnumBindingSourceExtension()
        {
        }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == _enumType)
                throw new InvalidOperationException("The EnumType must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == _enumType)
                return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}
