#region

using System;
using System.IO;
using System.Text;
using System.Xml;
using AnimationDatabaseExplorer.Models;
using OStimAnimationTool.Core.Models;

#endregion

namespace OStimAnimationTool.Core
{
    public class DatabaseScriber
    {
        private readonly int _actorCount;
        private readonly string _animationClass;
        private readonly AnimationSet _animationSet;
        private readonly string _animationSetDescription;
        private readonly string _animator;
        private readonly bool _isTransition;
        private readonly string _moduleKey;
        private readonly string _positionKeys = "Cy6!DDy6";
        private readonly string _safePath;
        private readonly string _setName;
        private readonly string _transitionDestination;

        public DatabaseScriber(AnimationDatabase animationDatabase, AnimationSet animationSet)
        {
            _safePath = animationDatabase.SafePath;
            _animationSet = animationSet;
            _moduleKey = animationDatabase.ModuleKey;
            _animationClass = animationSet.AnimationClass;
            _setName = animationSet.SetName;
            _actorCount = animationSet.ActorCount;
            _animationSetDescription = animationSet.Description;
            _animator = animationSet.Animator;
            _isTransition = animationSet.IsTransition;
            _transitionDestination = animationSet.TransitionDestination;
        }

        public void XmlScriber()
        {
            var xmlDir = Path.Combine(_safePath, @"meshes\0SA\mod\0Sex\scene", _moduleKey, _animationClass);
            var animId = $"0Sx{_moduleKey}_{_animationClass}-{_setName}";
            var setSize = _animationSet.Count / _actorCount;

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            if (!Directory.Exists(xmlDir)) Directory.CreateDirectory(xmlDir);
            var writer = XmlWriter.Create(Path.Combine(xmlDir, $"{_setName}.xml"), settings);

            writer.WriteStartElement("scene");
            writer.WriteAttributeString("id", $"{_moduleKey}|{_positionKeys}|{_animationClass}|{_setName}");
            writer.WriteAttributeString("actors", _actorCount.ToString());
            writer.WriteAttributeString("style", "OScene");
            writer.WriteStartElement("info");
            writer.WriteAttributeString("name", _animationSetDescription);
            writer.WriteAttributeString("animator", _animator);
            writer.WriteEndElement();
            writer.WriteStartElement("anim");
            writer.WriteAttributeString("id", animId);
            writer.WriteAttributeString("t", "L");
            writer.WriteAttributeString("l", "2");

            if (_isTransition)
            {
                writer.WriteAttributeString("dest", _transitionDestination);
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
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Close();
        }

        public void FnisScriber()
        {
            var fnisDir = Path.Combine(_safePath, @$"meshes\actors\character\animations\0Sex_{_moduleKey}_A");
            if (!Directory.Exists(fnisDir))
                Directory.CreateDirectory(fnisDir);

            foreach (var animation in _animationSet)
            {
                var animationName = animation.AnimationName;
                var contents =
                    @$"b -Tn {animationName} ..\..\..\..\{Path.Combine(@"0SA\mod\0Sex\anim\", _moduleKey, _animationClass, animationName + ".hkx")} {Environment.NewLine}";
                File.AppendAllText(Path.Combine(fnisDir, $"FNIS_0Sex_{_moduleKey}_A_List.txt"), contents);
            }
        }
    }
}
