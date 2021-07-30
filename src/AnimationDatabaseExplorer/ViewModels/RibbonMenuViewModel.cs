using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DynamicData;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class RibbonMenuViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;

        public RibbonMenuViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;

            AddAnimationSetCommand = new DelegateCommand(AddAnimationSet, ActiveDatabase);
            AddSlAnimationSetCommand = new DelegateCommand(AddSlAnimationSet, ActiveDatabase);
            AddSlFolderCommand = new DelegateCommand(AddSlFolder, ActiveDatabase);

            eventAggregator.GetEvent<OpenDatabaseEvent>()
                .Subscribe(() =>
                {
                    AddSlAnimationSetCommand.RaiseCanExecuteChanged();
                    AddSlFolderCommand.RaiseCanExecuteChanged();
                });
        }

        public DelegateCommand AddAnimationSetCommand { get; }
        public DelegateCommand AddSlAnimationSetCommand { get; }
        public DelegateCommand AddSlFolderCommand { get; }

        private static bool ActiveDatabase()
        {
            return AnimationDatabase.IsValueCreated;
        }

        private static void AddAnimationSet()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Havok Animation files (*.hkx)|*hkx|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() != true) return;

            foreach (var filename in openFileDialog.FileNames)
            {
                var setName = Path.GetFileName(filename[..^4]);
                var animationSet = new AnimationSet(setName);
                var animation = new Animation(filename, animationSet);

                var actorMatch = Regex.Match(setName, @"A(\d)");
                if (actorMatch.Success)
                    animation.Actor = int.Parse(actorMatch.Groups[1].Value);

                var speedMatch = Regex.Match(setName, @"S(\d)");
                if (speedMatch.Success)
                    animation.Speed = int.Parse(speedMatch.Groups[1].Value);

                if (speedMatch.Success && actorMatch.Success)
                    setName = actorMatch.Groups[1].Index < speedMatch.Groups[1].Index
                        ? setName[..(actorMatch.Groups[1].Index - 1)]
                        : setName[..(speedMatch.Groups[1].Index - 1)];
            }
        }

        private static void AddSlAnimationSet()
        {
            /*
                        var fileDialog = new OpenFileDialog
                        {
                            Multiselect = true,
                            Filter = "Havok Animation files (*.hkx)|*hkx|All files (*.*)|*.*",
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        };
            
                        if (fileDialog.ShowDialog() != true) return;
            
                        string setName = string.Empty;
                        AnimationSet animationSet = new(string.Empty);
            
                        foreach (var filename in fileDialog.FileNames)
                        {
                            if (!setName.Equals(Path.GetFileName(filename[..^10])))
                                animationSet = SetFinder(Path.GetFileName(filename[..^10]), new Module(""));
            
                            if (animationSet == null) continue;
                            var animation = new Animation(filename, animationSet)
                            {
                                Actor = (int) char.GetNumericValue(Path.GetFileName(filename)[^8]) == 1 ? 1 : 0,
                                Speed = (int) char.GetNumericValue(Path.GetFileName(filename)[^5]) - 1
                            };
            
                            if (!AnimationDatabase.Instance.Contains(animationSet))
                                //AnimationDatabase.Instance.Add(animationSet);
            
                            if (!animationSet.Animations.Contains(animation))
                                animationSet.Animations.Add(animation);
                        }*/
        }

        // Method responsible for processing entire SLPack
        private static void AddSlFolder()
        {
            string sLFolderPath;

            FolderBrowserDialog folderDialog = new();
            {
                folderDialog.ShowDialog();
                sLFolderPath = folderDialog.SelectedPath;
            }

            var module = new Module(Path.GetFileName(sLFolderPath)[..3]);
            AnimationDatabase.Instance.Modules.Add(module);

            SearchSlFolder(sLFolderPath, module);
        }

        // Recursive search for relevant data within SLDirectory
        private static void SearchSlFolder(string rootDirectory, Module module)
        {
            foreach (var directory in Directory.GetDirectories(rootDirectory))
                // The "animobjects", "textures" and .esp are simply added to the Miscellaneous List of the Database.
                // The "actors" is used to determine the AnimationSets which the Pack contains.
                switch (Path.GetFileName(directory).ToLowerInvariant())
                {
                    case "meshes":
                    {
                        foreach (var dir in Directory.GetDirectories(directory))
                            switch (Path.GetFileName(dir).ToLowerInvariant())
                            {
                                case "actors":
                                    foreach (var d in Directory.GetDirectories(dir)) FnisFinder(d, module);
                                    break;

                                case "animobjects":
                                    AnimationDatabase.Instance.Misc.Add(dir);
                                    break;
                            }

                        break;
                    }

                    case "textures":
                        AnimationDatabase.Instance.Misc.Add(directory);
                        break;

                    default:
                        SearchSlFolder(directory, module);
                        break;
                }

            foreach (var file in Directory.GetFiles(rootDirectory))
                if (Path.GetExtension(file) == ".esp")
                    AnimationDatabase.Instance.Misc.Add(file);
        }

        // Method responsible for parsing FNIS Files
        private static void FnisFinder(string actorsDir, Module module)
        {
            var animationsDir = Path.Combine(actorsDir, "animations");

            if (!Directory.Exists(animationsDir)) return;

            foreach (var dir in Directory.GetDirectories(animationsDir))
            foreach (var file in Directory.GetFiles(dir))
            {
                // Looking for FNIS keyword in filename
                var fileArgs = Path.GetFileName(file).Split('_');
                if (fileArgs[0].ToLowerInvariant() != "fnis") continue;

                // If the FNIS File isn't located in the character directory also looks for the race named in FNIS filename.
                var creature = Path.GetFileName(actorsDir).ToLowerInvariant() != "character"
                    ? $"_{fileArgs[^2]}"
                    : string.Empty;

                if (!module.Creatures.Contains(creature)) module.Creatures.Add(creature);

                foreach (var line in File.ReadAllLines(file))
                {
                    // Only certain Animation types are supported
                    if (string.IsNullOrEmpty(line) || line[0] != 's' && line[0] != '+' && line[0] != 'b') continue;

                    // Splitting the Line into single arguments
                    var fnisArgs = line.Split(' ');

                    // Looking for options in Line
                    var setIndex = fnisArgs[1][0] == '-' ? 2 : 1;

                    // Creating new AnimationSet or using existing one
                    var animationSet = SetFinder(fnisArgs[setIndex][..^6], module);

                    List<string> animationFnisArgs = new() { string.Empty };

                    if (setIndex == 2)
                    {
                        // If existent adds options to FnisArgs
                        animationFnisArgs.Replace(string.Empty, $",{fnisArgs[1][1..]}");
                        animationFnisArgs.AddRange(fnisArgs[4..]);
                    }
                    else
                    {
                        animationFnisArgs.AddRange(fnisArgs[3..]);
                    }

                    var animation = new Animation(Path.Combine(dir, fnisArgs[setIndex + 1]), animationSet,
                        (int)char.GetNumericValue(fnisArgs[setIndex + 1][^5]) - 1,
                        (int)char.GetNumericValue(fnisArgs[setIndex + 1][^8]) == 1 ? 1 : 0,
                        animationFnisArgs, creature);

                    animationSet.Animations.Add(animation);
                }
            }
        }

        // Returns existing Set if already in Database else creates a new one.
        private static AnimationSet SetFinder(string setName, Module module)
        {
            foreach (var _ in AnimationDatabase.Instance.Modules)
            foreach (var animSet in module.AnimationSets)
                if (setName.Equals(animSet.SetName))
                    return animSet;

            HubAnimationSet animationSet = new(setName);
            module.AnimationSets.Add(animationSet);
            animationSet.Module = module;
            return animationSet;
        }
    }
}
