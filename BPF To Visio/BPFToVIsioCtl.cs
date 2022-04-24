using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace LinkeD365.BPFToVisio
{
    public partial class BPFToVisioCtl : PluginControlBase
    {
        private Settings mySettings;

        public BPFToVisioCtl()
        {
            InitializeComponent();
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbSample_Click(object sender, EventArgs e)
        {
            // The ExecuteMethod method handles connecting to an
            // organization if XrmToolBox is not yet connected
            ExecuteMethod(GetAccounts);
        }

        private void GetAccounts()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Getting accounts",
                Work = (worker, args) =>
                {
                    args.Result = Service.RetrieveMultiple(new QueryExpression("account")
                    {
                        TopCount = 50
                    });
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as EntityCollection;
                    if (result != null)
                    {
                        MessageBox.Show($"Found {result.Entities.Count} accounts");
                    }
                }
            });
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }

            ExecuteMethod(LoadWFs);
        }

        private void LoadWFs()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Business Process Flows",
                Work = (w, e) =>
                {
                    var fetchXml = $@"
<fetch xmlns:generator='MarkMpn.SQL4CDS'>
  <entity name='workflow'>
    <attribute name='name' />
    <attribute name='businessprocesstype' />
    <attribute name='clientdata' />
    <attribute name='workflowid' />
    <filter>
      <condition attribute='category' operator='eq' value='4' />
      <condition attribute='businessprocesstype' operator='eq' value='0' />
    </filter>
  </entity>
</fetch>";
                    var qe = new FetchExpression(fetchXml);


                    var wfRecords = Service.RetrieveMultiple(qe);

                    e.Result = wfRecords.Entities.Select(ent => new WorkFlow() { Id = ent["workflowid"].ToString(), Name = ent["name"].ToString(), Schema = ent["clientdata"].ToString() }).ToList();

                },
                ProgressChanged = e =>
                {
                },
                PostWorkCallBack = e =>
                {
                    var returnWFs = e.Result as List<WorkFlow>;
                    if (returnWFs.Any())
                    {
                        //bots = returnBots;
                        gvBPFs.DataSource = returnWFs;

                    }
                    ConfigGrid();
                },
            });
        }

        protected void ConfigGrid()
        {
            gvBPFs.AutoResizeColumns();
            gvBPFs.Columns["Name"].SortMode = DataGridViewColumnSortMode.Automatic;

        }
        private void BPFToVisioCtl_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs args)
        {

        }

        private void btnCreateVisio_Click(object sender, EventArgs e)
        {
            var selectedWFs = new List<WorkFlow>();

            if (gvBPFs.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one BPF to document", "Select BPF(s)", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int bpfCount = 1;
            Utils.ActionCount = 0;
            SaveFileDialog saveDialog;
            if (gvBPFs.SelectedRows.Count == 1)
            {
                var selectedWF = (WorkFlow)gvBPFs.SelectedRows[0].DataBoundItem;
                saveDialog = GetSaveDialog(selectedWF.Name);
                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Utils.CreateVisio(selectedWF, saveDialog.FileName, 1);
                Utils.CompleteVisio(saveDialog.FileName);
            }
            else
            {
                saveDialog = GetSaveDialog("My BPFs.vsdx");
                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                foreach (DataGridViewRow row in gvBPFs.SelectedRows)
                {
                    var selectedTopic = (WorkFlow)row.DataBoundItem;
                    Utils.CreateVisio(selectedTopic, saveDialog.FileName, bpfCount);
                    bpfCount++;
                }
                Utils.CompleteVisio(saveDialog.FileName);

            }

            Utils.Ai.WriteEvent("BPFs Created", bpfCount);
            Utils.Ai.WriteEvent("Shapes Created", Utils.ActionCount);

            if (MessageBox.Show($@"{bpfCount} BPF{(bpfCount > 1 ? "s have" : " has")} been created with {Utils.ActionCount} steps.
Do you want to open the Visio File?", "Visio created successfully", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start(saveDialog.FileName);
            }
        }

        private SaveFileDialog GetSaveDialog(string fileName)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Visio Files(*.vsdx) | *.vsdx";
            saveFileDialog.DefaultExt = "vsdx";

            saveFileDialog.FileName = fileName;
            return saveFileDialog;
        }
    }
}