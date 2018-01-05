using System.Windows;
using System.Windows.Controls;
using Dynamo.Extensions;
using Dynamo.Wpf.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.ViewModels;
using Dynamo.Graph.Workspaces;
using Dynamo.Graph;

namespace SampleViewExtension
{
    /// <summary>
    /// Interaction logic for SampleWindow.xaml
    /// </summary>
    public partial class SampleWindow : Window
    {

        public SampleWindow()
        {
            InitializeComponent();
        }

        private void selectRow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    var dgrData = dgr.DataContext;
                    ModelBase dgrNode= dgrData.GetType().GetProperty("theNode").GetValue(dgrData) as ModelBase;
                    ViewLoadedParams dgrWS = dgrData.GetType().GetProperty("theWSModel").GetValue(dgrData) as ViewLoadedParams;
                    string dgrGuid = dgrData.GetType().GetProperty("guid").GetValue(dgrData) as string;
                    foreach(NodeModel node in dgrWS.CurrentWorkspaceModel.Nodes)
                    {
                        node.Deselect();
                        node.IsSelected = false;
                    }
                    var VM = dgrWS.DynamoWindow.DataContext as DynamoViewModel;
                    VM.CurrentSpaceViewModel.ResetFitViewToggleCommand.Execute(null);
                    VM.AddToSelectionCommand.Execute(dgrNode);
                    VM.FitViewCommand.Execute(null);
                }
            }
        }
    }
}
