namespace ELE_543_App
{
    public partial class Form1 : Form
    {
        string localFilePath = "C:\\MattSFTP";
        SFTP_Client sFTP;
        DirectoryInfo localDirectory;
        TreeNode rootNode;
        string localPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create the local directory if it does not exist
            if (!Directory.Exists(localFilePath))
            {
                Directory.CreateDirectory(localFilePath);
            }

            // Load the current directory, and make one if it doesn't exist
            localDirectory = new DirectoryInfo(localFilePath);

            rootNode = new TreeNode();
            rootNode.Text = localDirectory.FullName;
            localFileTree.Nodes.Add(rootNode);

            foreach (DirectoryInfo dir in localDirectory.GetDirectories())
            {
                TreeNode firstNode = new TreeNode(dir.Name);
                rootNode.Nodes.Add(firstNode);
                AddRecursiveDirectory(dir, firstNode);
            }

            foreach (FileInfo file in localDirectory.GetFiles())
            {
                localFileTree.Nodes.Add(new TreeNode(file.Name + " ... " + file.Length / 1024f + "KiB"));
            }

            statusBox.Text += "Program loaded successfully.\n";
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            string username = userNameInput.Text;
            string password = passwordInput.Text;
            string host = hostInput.Text;

            if (username == "")
            {
                statusBox.Text += "No username provided. Please enter one and try again.\n";
                return;
            }
            if (password == "")
            {
                statusBox.Text += "No password provided. Please enter one and try again.\n";
                return;
            }
            if (host == "")
            {
                statusBox.Text += "No host address provided. Please enter one and try again.\n";
                return;
            }

            try
            {
                sFTP = new SFTP_Client(username, password, host);
            }
            catch (Exception ex)
            {
                statusBox.Text += "Error creating SFTP client: " + ex.Message + "\n";
                sFTP.Disconnect();
                return; // Ensure we exit if the client creation fails
            }

            try
            {
                // Get the root node from the SFTP client
                TreeNode? remoteRootNode = sFTP.GetTopLevelDirectory();

                // Check for null before adding to the tree
                if (remoteRootNode != null)
                {
                    remoteFileTree.Nodes.Add(remoteRootNode);
                    remoteRootNode.Expand();
                }
                else
                {
                    statusBox.Text += "Remote directory is empty or could not be retrieved.\n";
                }
            }
            catch (Exception ex)
            {
                statusBox.Text += "Error populating remote directory: " + ex.Message + "\n";
                sFTP.Disconnect();
                return;
            }
        }

        private void AddRecursiveDirectory(DirectoryInfo directory, TreeNode parentNode)
        {
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                TreeNode newNode = new TreeNode(dir.Name);
                parentNode.Nodes.Add(newNode);
                AddRecursiveDirectory(dir, newNode);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                parentNode.Nodes.Add(new TreeNode(file.Name + " ... " + file.Length / 1024f + "KiB"));
            }
        }

        private void remoteFileTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var node = e.Node;
            if (node == null) return;

            // if we previously added exactly one “dummy” child (Tag==null), replace it:
            if (node.Nodes.Count == 1 && node.Nodes[0].Tag == null)
            {
                node.Nodes.Clear();
                sFTP.PopulateNode(ref node);
            }
        }
    }
}
