#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using DynamicData;
using NodeNetwork.Toolkit.Layout.ForceDirected;
using NodeNetwork.ViewModels;
using OStimAnimationTool.Core;
using Prism.Commands;
using Prism.Regions;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class NavNodeViewModel : ViewModelBase
    {
        private NetworkViewModel _network = new();

        public NavNodeViewModel()
        {
            LayoutCommand = new DelegateCommand(Layouter);
        }

        public DelegateCommand LayoutCommand { get; }

        public NetworkViewModel Network
        {
            get => _network;
            set => SetProperty(ref _network, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavFinder(
                @"C:\Users\Admin\Downloads\OSex-17209-2-02SE-Alpha\OSex 2.02S Alpha\Data\meshes\0SA\mod\0Sex\scene\0MF");

            /* const string rootDir =
                 @"C:\Users\Admin\Downloads\OSex-17209-2-02SE-Alpha\OSex 2.02S Alpha\Data\meshes\0SA\mod\0Sex\scene";
             const string startPath =
                 @"C:\Users\Admin\Downloads\OSex-17209-2-02SE-Alpha\OSex 2.02S Alpha\Data\meshes\0SA\mod\0Sex\scene\0MF\_TOG\OM\AutoStartBasic.xml";
 
             XDocument doc = XDocument.Load(startPath);
             
             var startNode = new SetNode {Name = "Start"};
             var output = new NodeOutputViewModel();
             
             Network.Nodes.Add(startNode);
             startNode.Outputs.Add(output);
 
             IEnumerable<XAttribute?> destinations =
                 doc.XPathSelectElements("/scene/togs/tog0/dest").Attributes("id");
             destinations = destinations.Concat(new[]
                 {doc.XPathSelectElement("/scene/togs/tog0/tog1")?.Attribute("dest")});
             
             foreach (var destination in destinations)
             {
                 var scenePath = destination?.Value.Replace("|", @"\");
                 scenePath = scenePath?.Replace("!", "");
 
                 var input = new NodeInputViewModel();
                 var destNode = new SetNode {Name = scenePath};
                 Network.Nodes.Add(destNode);
                 destNode.Inputs.Add(input);
 
                 Network.Connections.Add(new ConnectionViewModel(Network, input, output));
                 RecursiveNavLocator(scenePath, destNode);
             }
 
             void RecursiveNavLocator(string? scenePath, SetNode destNode)
             {
                 try
                 {
                     var destOutput = new NodeOutputViewModel();
                     destNode.Outputs.Add(destOutput);
                     
                     XDocument docx = XDocument.Load(Path.Combine(rootDir, scenePath + ".xml"));
 
                     if ((from nav in docx.Elements("scene")
                         select nav.Element("nav")).SingleOrDefault() is not null)
                     {
                         IEnumerable<XAttribute> dest =
                             docx.XPathSelectElements("/scene/nav/tab/page/option").Attributes("go");
                         Parallel.ForEach(dest, destination =>
                         {
                             string? newScenePath;
 
                             if (destination.Value[0] == '^')
                             {
                                 newScenePath = scenePath + destination.Value[1..];
                             }
                             else
                             {
                                 newScenePath = destination.Value.Replace("|", @"\");
                                 newScenePath = newScenePath.Replace("!", "");
                             }
 
                             var input = new NodeInputViewModel();
                             destNode = DestinationNodeFinder(newScenePath);
                             destNode.Inputs.Add(input);
                             if (Network.Nodes.Items.Contains(destNode))
                             {
                                 Network.Connections.Add(new ConnectionViewModel(Network, input, destOutput));
                             }
                             else
                             {
                                 Network.Nodes.Add(destNode);destNode.Inputs.Add(input);
                                 Network.Connections.Add(new ConnectionViewModel(Network, input, destOutput));
                                 RecursiveNavLocator(newScenePath, destNode);
                             }
                         });
                     }
                     else if (docx.XPathSelectElement("/scene/anim")?.Attribute("dest") is not null)
                     {
                         var destination = docx.XPathSelectElement("/scene/anim")?.Attribute("dest");
                         string? newScenePath;
 
                         if (destination?.Value[0] == '^')
                         {
                             newScenePath = scenePath + destination.Value[1..];
                         }
                         else
                         {
                             newScenePath = destination?.Value.Replace("|", @"\");
                             newScenePath = newScenePath?.Replace("!", "");
                         }
 
                         var input = new NodeInputViewModel();
                         destNode = DestinationNodeFinder(newScenePath);
                         destNode.Inputs.Add(input);
                         if (Network.Nodes.Items.Contains(destNode))
                         {
                             Network.Connections.Add(new ConnectionViewModel(Network, input, destOutput));
                         }
                         else
                         {
                             Network.Nodes.Add(destNode);destNode.Inputs.Add(input);
                             Network.Connections.Add(new ConnectionViewModel(Network, input, destOutput));
                             RecursiveNavLocator(newScenePath, destNode);
                         }
                     }
                 }
                 catch (Exception e)
                 {
                     Console.WriteLine(e);
                 }
             }*/
        }

        private void NavFinder(string rootDir)
        {
            try
            {
                Parallel.ForEach(Directory.GetFiles(rootDir), f =>
                {
                    XDocument doc = XDocument.Load(f);
                    var sceneId = (from id in doc.Elements("scene")
                        select id.Attribute("id")).SingleOrDefault();
                    if (sceneId is null)
                        return;

                    var name = Regex.Match(sceneId.Value, @"[^\|]*$").ToString();
                    var node = new SetNode {Name = name};
                    if (!Network.Nodes.Items.Contains(node))
                        Network.Nodes.Add(node);

                    var output = new NodeOutputViewModel();
                    node.Outputs.Add(output);

                    if (name.Equals("AutoStartBasic"))
                    {
                        IEnumerable<XAttribute?> destinations =
                            doc.XPathSelectElements("/scene/togs/tog0/dest").Attributes("id");
                        destinations = destinations.Concat(new[]
                            {doc.XPathSelectElement("/scene/togs/tog0/tog1")?.Attribute("dest")});
                        foreach (var destination in destinations)
                        {
                            var destinationName = Regex.Match(destination!.Value, @"[^\|]*$").ToString();
                            var input = new NodeInputViewModel();
                            DestinationNodeFinder(destinationName).Inputs.Add(input);

                            Network.Connections.Add(new ConnectionViewModel(Network, input, output));
                        }
                    }
                    else if ((from nav in doc.Elements("scene")
                        select nav.Element("nav")).SingleOrDefault() is not null)
                    {
                        IEnumerable<XAttribute> destinations =
                            doc.XPathSelectElements("/scene/nav/tab/page/option").Attributes("go");
                        foreach (var destination in destinations)
                        {
                            var destinationName = destination.Value[0] == '^'
                                ? name + destination.Value[1..]
                                : Regex.Match(destination.Value, @"[^\|]*$").ToString();

                            var input = new NodeInputViewModel();
                            DestinationNodeFinder(destinationName).Inputs.Add(input);

                            Network.Connections.Add(new ConnectionViewModel(Network, input, output));
                        }
                    }
                    else if (doc.XPathSelectElement("/scene/anim")?.Attribute("dest") is not null)
                    {
                        var destination = doc.XPathSelectElement("/scene/anim")?.Attribute("dest");
                        var destinationName = destination!.Value[0] == '^'
                            ? name + destination.Value[1..]
                            : Regex.Match(destination.Value, @"[^\|]*$").ToString();

                        var input = new NodeInputViewModel();
                        DestinationNodeFinder(destinationName).Inputs.Add(input);

                        Network.Connections.Add(new ConnectionViewModel(Network, input, output));
                    }
                });

                Parallel.ForEach(Directory.GetDirectories(rootDir), NavFinder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private SetNode DestinationNodeFinder(string? destinationName)
        {
            foreach (var nodeViewModel in _network.Nodes.Items)
            {
                var destNode = (SetNode) nodeViewModel;
                if (destNode.Name.Equals(destinationName))
                    return destNode;
            }

            return new SetNode {Name = destinationName};
        }

        private void Layouter()
        {
            ForceDirectedLayouter layouter = new();
            layouter.Layout(new Configuration {Network = Network}, 1000);
        }
    }
}
