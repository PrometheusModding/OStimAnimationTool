using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DynamicData;
using DynamicData.Binding;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.Models.Navigation;
using OStimAnimationTool.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using DialogResult = System.Windows.Forms.DialogResult;

namespace OStimConversionTool.ViewModels
{
    public class BadXmlException : XmlException
    {
        public BadXmlException(string message) : base(message)
        {
        }
    }

    public class
        MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IDialogService dialogService, IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            NewDatabaseCommand = new DelegateCommand(NewAnimationDatabase);
            LoadDatabaseCommand = new DelegateCommand(LoadDatabase);
            SaveDatabaseCommand = new DelegateCommand(SaveDatabase, () => AnimationDatabase.IsValueCreated);
        }

        public DelegateCommand NewDatabaseCommand { get; }
        public DelegateCommand LoadDatabaseCommand { get; }
        public DelegateCommand SaveDatabaseCommand { get; }

        //TODO:Overhaul Database save system
        /*private static bool WellFormedDatabase()
        {
            return AnimationDatabase.IsValueCreated &&
                   AnimationDatabase.Instance.Modules.All(module => module.AnimationSets.All(animationSet =>
                       !string.IsNullOrEmpty(animationSet
                           .AnimationClass) &&
                       !string.IsNullOrEmpty(animationSet
                           .PositionKey)));
        }*/

        // Method for initializing New Database
        private void NewAnimationDatabase()
        {
            _dialogService.ShowDialog("NewAnimationDatabaseDialog", result =>
            {
                if (result.Result != ButtonResult.OK) return;

                // Clearing Database
                AnimationDatabase.Instance.Modules.Clear();
                AnimationDatabase.Instance.Misc.Clear();
                AnimationDatabase.Instance.SafePath = string.Empty;

                if (!string.IsNullOrEmpty(result.Parameters.GetValue<string>("name")))
                    AnimationDatabase.Instance.Name = result.Parameters.GetValue<string>("name");

                // Loading default OSex Animations
                FolderBrowserDialog folderBrowserDialog = new();
                {
                    folderBrowserDialog.UseDescriptionForTitle = true;
                    folderBrowserDialog.Description =
                        @"Please select your OSex scene Folder";
                    folderBrowserDialog.ShowDialog();

                    var oSexXmlDirectory = folderBrowserDialog.SelectedPath;
                    LoadOSexAnimations(oSexXmlDirectory);
                }

                AnimationDatabase.Instance.Modules.ToObservableChangeSet().AutoRefresh(x => x.AnimationSets).Subscribe(
                    _ =>
                    {
                        foreach (var module in AnimationDatabase.Instance.Modules)
                            module.AnimationSets.ToObservableChangeSet().AutoRefresh(x => x.SceneId)
                                .Subscribe(_ => SaveDatabaseCommand.RaiseCanExecuteChanged());
                    });

                _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView");
                _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
            });
        }

        // Method for loading OSex Animations into a new Database
        private static void LoadOSexAnimations(string sceneDir)
        {
            var oSexDirectory = sceneDir.Replace("scene", "anim");
            // Creating the Modules depending on the Folders inside your OSex Scene Directory
            foreach (var moduleDir in Directory.GetDirectories(sceneDir))
            {
                var moduleName = Path.GetFileName(moduleDir);
                var module = new Module(moduleName);
                AnimationDatabase.Instance.Modules.Add(module);
            }

            foreach (var moduleDir in Directory.GetDirectories(sceneDir))
            foreach (var positionDir in Directory.GetDirectories(moduleDir))
            foreach (var classDir in Directory.GetDirectories(positionDir))
            foreach (var file in Directory.GetFiles(classDir))
                try
                {
                    // Loading Xml Files
                    XElement doc = XElement.Load(file);

                    // Getting necessary information from the .xml
                    var sceneId = doc.Attribute("id")?.Value;
                    if (sceneId is null) throw new BadXmlException($"{file}: Bad Xml, Faulty SceneID");
                    var actors = doc.Attribute("actors")?.Value;
                    if (actors is null)
                        throw new BadXmlException($"{file}: Bad Xml, Missing Actors Attribute");
                    var animationId = doc.Element("anim")?.Attribute("id")?.Value ?? doc.Element("animation")
                        ?.Element("animrole")?.Element("animplan")?.Element("anim")?.Attribute("id")
                        ?.Value;

                    if (animationId is null)
                    {
                        throw new BadXmlException($"{file}: Bad Xml, Missing AnimationID");
                    }

                    // Getting optional information from the .xml
                    var description = doc.Element("info")?
                        .Attribute("name")?.Value;
                    var animator = doc.Element("info")?
                        .Attribute("animator")?.Value;

                    var sceneIdArray = sceneId.Split('|');

                    if (sceneIdArray.Length != 4)
                        throw new BadXmlException($"{file}: Bad Xml, Faulty SceneID");

                    // Checking if SceneID matches File Location
                    if (sceneIdArray[0] != Path.GetFileName(moduleDir))
                        throw new BadXmlException(
                            $"{file}: Bad Xml, (Module) SceneID doesn't match file location");
                    if (sceneIdArray[1].Replace("!", "") != Path.GetFileName(positionDir))
                        throw new BadXmlException(
                            $"{file}: Bad Xml, (PositionKey) SceneID doesn't match file location");
                    if (sceneIdArray[2] != Path.GetFileName(classDir))
                        throw new BadXmlException(
                            $"{file}: Bad Xml, (AnimationClass) SceneID doesn't match file location");
                    if (sceneIdArray[3] != Path.GetFileNameWithoutExtension(file))
                        throw new BadXmlException(
                            $"{file}: Bad Xml, (SetName) SceneID doesn't match file location");

                    // Modifying AnimationSet with correct sceneID if already in the Database else creates a new AnimationSet
                    var animationSet = SetFinder(sceneIdArray);
                    if (animationSet is null) throw new BadXmlException($"{file}: BadXml");

                    if (description != null) animationSet.Description = description;
                    if (animator != null) animationSet.Animator = animator;
                    animationSet.Is0SexAnimation = true;

                    AnimationSet? destinationAnimationSet;
                    switch (animationSet)
                    {
                        case TransitionAnimationSet transitionAnimationSet:
                            var destinationId = doc.Element("anim")?.Attribute("dest")?.Value;
                            if (destinationId is null)
                                throw new BadXmlException(
                                    $"{file}: BadXml, Transition is missing Destination");

                            // Appending to SceneID if "^" Notation is used
                            if (destinationId.StartsWith('^')) destinationId = sceneId + destinationId[1..];

                            // Setting the Transition Destination
                            var destinationIdArray = destinationId.Split('|');
                            if (destinationIdArray.Length != 4)
                                throw new BadXmlException(
                                    $"{file}: BadXml, Destination has faulty SceneID");

                            destinationAnimationSet = SetFinder(destinationIdArray);
                            if (destinationAnimationSet is null)
                                throw new BadXmlException(
                                    $"{file}: BadXml, Destination has faulty SceneID");
                            destinationAnimationSet.Is0SexAnimation = true;
                            transitionAnimationSet.Destination = destinationAnimationSet;

                            // Defining Animations Contained in AnimationSet
                            for (var i = 0; i < int.Parse(actors); i++)
                            {
                                var oldPath = sceneIdArray[0].Equals("EMF")
                                    ? Path.Combine(oSexDirectory, sceneIdArray[0],
                                        sceneIdArray[1].Replace("!", ""),
                                        sceneIdArray[2], transitionAnimationSet.ParentSet,
                                        transitionAnimationSet.Emf,
                                        animationId + $"_{i}.hkx")
                                    : Path.Combine(oSexDirectory, sceneIdArray[0],
                                        sceneIdArray[1].Replace("!", ""),
                                        sceneIdArray[2], transitionAnimationSet.ParentSet,
                                        animationId + $"_{i}.hkx");

                                var animation = new Animation(oldPath, animationSet)
                                {
                                    Speed = 0,
                                    Actor = i
                                };

                                if (File.Exists(animation.OldPath)) animationSet.Animations.Add(animation);
                            }

                            break;

                        case HubAnimationSet hubAnimationSet:
                        {
                            if (file == "AutoStartBasic")
                            {
                                //TODO
                            }
                            else
                            {
                                foreach (var tabElement in doc.Elements("nav").Elements("tab"))
                                {
                                    var tab = new Tab
                                    {
                                        Actor = tabElement.Attribute("actor")?.Value switch
                                        {
                                            "0" => Actors.Dom,
                                            "1" => Actors.Sub,
                                            null => Actors.Dom,
                                            _ => throw new ArgumentOutOfRangeException()
                                        },
                                        Icon = tabElement.Attribute("icon")?.Value switch
                                        {
                                            "sdom" => TabIcons.SDom,
                                            "ssub" => TabIcons.SSub,
                                            "smale" => TabIcons.SMale,
                                            "sfemale" => TabIcons.SFemale,
                                            "splus" => TabIcons.SPlus,
                                            "sobs" => TabIcons.SObs,
                                            _ => throw new ArgumentOutOfRangeException()
                                        },
                                        TextColor = tabElement.Element("hue")?.Attribute("a")?.Value switch
                                        {
                                            "0" => Colors.Blue,
                                            "1" => Colors.Red,
                                            null => Colors.Blue,
                                            _ => throw new Exception()
                                        },
                                        IconColor = tabElement.Element("bnhue")?.Attribute("a")?.Value switch
                                        {
                                            "0" => Colors.Blue,
                                            "1" => Colors.Red,
                                            null => Colors.Blue,
                                            _ => throw new Exception()
                                        }
                                    };

                                    if (tabElement.Element("xmenu") is not null)
                                    {
                                        var xmenu = new XMenu();

                                        tab.XMenu = xmenu;
                                    }

                                    hubAnimationSet.NavTabs.Add(tab);

                                    foreach (var pageElement in tabElement.Elements("page"))
                                    {
                                        var page = new Page
                                        {
                                            Icon = pageElement.Attribute("icon")?.Value switch
                                            {
                                                "mass" => PageIcons.MAss,
                                                "mcuirass" => PageIcons.MCuirass,
                                                "mgenim" => PageIcons.MGenIm,
                                                "mgenimm2" => PageIcons.MGenIm,
                                                "mgensignf" => PageIcons.MGenSignF,
                                                "mgensignm" => PageIcons.MGenSignM,
                                                "mhand" => PageIcons.MHand,
                                                "mhandex" => PageIcons.MHandEx,
                                                "mheart" => PageIcons.MHeart,
                                                "mintimatef" => PageIcons.MIntimateF,
                                                "mmagi" => PageIcons.MMagi,
                                                "morif" => PageIcons.MOrif,
                                                "mshirt" => PageIcons.MShirt,
                                                "mtri" => PageIcons.MTri,
                                                "mtriex" => PageIcons.MTriEx,
                                                "mtritri" => PageIcons.MTriTri,
                                                "mwhipcream" => PageIcons.MWhipCream,
                                                _ => throw new ArgumentOutOfRangeException()
                                            },
                                            IconColor = pageElement.Element("hue")?.Attribute("a")?.Value switch
                                            {
                                                "0" => Colors.Blue,
                                                "1" => Colors.Red,
                                                null => Colors.Blue,
                                                _ => throw new Exception()
                                            }
                                        };
                                        tab.Pages.Add(page);

                                        foreach (var optionElement in pageElement.Elements("option"))
                                        {
                                            var destination = optionElement.Attribute("go")?.Value;

                                            if (destination is null)
                                                throw new BadXmlException(
                                                    $"{file}: BadXml, Destination has faulty SceneID");

                                            // Adding destinations to the AnimationSets Destination List
                                            // Annotation: LINQ handles "^"-Notation
                                            var destIdArray = (destination.StartsWith('^')
                                                ? sceneId + destination[1..]
                                                : destination).Split('|');
                                            {
                                                if (destIdArray.Length != 4)
                                                    throw new BadXmlException(
                                                        $"{file}: BadXml, Destination has faulty SceneID");

                                                destinationAnimationSet = SetFinder(destIdArray);
                                                if (destinationAnimationSet is null)
                                                    throw new BadXmlException(
                                                        $"{file}: BadXml, Destination has faulty SceneID");
                                                destinationAnimationSet.Is0SexAnimation = true;
                                            }

                                            var option = new Option(destinationAnimationSet);
                                            var icon = optionElement.Attribute("icon")?.Value;

                                            if (icon != null)
                                            {
                                                var m = Regex.Match(icon, "\\$G");

                                                while (m.Success)
                                                {
                                                    m = Regex.Match(icon, "\\$G");

                                                    icon = icon[m.Index + 2] switch
                                                    {
                                                        '0' => icon.Replace("$G0", "m"),
                                                        '1' => icon.Replace("$G1", "f"),
                                                        _ => icon
                                                    };
                                                }
                                            }

                                            option.Icon = icon switch
                                            {
                                                "o1fpufrx_mf" => OptionIcons.o1fpufrx_mf,
                                                "o1fpufr_mf" => OptionIcons.o1fpufr_mf,
                                                "o2fpufrx_mf" => OptionIcons.o2fpufrx_mf,
                                                "o2fpufr_mf" => OptionIcons.o2fpufr_mf,
                                                "obalhos2_f" => OptionIcons.obalhos2_f,
                                                "obb1freartx_mf" => OptionIcons.obb1freartx_mf,
                                                "obb1freart_mf" => OptionIcons.obb1freart_mf,
                                                "obb2freartx_mf" => OptionIcons.obb2freartx_mf,
                                                "obb2freart_mf" => OptionIcons.obb2freart_mf,
                                                "obbpenrearts2_f" => OptionIcons.obbpenrearts2_f,
                                                "obbpenrearts2_m" => OptionIcons.obbpenrearts2_m,
                                                "obbpenxrearts2_f" => OptionIcons.obbpenxrearts2_f,
                                                "obbpenxrearts2_m" => OptionIcons.obbpenxrearts2_m,
                                                "obbspankl_6_f" => OptionIcons.obbspankl_6_f,
                                                "obbspankr_6_f" => OptionIcons.obbspankr_6_f,
                                                "obbstup_f" => OptionIcons.obbstup_f,
                                                "obigx" => OptionIcons.obigx,
                                                "objbf_f" => OptionIcons.objbf_f,
                                                "objbos_mf" => OptionIcons.objbos_mf,
                                                "obododn_f" => OptionIcons.obododn_f,
                                                "obodoup_f" => OptionIcons.obodoup_f,
                                                "obombpenrears2_f" => OptionIcons.obombpenrears2_f,
                                                "obustt180_f" => OptionIcons.obustt180_f,
                                                "ocurlidn_9_f" => OptionIcons.ocurlidn_9_f,
                                                "ocurliup_9_f" => OptionIcons.ocurliup_9_f,
                                                "ofdaut_f" => OptionIcons.ofdaut_f,
                                                "ohandsoffbody_ff" => OptionIcons.ohandsoffbody_ff,
                                                "ohandsoffbody_fm" => OptionIcons.ohandsoffbody_fm,
                                                "ohandsoffbody_mf" => OptionIcons.ohandsoffbody_mf,
                                                "ohandsoffbody_mm" => OptionIcons.ohandsoffbody_mm,
                                                "ohgpenfrs2_f" => OptionIcons.ohgpenfrs2_f,
                                                "ohgpenxfrs2_f" => OptionIcons.ohgpenxfrs2_f,
                                                "ohjdubstand_mf" => OptionIcons.ohjdubstand_mf,
                                                "ohjs2_9_f" => OptionIcons.ohjs2_9_f,
                                                "ohjs2_9_m" => OptionIcons.ohjs2_9_m,
                                                "ohjugs2_9_f" => OptionIcons.ohjugs2_9_f,
                                                "ohjugs2_9_m" => OptionIcons.ohjugs2_9_m,
                                                "ohjugxs2_9_f" => OptionIcons.ohjugxs2_9_f,
                                                "ohjugxs2_9_m" => OptionIcons.ohjugxs2_9_m,
                                                "ohjxs2_9_f" => OptionIcons.ohjxs2_9_f,
                                                "ohjxs2_9_m" => OptionIcons.ohjxs2_9_m,
                                                "oholdbody_8_mf" => OptionIcons.oholdbody_8_mf,
                                                "oikea_mf" => OptionIcons.oikea_mf,
                                                "okalbdn_f" => OptionIcons.okalbdn_f,
                                                "okalbup_f" => OptionIcons.okalbup_f,
                                                "oknlbdn_f" => OptionIcons.oknlbdn_f,
                                                "oknlbup_f" => OptionIcons.oknlbup_f,
                                                "olipsx" => OptionIcons.olipsx,
                                                "oludn_6_f" => OptionIcons.oludn_6_f,
                                                "oludn_9_f" => OptionIcons.oludn_9_f,
                                                "oluup_6_f" => OptionIcons.oluup_6_f,
                                                "oluup_9_f" => OptionIcons.oluup_9_f,
                                                "oluxdn_6_f" => OptionIcons.oluxdn_6_f,
                                                "oluxdn_9_f" => OptionIcons.oluxdn_9_f,
                                                "oluxup_6_f" => OptionIcons.oluxup_6_f,
                                                "oluxup_9_f" => OptionIcons.oluxup_9_f,
                                                "omgsholds2_f" => OptionIcons.omgsholds2_f,
                                                "omgsholds2_m" => OptionIcons.omgsholds2_m,
                                                "omgsletgos2_f" => OptionIcons.omgsletgos2_f,
                                                "omgsletgos2_m" => OptionIcons.omgsletgos2_m,
                                                "oplumpf180_f" => OptionIcons.oplumpf180_f,
                                                "oplumppenx_f" => OptionIcons.oplumppenx_f,
                                                "oplumppen_f" => OptionIcons.oplumppen_f,
                                                "opowemb_mf" => OptionIcons.opowemb_mf,
                                                "opowkiss_mf" => OptionIcons.opowkiss_mf,
                                                "osbtkstra_6_f" => OptionIcons.osbtkstra_6_f,
                                                "osc_bjportrait_mf" => OptionIcons.osc_bjportrait_mf,
                                                "osc_cowgirl_mf" => OptionIcons.osc_cowgirl_mf,
                                                "osc_handlebj_f" => OptionIcons.osc_handlebj_f,
                                                "osc_mufold_mf" => OptionIcons.osc_mufold_mf,
                                                "osc_pantypeelrear_f" => OptionIcons.osc_pantypeelrear_f,
                                                "osc_powerkiss_mf" => OptionIcons.osc_powerkiss_mf,
                                                "osc_sexcradle_mf" => OptionIcons.osc_sexcradle_mf,
                                                "osc_wizsex_mf" => OptionIcons.osc_wizsex_mf,
                                                "oslnbobkd_f" => OptionIcons.oslnbobkd_f,
                                                "oslnbobku_f" => OptionIcons.oslnbobku_f,
                                                "oslrevayd_f" => OptionIcons.oslrevayd_f,
                                                "oslrevayu_f" => OptionIcons.oslrevayu_f,
                                                "osn_lflotus_mf" => OptionIcons.osn_lflotus_mf,
                                                "osn_st6ho_mf" => OptionIcons.osn_st6ho_mf,
                                                "osn_st9ho_mf" => OptionIcons.osn_st9ho_mf,
                                                "osn_stknap_mf" => OptionIcons.osn_stknap_mf,
                                                "osqtlbbdn_f" => OptionIcons.osqtlbbdn_f,
                                                "ostbkstra_9_f" => OptionIcons.ostbkstra_9_f,
                                                "ostgbo45_f" => OptionIcons.ostgbo45_f,
                                                "osx_180bust_f" => OptionIcons.osx_180bust_f,
                                                "osx_bklomau_f" => OptionIcons.osx_bklomau_f,
                                                "osx_bodd_f" => OptionIcons.osx_bodd_f,
                                                "osx_bodo_f" => OptionIcons.osx_bodo_f,
                                                "osx_bom_f" => OptionIcons.osx_bom_f,
                                                "osx_do_f" => OptionIcons.osx_do_f,
                                                "osx_embkiss_mf" => OptionIcons.osx_embkiss_mf,
                                                "osx_feettoesup_f" => OptionIcons.osx_feettoesup_f,
                                                "osx_jerk_m" => OptionIcons.osx_jerk_m,
                                                "osx_ka_f" => OptionIcons.osx_ka_f,
                                                "osx_kf_f" => OptionIcons.osx_kf_f,
                                                "osx_kn6x_f" => OptionIcons.osx_kn6x_f,
                                                "osx_kn6y_f" => OptionIcons.osx_kn6y_f,
                                                "osx_kn_f" => OptionIcons.osx_kn_f,
                                                "osx_kn_m" => OptionIcons.osx_kn_m,
                                                "osx_layonback_f" => OptionIcons.osx_layonback_f,
                                                "osx_layside_f" => OptionIcons.osx_layside_f,
                                                "osx_legcurl_f" => OptionIcons.osx_legcurl_f,
                                                "osx_letgomale_m" => OptionIcons.osx_letgomale_m,
                                                "osx_mu01_f" => OptionIcons.osx_mu01_f,
                                                "osx_mu02_f" => OptionIcons.osx_mu02_f,
                                                "osx_mu03_f" => OptionIcons.osx_mu03_f,
                                                "osx_penpusrearout_f" => OptionIcons.osx_penpusrearout_f,
                                                "osx_penpusrear_f" => OptionIcons.osx_penpusrear_f,
                                                "osx_spankl_f" => OptionIcons.osx_spankl_f,
                                                "osx_spankr_f" => OptionIcons.osx_spankr_f,
                                                "osx_sqt_f" => OptionIcons.osx_sqt_f,
                                                "osx_starch_f" => OptionIcons.osx_starch_f,
                                                "osx_stbkstra_f" => OptionIcons.osx_stbkstra_f,
                                                "osx_stlegspr_f" => OptionIcons.osx_stlegspr_f,
                                                "osx_st_f" => OptionIcons.osx_st_f,
                                                "osx_st_m" => OptionIcons.osx_st_m,
                                                "osx_thispr_f" => OptionIcons.osx_thispr_f,
                                                "osx_tumli_f" => OptionIcons.osx_tumli_f,
                                                "otiptoedn_f" => OptionIcons.otiptoedn_f,
                                                "otiptoeup_f" => OptionIcons.otiptoeup_f,
                                                null => OptionIcons.ofdaut_f,
                                                _ => OptionIcons.ofdaut_f
                                            };

                                            page.Options.Add(option);
                                        }
                                    }
                                }
                            }

                            // Destinations are different in the AutoStart-Xml
                            var destinations = file == "AutoStartBasic"
                                ? doc.Elements("togs").Elements("tog0").Elements("dest").Attributes("id")
                                    .Select(d => d.Value).ToList()
                                : doc.Elements("nav").Elements("tab").Elements("page").Elements("option")
                                    .Attributes("go").Select(d => d.Value).ToList();

                            // Defining Animations Contained in AnimationSet
                            var animations = doc.Element("speed") is null
                                ? new List<string> { animationId }
                                : doc.Elements("speed").Elements("sp").Elements("anim").Attributes("id")
                                    .Select(animation => animation.Value);

                            foreach (var anim in animations)
                                for (var i = 0; i < int.Parse(actors); i++)
                                {
                                    var oldPath = Path.Combine(oSexDirectory, sceneIdArray[0],
                                        sceneIdArray[1].Replace("!", ""), sceneIdArray[2], sceneIdArray[3],
                                        anim + $"_{i}.hkx");

                                    var animation = new Animation(oldPath, animationSet)
                                    {
                                        Speed = (int)char.GetNumericValue(anim[^1]),
                                        Actor = i
                                    };

                                    if (File.Exists(animation.OldPath)) animationSet.Animations.Add(animation);
                                }

                            break;
                        }
                    }
                }
                catch (XmlException e)
                {
                    if (e is BadXmlException) MessageBox.Show(e.Message);
                }
        }

        // Method responsible for loading Database Files
        private void LoadDatabase()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Animation Database file (*.xml)|*xml",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                AnimationDatabase.Instance.Modules.Clear();
                AnimationDatabase.Instance.Misc.Clear();

                var databaseFile = fileDialog.FileName;

                // Load Database Xml
                XElement doc = XElement.Load(databaseFile);

                AnimationDatabase.Instance.Name = doc.Attribute("Name")?.Value ??
                                                  throw new BadXmlException($"{databaseFile}: Invalid Name");
                AnimationDatabase.Instance.SafePath = Path.GetDirectoryName(fileDialog.FileName) ??
                                                      throw new BadXmlException($"{databaseFile}Invalid Safe Location");

                // Parser for Database Xml
                foreach (var moduleElement in doc.Elements("Module"))
                {
                    var moduleName = moduleElement.Attribute("Name")?.Value;
                    if (moduleName is null) throw new BadXmlException($"{databaseFile}: Invalid Module Name");

                    var module = new Module(moduleName);
                    AnimationDatabase.Instance.Modules.Add(module);

                    // Different parsing for Hub- and Transitionanimationsets
                    foreach (var hubAnimationSetElement in moduleElement.Elements("Hub"))
                    {
                        var sceneId = hubAnimationSetElement.Attribute("SceneID")?.Value;
                        if (sceneId is null) throw new BadXmlException($"{databaseFile}: Invalid SceneID");

                        var animationSet = SetFinder(sceneId.Split('|'));
                        if (animationSet is not HubAnimationSet hubAnimationSet)
                            throw new BadXmlException($"{databaseFile}: Invalid Animationset");

                        animationSet.Module = module;
                        animationSet.Animator = hubAnimationSetElement.Attribute("Animator")?.Value ??
                                                string.Empty;
                        animationSet.Description = hubAnimationSetElement.Attribute("Description")?.Value ??
                                                   string.Empty;

                        foreach (var destination in hubAnimationSetElement.Elements("Destination"))
                        {
                            var destinationSceneId = destination.Value;

                            var destinationSet = SetFinder(destinationSceneId.Split('|'));
                            if (destinationSet is null)
                                throw new BadXmlException($"{databaseFile}: Invalid Animationset");

                            //hubAnimationSet.NavTabs.Add(destinationSet);
                        }

                        foreach (var animationElement in hubAnimationSetElement.Elements("Animation"))
                        {
                            var name = animationElement.Attribute("Name")?.Value;
                            if (name is null) throw new BadXmlException($"{databaseFile}: Invalid animation name");

                            var fnisArgs = animationElement.Attribute("FnisArgument")?.Value.Split(',');
                            if (fnisArgs is null) throw new BadXmlException($"{databaseFile}: Invalid FNIS arguments");

                            var creature = animationElement.Attribute("Creature")?.Value;
                            if (creature is null) throw new BadXmlException($"{databaseFile}: Invalid Creature");

                            var animation = new Animation(animationSet, name, fnisArgs, creature);
                            animationSet.Animations.Add(animation);
                        }
                    }

                    foreach (var transitionAnimationSetElement in moduleElement.Elements("Transition"))
                    {
                        var sceneId = transitionAnimationSetElement.Attribute("SceneID")?.Value;
                        if (sceneId is null) throw new BadXmlException($"{databaseFile}: Invalid SceneID");

                        var animationSet = SetFinder(sceneId.Split('|'));
                        if (animationSet is not TransitionAnimationSet transitionAnimationSet)
                            throw new BadXmlException($"{databaseFile}: Invalid Animationset");

                        animationSet.Module = module;
                        animationSet.Animator = transitionAnimationSetElement.Attribute("Animator")?.Value ??
                                                string.Empty;
                        animationSet.Description = transitionAnimationSetElement.Attribute("Description")?.Value ??
                                                   string.Empty;

                        var destinationSceneId = transitionAnimationSetElement.Attribute("Destination")?.Value;
                        if (destinationSceneId is null)
                            throw new BadXmlException($"{databaseFile}: Invalid SceneID");

                        var destination = SetFinder(destinationSceneId.Split('|'));
                        if (destination is null) throw new BadXmlException($"{databaseFile}: Invalid Animationset");

                        transitionAnimationSet.Destination = destination;

                        foreach (var animationElement in transitionAnimationSetElement.Elements("Animation"))
                        {
                            var name = animationElement.Attribute("Name")?.Value;
                            if (name is null) throw new BadXmlException($"{databaseFile}: Invalid animation name");

                            var fnisArgs = animationElement.Attribute("FnisArgument")?.Value.Split(',');
                            if (fnisArgs is null) throw new BadXmlException($"{databaseFile}: Invalid FNIS arguments");

                            var creature = animationElement.Attribute("Creature")?.Value;
                            if (creature is null) throw new BadXmlException($"{databaseFile}: Invalid Creature");

                            var animation = new Animation(animationSet, name, fnisArgs, creature);
                            animationSet.Animations.Add(animation);
                        }
                    }
                }
            }
            catch (BadXmlException e)
            {
                MessageBox.Show(e.Message);
            }

            _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView");
            _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
        }

        // Method for Saving Database
        private static void SaveDatabase()
        {
            if (string.IsNullOrEmpty(AnimationDatabase.Instance.SafePath))
            {
                FolderBrowserDialog folderBrowserDialog = new();
                {
                    folderBrowserDialog.ShowDialog();
                }

                if (!Directory.Exists(folderBrowserDialog.SelectedPath)) return;
                AnimationDatabase.Instance.SafePath = folderBrowserDialog.SelectedPath;
            }


            DatabaseScriber databaseScriber = new();
            databaseScriber.XmlScriber();
            databaseScriber.FnisScriber();
            databaseScriber.DatabaseFileScriber();
        }

        private static AnimationSet? SetFinder(IReadOnlyList<string> sceneId)
        {
            foreach (var module in AnimationDatabase.Instance.Modules)
            {
                if (!module.Name.Equals(sceneId[0])) continue;

                AnimationSet animationSet;

                foreach (var set in module.AnimationSets)
                {
                    if (!set.SetName.Equals(sceneId[3])) continue;
                    animationSet = set;
                    return animationSet;
                }

                var positionKey = sceneId[1];
                var animationClass = sceneId[2];
                var setName = sceneId[3];

                animationSet = setName.Contains('+')
                    ? new TransitionAnimationSet(module, positionKey, animationClass, setName)
                    : new HubAnimationSet(module, positionKey, animationClass, setName);

                module.AnimationSets.Add(animationSet);

                return animationSet;
            }

            return null;
        }
    }
}
