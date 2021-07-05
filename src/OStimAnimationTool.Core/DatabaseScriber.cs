using System;
using System.IO;
using System.Text;
using System.Xml;
using OStimAnimationTool.Core.Models;

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
            foreach (var animationSet in _animationDatabase.AnimationSets)
            {
                // Base Path for all Animation Sets
                string setDir = Path.Combine(_animationDatabase.SafePath, @"meshes\0SA\mod\0Sex\anim\",
                    animationSet.ModuleName, animationSet.PositionKey.Replace("!", ""), animationSet.AnimationClass);

                // Specifying Path depending on type of Animation Set
                setDir = animationSet is TransitionAnimationSet transitionAnimationSet
                    ? Path.Combine(setDir, transitionAnimationSet.ParentSet)
                    : Path.Combine(setDir, animationSet.SetName);

                // Adding Set Folder if missing
                if (!Directory.Exists(setDir))
                    Directory.CreateDirectory(setDir);

                foreach (var animation in animationSet.Animations)
                {
                    var newPath = Path.Combine(setDir, animation.AnimationName + ".hkx");
                    if (!File.Exists(newPath))
                        File.Copy(animation.OldPath, Path.Combine(setDir, animation.AnimationName + ".hkx"));
                    animation.OldPath = newPath;  // Updating Animation Location
                }
            }
        }

        // Method responsible for 0SA .xml Files
        public void XmlScriber()
        {
            var xmlPath = Path.Combine(_safePath, @"meshes\0SA\mod\0Sex\scene");
            foreach (var animationSet in _animationDatabase.AnimationSets)
            {
                var setName = animationSet.SetName;
                var moduleKey = animationSet.ModuleName;
                var animationClass = animationSet.AnimationClass;
                var positionKey = animationSet.PositionKey;
                var actors = animationSet.Actors;
                var setSize = animationSet.Animations.Count / actors;
                var xmlDir = Path.Combine(xmlPath, moduleKey, positionKey.Replace("!", ""), animationClass);
                var animId = $"0Sx{moduleKey}_{animationClass}-{setName}";


                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = Encoding.UTF8
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
                        writer.WriteAttributeString("dest", transitionAnimationSet.Destination.SceneID);

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
                                    _ => string.Empty
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

                        writer.WriteStartElement("tab");
                        writer.WriteAttributeString("actor", "0");
                        writer.WriteAttributeString("icon", "sdom");
                        writer.WriteAttributeString("text", "$name");

                        writer.WriteStartElement("hue");
                        writer.WriteAttributeString("n", "hu");
                        writer.WriteAttributeString("cmd", "rg");
                        writer.WriteAttributeString("a", "0");
                        writer.WriteEndElement();

                        writer.WriteStartElement("bnhue");
                        writer.WriteAttributeString("cmd", "rn");
                        writer.WriteAttributeString("a", "0");
                        writer.WriteEndElement();

                        writer.WriteStartElement("page");
                        writer.WriteAttributeString("icon", "mtri");

                        writer.WriteStartElement("hue");
                        writer.WriteAttributeString("n", "hu");
                        writer.WriteAttributeString("cmd", "rg");
                        writer.WriteAttributeString("a", "0");
                        writer.WriteEndElement();

                        foreach (var destination in hubAnimationSet.Destinations)
                        {
                            writer.WriteStartElement("option");
                            writer.WriteAttributeString("halo", "hgentle");
                            writer.WriteAttributeString("icon", "");
                            writer.WriteAttributeString("go", destination.SceneID);
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
                        writer.WriteEndElement();
                        writer.WriteEndElement();

                        break;
                    }
                }

                writer.WriteEndElement();
                writer.Close();
            }
        }

        public void FnisScriber()
        {
            foreach (var animationSet in _animationDatabase.AnimationSets)
            {
                var moduleKey = animationSet.ModuleName;
                var fnisDir = Path.Combine(_safePath, @$"meshes\actors\character\animations\0Sex_{moduleKey}_A");

                if (!Directory.Exists(fnisDir))
                    Directory.CreateDirectory(fnisDir);

                foreach (var animation in animationSet.Animations)
                {
                    var animationName = animation.AnimationName;
                    var contents =
                        @$"b -Tn {animationName} ..\..\..\..\{Path.Combine(@"0SA\mod\0Sex\anim\", moduleKey, animationSet.AnimationClass, animationSet.PositionKey.Replace("!", ""), animationName + ".hkx")} {Environment.NewLine}";
                    File.AppendAllText(Path.Combine(fnisDir, $"FNIS_0Sex_{moduleKey}_A_List.txt"), contents);
                }
            }
        }

        //TODO Extend Databasescriber
        public void DatabaseFileScriber()
        {
            var databaseName = _animationDatabase.Name;
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            var writer = XmlWriter.Create(Path.Combine(_animationDatabase.SafePath, $"{databaseName}.xml"), settings);

            writer.WriteStartElement("database");
            writer.WriteAttributeString("name", databaseName);

            foreach (var animationSet in _animationDatabase.AnimationSets)
            {
                writer.WriteStartElement("animationset");
                writer.WriteAttributeString("name", animationSet.SetName);
                writer.WriteAttributeString("class", animationSet.AnimationClass);
                writer.WriteAttributeString("animator", animationSet.Animator);
                writer.WriteAttributeString("description", animationSet.Description);

                foreach (var animation in animationSet.Animations)
                {
                    writer.WriteStartElement("animation");
                    writer.WriteAttributeString("name", animation.AnimationName);
                    writer.WriteAttributeString("actor", animation.Actor.ToString());
                    writer.WriteAttributeString("speed", animation.Speed.ToString());
                    writer.WriteAttributeString("oldPath", animation.OldPath);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Close();
        }
    }
}
