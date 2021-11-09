using System;
using System.IO;
using System.Text;
using System.Xml;
using OStimAnimationTool.Core.Models;
using static System.String;

namespace OStimAnimationTool.Core
{
    // Class responsible for Saving Database related Files
    public class DatabaseScriber
    {
        // Getting Initial Values from the Animation Database Instance
        private readonly AnimationDatabase _animationDatabase = AnimationDatabase.Instance;
        private readonly string _safePath = AnimationDatabase.Instance.SafePath;

        // Method responsible for arranging .hkx files in correct Folder Structure 
        public DatabaseScriber()
        {
            foreach (var module in _animationDatabase.Modules)
            foreach (var animationSet in module.AnimationSets)
            {
                if (!animationSet.ChangedThisSession || animationSet.Is0SexAnimation ||
                    animationSet.AnimationClass is null || animationSet.PositionKey is null) continue;

                // Base Path for all Animation Sets
                var setDir = Path.Combine(_animationDatabase.SafePath, @"meshes\0SA\mod\0Sex\anim\",
                    animationSet.Module.Name, animationSet.PositionKey.Replace("!", ""),
                    animationSet.AnimationClass);

                // Specifying Path depending on type of Animation Set
                setDir = animationSet is TransitionAnimationSet transitionAnimationSet
                    ? Path.Combine(setDir, transitionAnimationSet.ParentSet ?? Empty)
                    : Path.Combine(setDir, animationSet.SetName);

                // Adding Set Folder if missing
                if (!Directory.Exists(setDir)) Directory.CreateDirectory(setDir);

                // Copying of .hkx files
                foreach (var animation in animationSet.Animations)
                {
                    var newPath = Path.Combine(setDir, animation.AnimationName + ".hkx");

                    if (!File.Exists(newPath)) File.Copy(animation.OldPath, newPath);

                    animation.OldPath = newPath; // Updating Animation Location
                }
            }

            // Copying of Miscellaneous files e.g. .esp, .nif, etc.
            foreach (var misc in _animationDatabase.Misc)
            {
                var newPath = Path.GetFileName(misc).ToLowerInvariant().Equals("animobjects")
                    ? Path.Combine(_animationDatabase.SafePath, "meshes", Path.GetFileName(misc))
                    : Path.Combine(_animationDatabase.SafePath, Path.GetFileName(misc));

                if (Directory.Exists(misc))
                    DirectoryCopy(misc, newPath);
                else if (File.Exists(misc)) File.Copy(misc, newPath, true);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new(sourceDirName);
            if (!Directory.Exists(destDirName)) Directory.CreateDirectory(destDirName);

            foreach (var file in dir.GetFiles())
            {
                var tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            foreach (var subDir in dir.GetDirectories())
            {
                var tempPath = Path.Combine(destDirName, subDir.Name);
                DirectoryCopy(subDir.FullName, tempPath);
            }
        }

        // Method responsible for 0SA .xml Files
        private void WriteXml(AnimationSet animationSet)
        {
            if (!animationSet.ChangedThisSession || animationSet.Is0SexAnimation ||
                animationSet.AnimationClass is null || animationSet.PositionKey is null) return;

            var moduleKey = animationSet.Module.Name;
            var xmlPath = Path.Combine(_safePath, @"meshes\0SA\mod\0Sex\scene");
            {
                var setName = animationSet.SetName;
                var animationClass = animationSet.AnimationClass;
                var positionKey = animationSet.PositionKey;
                var actors = animationSet.Actors;
                var setSize = animationSet.Animations.Count / actors;
                var xmlDir = Path.Combine(xmlPath, moduleKey, positionKey.Replace("!", ""), animationClass);
                var animId = $"0Sx{moduleKey}_{animationClass}-{setName}";


                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = true
                };

                if (!Directory.Exists(xmlDir)) Directory.CreateDirectory(xmlDir);
                var writer = XmlWriter.Create(Path.Combine(xmlDir, $"{setName}.xml"), settings);

                writer.WriteStartElement("scene");
                writer.WriteAttributeString("id", $"{moduleKey}|{positionKey}|{animationClass}|{setName}");
                writer.WriteAttributeString("actors", actors.ToString());
                writer.WriteAttributeString("style", "OScene");

                writer.WriteStartElement("info");
                writer.WriteAttributeString("name", animationSet.Description);
                writer.WriteAttributeString("animator", animationSet.Animator);
                writer.WriteEndElement();

                writer.WriteStartElement("anim");
                writer.WriteAttributeString("id", animId);
                writer.WriteAttributeString("t", "L");
                writer.WriteAttributeString("l", "2");

                switch (animationSet)
                {
                    case TransitionAnimationSet transitionAnimationSet:
                        writer.WriteAttributeString("dest", transitionAnimationSet.Destination?.SceneId);

                        writer.WriteStartElement("dfx");
                        writer.WriteAttributeString("a", "0");
                        writer.WriteAttributeString("fx", "1");
                        writer.WriteEndElement();

                        writer.WriteStartElement("dfx");
                        writer.WriteAttributeString("a", "1");
                        writer.WriteAttributeString("fx", "1");
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                        break;
                    case HubAnimationSet hubAnimationSet:
                    {
                        writer.WriteEndElement();

                        writer.WriteStartElement("speed");
                        writer.WriteAttributeString("a", "0");
                        writer.WriteAttributeString("min", "0");
                        writer.WriteAttributeString("max", setSize.ToString());
                        writer.WriteAttributeString("name", "thrusts");

                        for (var i = 0; i <= setSize; i++)
                            if (i is 0)
                            {
                                writer.WriteStartElement("sp");
                                writer.WriteAttributeString("mtx", "^idle");
                                writer.WriteAttributeString("tar", "1");
                                writer.WriteAttributeString("type", "");

                                writer.WriteStartElement("anim");
                                writer.WriteAttributeString("id", $"{animId}_S{i}");
                                writer.WriteAttributeString("t", "L");
                                writer.WriteAttributeString("l", "2");
                                writer.WriteAttributeString("i0", $"{i}");
                                writer.WriteAttributeString("i1", $"{i}");
                                writer.WriteEndElement();

                                writer.WriteEndElement();
                            }
                            else
                            {
                                var speed = i switch
                                {
                                    1 => ".5",
                                    2 => "1",
                                    3 => "1.5",
                                    4 => "2.5",
                                    _ => Empty
                                };
                                writer.WriteStartElement("sp");
                                writer.WriteAttributeString("mtx", "^thrustsPerSec");
                                writer.WriteAttributeString("qnt", speed);

                                writer.WriteStartElement("anim");
                                writer.WriteAttributeString("id", $"{animId}_S{i}");
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

                        writer.WriteEndElement();


                        writer.WriteStartElement("nav");

                        foreach (var tab in hubAnimationSet.NavTabs)
                        {
                            writer.WriteStartElement("tab");
                            writer.WriteAttributeString("actor", ((int)tab.Actor).ToString());
                            writer.WriteAttributeString("icon", tab.Icon.ToString().ToLowerInvariant());
                            writer.WriteAttributeString("text", "$name");

                            writer.WriteStartElement("hue");
                            writer.WriteAttributeString("n", "hu");
                            writer.WriteAttributeString("cmd", "rg");
                            writer.WriteAttributeString("a", ((int)tab.TextColor).ToString());
                            writer.WriteEndElement();

                            writer.WriteStartElement("bnhue");
                            writer.WriteAttributeString("cmd", "rn");
                            writer.WriteAttributeString("a", ((int)tab.IconColor).ToString());
                            writer.WriteEndElement();

                            foreach (var page in tab.Pages)
                            {
                                writer.WriteStartElement("page");
                                writer.WriteAttributeString("icon", page.Icon.ToString().ToLowerInvariant());

                                writer.WriteStartElement("hue");
                                writer.WriteAttributeString("n", "hu");
                                writer.WriteAttributeString("cmd", "rg");
                                writer.WriteAttributeString("a", page.IconColor.ToString());
                                writer.WriteEndElement();

                                foreach (var option in page.Options)
                                {
                                    writer.WriteStartElement("option");
                                    writer.WriteAttributeString("halo", "hgentle");
                                    writer.WriteAttributeString("icon", "");
                                    writer.WriteAttributeString("go", option.Destination.SceneId);
                                    writer.WriteAttributeString("text", "test");

                                    writer.WriteStartElement("enhance");
                                    writer.WriteAttributeString("a", "1");
                                    writer.WriteEndElement();

                                    writer.WriteStartElement("hhue");
                                    writer.WriteAttributeString("n", "hu");
                                    writer.WriteAttributeString("cmd", "rg");
                                    writer.WriteAttributeString("a", "0");
                                    writer.WriteEndElement();

                                    writer.WriteStartElement("ihue");
                                    writer.WriteAttributeString("cmd", "body");
                                    writer.WriteAttributeString("a", "1");
                                    writer.WriteEndElement();

                                    writer.WriteStartElement("ihue");
                                    writer.WriteAttributeString("n", "gx");
                                    writer.WriteAttributeString("cmd", "rg");
                                    writer.WriteAttributeString("a", "1");
                                    writer.WriteEndElement();

                                    writer.WriteEndElement();
                                }

                                writer.WriteEndElement();
                            }

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();

                        break;
                    }
                }

                writer.WriteEndElement();
                writer.Close();
            }
        }

        // Method responsible for creating FNIS files
        private void WriteFnis(AnimationSet animationSet)
        {
            if (animationSet.AnimationClass is null || animationSet.PositionKey is null) return;

            var moduleKey = animationSet.Module.Name;
            var fnisDir = Path.Combine(_safePath, @$"meshes\actors\character\animations\0Sex_{moduleKey}_A");

            if (!Directory.Exists(fnisDir)) Directory.CreateDirectory(fnisDir);

            foreach (var animation in animationSet.Animations)
            {
                var animationName = animation.AnimationName;
                var setFolder = animationSet is TransitionAnimationSet transitionAnimationSet
                    ? transitionAnimationSet.ParentSet
                    : animationSet.SetName;

                var contents =
                    @$"b -Tn{animation.FnisArgs[0]} {animationName} ..\..\..\..\{Path.Combine(@"0SA\mod\0Sex\anim\", moduleKey, animationSet.PositionKey.Replace("!", ""), animationSet.AnimationClass, setFolder, animationName + ".hkx")} {animation.FnisArgs[1]} {Environment.NewLine}";

                File.AppendAllText(
                    Path.Combine(fnisDir, $"FNIS_0Sex_{moduleKey}_A{animation.Creature}_List.txt"), contents);
            }
        }

        private void DatabaseFileScriber()
        {
            var databaseName = _animationDatabase.Name;
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            var writer = XmlWriter.Create(Path.Combine(_animationDatabase.SafePath, $"{databaseName}.xml"), settings);

            writer.WriteStartElement("Database");
            writer.WriteAttributeString("Name", databaseName);

            foreach (var module in AnimationDatabase.Instance.Modules)
            {
                writer.WriteStartElement("Module");
                writer.WriteAttributeString("Name", module.Name);

                foreach (var animationSet in module.AnimationSets)
                    switch (animationSet)
                    {
                        case HubAnimationSet hubAnimationSet:
                            writer.WriteStartElement("Hub");
                            writer.WriteAttributeString("SceneID", animationSet.SceneId);
                            writer.WriteAttributeString("Animator", animationSet.Animator);
                            writer.WriteAttributeString("Description", animationSet.Description);

                            foreach (var destination in hubAnimationSet.NavTabs)
                                writer.WriteElementString("Destination", "");

                            foreach (var animation in animationSet.Animations)
                            {
                                writer.WriteStartElement("Animation");
                                writer.WriteAttributeString("Name", animation.AnimationName);
                                writer.WriteAttributeString("FnisArguments", Join(",", animation.FnisArgs));
                                writer.WriteAttributeString("Creature", animation.Creature);
                                writer.WriteEndElement();
                            }

                            writer.WriteEndElement();
                            break;

                        case TransitionAnimationSet transitionAnimationSet:
                            writer.WriteStartElement("Transition");
                            writer.WriteAttributeString("SceneID", animationSet.SceneId);
                            writer.WriteAttributeString("Animator", animationSet.Animator);
                            writer.WriteAttributeString("Description", animationSet.Description);
                            writer.WriteAttributeString("Destination", transitionAnimationSet.Destination?.SceneId);

                            foreach (var animation in animationSet.Animations)
                            {
                                writer.WriteStartElement("Animation");
                                writer.WriteAttributeString("Name", animation.AnimationName);
                                writer.WriteAttributeString("FnisArguments", Join(",", animation.FnisArgs));
                                writer.WriteAttributeString("Creature", animation.Creature);
                                writer.WriteEndElement();
                            }

                            writer.WriteEndElement();
                            break;
                    }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();
        }

        public void Write()
        {
            foreach (var module in _animationDatabase.Modules)
            foreach (var animationSet in module.AnimationSets)
            {
                WriteXml(animationSet);
                WriteFnis(animationSet);
            }

            DatabaseFileScriber();
        }
    }
}
