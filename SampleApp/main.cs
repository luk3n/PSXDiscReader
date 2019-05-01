using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using PSXDiscReader;

namespace SampleApp
{
    public partial class main : Form
    {
        PSXDisc disc;
        string _selectedDrive;

        public main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmb_drives.DataSource = GetDrives();
        }

        private List<DriveInfo> GetDrives()
        {
            return DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.CDRom).ToList();
        }

        private void cmb_drives_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedDrive = ((DriveInfo)cmb_drives.SelectedValue).ToString().Trim('\\');

            new Thread(new ThreadStart(() =>
            {
                btn_extract.Invoke(b => b.Enabled = false);
                treeViewInfo.Invoke(t => t.Nodes.Clear());
                statusLabel.Text = "Reading...";

                try
                {
                    disc = new PSXDisc(_selectedDrive);

                    Dictionary<string, string> systemInfo = new Dictionary<string, string>()
                    {
                        { "Licence",   disc.System.Licence },
                        { "Region",    disc.System.Region.ToString() },
                    };

                    Dictionary<string, string> volumeInfo = new Dictionary<string, string>()
                    {
                        { "System",                 disc.Volume.System },
                        { "Volume Identifier",      disc.Volume.VolumeIdentifier },
                        { "Identifier",             disc.Volume.Identifier },
                        { "CD-XA Signature",        disc.Volume.CDXA_Signature },
                        { "Publisher",              disc.Volume.Publisher },
                        { "Preparer",               disc.Volume.Preparer },
                        { "Volume Creation",        disc.Volume.VolumeCreation.ToString() },
                        { "Volume Modification",    disc.Volume.VolumeModification.ToString() },
                        { "Volume Effective",       disc.Volume.VolumeEffective.ToString() },
                        { "Volume Expiration",      disc.Volume.VolumeExpiration.ToString() },
                        { "Copyright Filename",     disc.Volume.CopyrightFilename },
                        { "Bibliographic Filename", disc.Volume.BibliographicFilename },
                        { "Abstract Filename",      disc.Volume.AbstractFilename },
                        { "Type",                   disc.Volume.Type.ToString() },
                        { "Version",                disc.Volume.Version.ToString() },
                        { "Blocks",                 disc.Volume.Size.ToString() }
                    };

                    treeViewInfo.Invoke(t => t.Nodes.Add("System Information"));

                    foreach (var item in systemInfo)
                    {
                        treeViewInfo.Invoke(t => t.Nodes[0].Nodes.Add(string.Format("{0}: {1}", item.Key, item.Value)));
                    }

                    treeViewInfo.Invoke(t => t.Nodes.Add("Volume Descriptor Information"));

                    foreach (var item in volumeInfo)
                    {
                        treeViewInfo.Invoke(t => t.Nodes[1].Nodes.Add(string.Format("{0}: {1}", item.Key, item.Value)));
                    }

                    treeViewInfo.Invoke(t => t.ExpandAll());
                    btn_extract.Invoke(b => b.Enabled = true);
                    statusLabel.Text = "Done";
                }
                catch (Exception ex)
                {
                    statusLabel.Text = ex.Message;
                }
                
            })).Start();
        }

        private void btn_extract_Click(object sender, EventArgs e)
        {
            SaveFileDialog extractDialog = new SaveFileDialog()
            {
                Filter = "TMD files (*.tmd)|*.tmd|All files (*.*)|*.*"
            };

            if (extractDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(extractDialog.FileName, disc.System.Logo);
                    MessageBox.Show("PSX logo successfully saved!", "PSXDiscReader", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error ocurred when saving logo. {0}", ex.Message), "PSXDiscReader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
 