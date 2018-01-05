using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Selection;
using Dynamo.Wpf.Extensions;
using Dynamo.Wpf.ViewModels;
using Dynamo.UI.Commands;
using Dynamo.ViewModels;
using Dynamo.Models;
using System.Windows;
using System.Windows.Media;
using Dynamo.Controls;
using Dynamo.Graph;

namespace SampleViewExtension
{
    public class SampleWindowViewModel : NotificationObject, IDisposable
    {
        private string selectedNodesText = "Begin selecting ";
        private ViewLoadedParams readyParams;

        public string SelectedNodesText => $"There are {readyParams.CurrentWorkspaceModel.Nodes.ToList().Count()} nodes in the workspace.";
        public List<nodeData> allNodes => getAllNodes();
        public List<string> CurrentSelection => guidOfCurrent();

        public class nodeData
        {
            public string nickname { get; set; }
            public string creationname { get; set; }
            public string guid { get; set; }
            public ModelBase theNode { get; set; }
            public ViewLoadedParams theWSModel { get; set; }
        }

        public List<nodeData> getAllNodes()
        {
            List<nodeData> tempNodes = new List<nodeData>();
            foreach (NodeModel node in readyParams.CurrentWorkspaceModel.Nodes)
            {
                tempNodes.Add(new nodeData()
                {
                    nickname = node.NickName,
                    creationname = node.CreationName,
                    guid = node.AstIdentifierGuid,
                    theNode = node,
                    theWSModel = readyParams
                });
            }
            return tempNodes;
        }

        public List<string> guidOfCurrent()
        {
            List<string>guids = new List<string>();
            foreach(NodeModel node in readyParams.CurrentWorkspaceModel.CurrentSelection)
            {
                guids.Add(node.AstIdentifierGuid);
            }
            return guids;
        }
        public SampleWindowViewModel(ReadyParams p)
        {
            readyParams = p as ViewLoadedParams;
            WorkspaceModel checkSelection = p.CurrentWorkspaceModel as WorkspaceModel;
            p.CurrentWorkspaceModel.NodeAdded += CurrentWorkspaceModel_NodesChanged;
            p.CurrentWorkspaceModel.NodeRemoved += CurrentWorkspaceModel_NodesChanged;
            readyParams.SelectionCollectionChanged += ReadyParams_SelectionCollectionChanged;
            foreach (NodeModel node in readyParams.CurrentWorkspaceModel.Nodes)
            {
                node.PropertyChanged += Node_PropertyChanged; 
                //node.Modified += CurrentWorkspaceModel_NodesChanged;
                //node.UpdateASTCollection += CurrentWorkspaceModel_NodesChanged;
            }
            //checkSelection.PropertyChanged += CurrentWorkspaceModel_PropertyUpdate;
            //var children  = FindVisualChildren<DynamoView>(readyParams.DynamoWindow);
            //var VM = children.First().DataContext as DynamoViewModel;
            //var VM = readyParams.DynamoWindow.DataContext as DynamoViewModel;
            //VM.FitViewCommand.Execute(null);
            
        }

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventArgs eventArgs = e as PropertyChangedEventArgs;
            string changedProperty = eventArgs.PropertyName;

            if (changedProperty == "NickName")
            {
                RaisePropertyChanged("allNodes");
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void ReadyParams_SelectionCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs obj)
        {
            RaisePropertyChanged("CurrentSelection");
        }

        /*private void CurrentWorkspaceModel_PropertyUpdate(object s, EventArgs e)
        {
            PropertyChangedEventArgs eventArgs = e as PropertyChangedEventArgs;
            string changedProperty = eventArgs.PropertyName;

            if (changedProperty == "NickName")
            {
                RaisePropertyChanged("allNodes");
            }
            
        }*/

        private void CurrentWorkspaceModel_NodesChanged(NodeModel obj)
        {
            RaisePropertyChanged("SelectedNodesText");
            RaisePropertyChanged("allNodes");
        }

        public void Dispose()
        {
            WorkspaceModel checkSelection = readyParams.CurrentWorkspaceModel as WorkspaceModel;
            readyParams.CurrentWorkspaceModel.NodeAdded -= CurrentWorkspaceModel_NodesChanged;
            readyParams.CurrentWorkspaceModel.NodeRemoved -= CurrentWorkspaceModel_NodesChanged;
            //checkSelection.PropertyChanged -= CurrentWorkspaceModel_PropertyUpdate;
            foreach (NodeModel node in readyParams.CurrentWorkspaceModel.Nodes)
            {
                node.Modified -= CurrentWorkspaceModel_NodesChanged;
                node.UpdateASTCollection -= CurrentWorkspaceModel_NodesChanged;
            }
        }
    }
}
