using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DevTools
{
    public partial class DevToolForm : Form
    {
        FileManager _fm;
        public DevToolForm()
        {
            InitializeComponent();
            _fm = new FileManager();
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            var dropped = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();

            if (!dropped.Any())
                return;

            foreach(string title in dropped)
            {
                if (File.Exists(title) && !_fm.Contains(title))
                {
                    AddNewFile(title, true);
                }
                else if (Directory.Exists(title))
                {
                    var dinfo = new DirectoryInfo(title);
                    var node = treeView1.Nodes.Add(dinfo.Name);
                    node.Nodes.Add("FullPath", dinfo.FullName);
                    node.Nodes.Add("LastEdit", dinfo.LastWriteTime.ToLongTimeString());
                    node.Nodes.Add("Type", "Directory");
                    foreach(var file in dinfo.GetFiles())
                    {
                        AddNewFile(file.FullName, false, node);
                    }
                }
            }
            treeView1.Update();
        }
        
        private void AddNewFile(string fileName, bool addToRoot, TreeNode root = null)
        {
            FileInfo file = new FileInfo(fileName);
            _fm.Add(file.FullName, file);
            var node = addToRoot ? treeView1.Nodes.Add(file.Name) : root.Nodes.Add(file.Name);
            
            node.Nodes.Add("FullPath", file.FullName);
            node.Nodes.Add("LastEdit", file.LastWriteTime.ToLongTimeString());
            node.Nodes.Add("Type", "File");
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void treeView1_DoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Nodes["Type"].Text == "File")
            {
                FileEntry fe = _fm.Get(e.Node.Nodes["FullPath"].Text);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = fe.fi.DirectoryName;
                sfd.FileName = fe.fi.Name;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    fe.SaveFileName = Path.Combine(sfd.InitialDirectory, sfd.FileName);
                }
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                var dinfo = new DirectoryInfo(e.Node.Nodes["FullPath"].Text);
                fbd.SelectedPath = dinfo.FullName;
                if(fbd.ShowDialog() == DialogResult.OK)
                {
                    foreach(TreeNode node in e.Node.Nodes)
                    {
                        //Must be a file, recursion not supported yet
                        if(node.Nodes.Count > 0)
                        {
                            var fe = _fm.Get(node.Nodes["FullPath"].Text);
                            fe.SaveFileName = Path.Combine(fbd.SelectedPath,
                                fe.fi.Name);
                        }
                    }
                }
            }
        }
    }
}
