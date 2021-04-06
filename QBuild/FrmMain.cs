using QBuild.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QBuild
{
    public partial class FrmMain : Form
    {
        TreeNode currentParentNode;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void btnPopulateTree_Click(object sender, EventArgs e)
        {
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;

            PopulateMainTree();

            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// This is the entry point for populating the treeview
        /// </summary>
        private void PopulateMainTree()
        {
            DataTable dt = DataAccess.GetTreeData();

            // Use a DataSet to manage the data
            DataSet ds = new DataSet();

            // Add DataTable to the DataSet
            ds.Tables.Add(dt);

            // Add a relationship
            ds.Relations.Add("TreeParentChild", ds.Tables[0].Columns["Name"],
                ds.Tables[0].Columns["Parent"], false);

            // Iterate all the rows in the DataSet
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["Parent"].ToString() == "")
                {
                    // Creates a TreeNode if the parent equals 0
                    TreeNode root = new TreeNode(dr["Name"].ToString());
                    treeView1.Nodes.Add(root);
                    // Recursively builds the tree
                    PopulateTree(dr, root);
                }
            }
            // Expands all the tree nodes
            treeView1.ExpandAll();

            treeView1.Nodes[0].EnsureVisible();
            treeView1.SelectedNode = treeView1.Nodes[0];    // Set top node as selected node
            treeView1.Focus();                      // highlight the current node
            treeView1.HideSelection = false;        // Ensure the node remains selected even when the treeview does not have focus

            // Disable button once the treeview is fully populated
            btnPopulateTree.Enabled = false;
        }

        /// <summary>
        /// Utility function for populating the treeView recursively. 
        /// </summary>
        private void PopulateTree(DataRow dr, TreeNode pNode)
        {
            // To iterate through all the rows in the DataSet
            foreach (DataRow row in dr.GetChildRows("TreeParentChild"))
            {
                // Creating a TreeNode for each row
                TreeNode cChild = new TreeNode(row["Name"].ToString());
                // Add cChild node to the pNode
                pNode.Nodes.Add(cChild);
                // Recursively build the tree
                PopulateTree(row, cChild);
            }
        }

        /// <summary>
        /// Exit application upon clicking on Exit Application button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This event handler is triggered after the tree node is selected.
        /// Display the selected node
        /// Display the text of the selected node's Parent
        /// Call the utility function to populate the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                // Display the selected node
                lblCurrent.Text = "Current Part: " + e.Node.Text;

                lblParentChild.Text = "Parent  Child Part: ";

                // Display the text of the selected tree node's Parent
                if (e.Node.Parent != null && e.Node.Parent.GetType() == typeof(TreeNode))
                {
                    lblParentChild.Text += e.Node.Parent.Text + "\\" + e.Node.Text;
                }
                
                // Check if selected node is a parent, i.e. has at least a child
                // If so, populate the datagrid
                if (e.Node.Nodes.Count > 0)
                {
                    currentParentNode = e.Node;
                    PopulateDataGridView(currentParentNode.Text);
                }
                else    // Arrive here if it's a child
                {
                    if (e.Node.Parent != null && e.Node.Parent != currentParentNode)
                    {
                        currentParentNode = e.Node.Parent;
                        PopulateDataGridView(currentParentNode.Text);
                    }
                }
            }
        }

        /// <summary>
        /// Utility function to populate the datagrid
        /// </summary>
        /// <param name="parent"></param>
        private void PopulateDataGridView(string parentName)
        {
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;

            DataTable dt = DataAccess.GetGridData(parentName);
            dataGridView1.DataSource = dt.DefaultView;

            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }

    }
}
