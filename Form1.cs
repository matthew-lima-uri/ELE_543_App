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

            localFileTree.Sorted = true;
            localFileTree.HideSelection = false;
            remoteFileTree.Sorted = true;
            remoteFileTree.HideSelection = false;
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
                rootNode.Nodes.Add(new TreeNode(file.Name + " (" + file.Length / 1024f + "KiB)"));
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
                connectedLabel.Text = "Disconnected";
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
                    connectedLabel.Text = "Disconnected";
                    sFTP.Disconnect();
                }
            }
            catch (Exception ex)
            {
                statusBox.Text += "Error populating remote directory: " + ex.Message + "\n";
                sFTP.Disconnect();
                connectedLabel.Text = "Disconnected";
                return;
            }
            connectedLabel.Text = "CONNECTED";
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
                parentNode.Nodes.Add(new TreeNode(file.Name + " (" + file.Length / 1024f + "KiB)"));
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sFTP != null)
            {
                sFTP.Disconnect();
            }
            connectedLabel.Text = "Disconnected";
        }

        private async void downloadButton_Click(object sender, EventArgs e)
        {
            if (sFTP == null)
            {
                statusBox.AppendText("No SFTP connection established. Please connect first.\n");
                return;
            }

            if (remoteFileTree.SelectedNode == null)
            {
                statusBox.AppendText("No file selected for download.\n");
                return;
            }

            // Grab the selected remote node
            var fileNode = remoteFileTree.SelectedNode;
            if (fileNode.Tag is not string remotePath || sFTP.IsDirectory(remotePath))
            {
                statusBox.AppendText("Please select a file, not a directory.\n");
                return;
            }

            // 1) Extract just the filename (no size text)
            string fileName = Path.GetFileName(remotePath);

            // 2) Determine target local folder
            TreeNode parentNode = localFileTree.SelectedNode != null && Directory.Exists(localFileTree.SelectedNode.Tag as string) ? localFileTree.SelectedNode : rootNode;
            string? targetFolder = parentNode.Text;     // default root
            if (localFileTree.SelectedNode?.Tag is string sel && Directory.Exists(sel))
                targetFolder = sel;

            string localFilePath = Path.Combine(targetFolder, fileName);

            statusBox.AppendText($"Beginning download: {fileName}\n");

            // 3) Set up progress timer
            var timer = new System.Windows.Forms.Timer { Interval = 200 };
            timer.Tick += (_, __) =>
            {
                float pct = sFTP.GetDownloadProgress();
                progressBar1.Value = (int)(pct * progressBar1.Maximum);
            };
            timer.Start();

            // 4) Do the download (blocking here—could be offloaded to Task.Run if you like)
            try
            {
                await sFTP.DownloadFile(remotePath, localFilePath);
            }
            catch (Exception ex)
            {
                timer.Stop();
                statusBox.AppendText($"Error downloading {fileName}: {ex.Message}\n");
                return;
            }

            // 5) Wait until done
            while (!sFTP.IsDownloadFinished())
                Thread.Sleep(100);

            timer.Stop();
            statusBox.AppendText($"Download finished: {fileName}\n");
            progressBar1.Value = 0;
            var newNode = new TreeNode(fileNode.Text)
            {
                Tag = localFilePath
            };
            parentNode.Nodes.Add(newNode);
            parentNode.Expand();
        }

        private async void uploadButton_Click(object sender, EventArgs e)
        {
            // 1) sanity checks
            if (sFTP == null)
            {
                statusBox.AppendText("No SFTP connection established. Please connect first.\n");
                return;
            }

            // 2) Make sure a local file is selected
            var localNode = localFileTree.SelectedNode;
            if (localNode.FullPath == null || Directory.Exists(localNode.FullPath.Trim('/')))
            {
                statusBox.AppendText("Please select a *file* to upload (not a folder).\n");
                return;
            }
            string localPath = localNode.FullPath.Split([" ("], StringSplitOptions.None)[0].Trim();

            // 3) Make sure a remote directory is selected
            var remoteNode = remoteFileTree.SelectedNode;
            if (remoteNode?.Tag is not string remoteDir || !sFTP.IsDirectory(remoteDir))
            {
                statusBox.AppendText("Please select a *remote folder* to upload into.\n");
                return;
            }

            // 4) Compute names & paths
            string fileName = Path.GetFileName(localPath);
            string remoteFilePath = remoteDir.TrimEnd('/') + "/" + fileName;

            statusBox.AppendText($"Beginning upload: {fileName}\n");

            // 5) Wire up a timer to drive the progress bar
            var timer = new System.Windows.Forms.Timer { Interval = 200 };
            timer.Tick += (_, _) =>
            {
                float pct = sFTP.GetUploadProgress();
                progressBar1.Value = (int)(pct * progressBar1.Maximum);
            };
            timer.Start();

            // 6) Kick off the async upload
            try
            {
                await sFTP.UploadFile(localPath, remoteFilePath);
            }
            catch (Exception ex)
            {
                timer.Stop();
                statusBox.AppendText($"Error uploading {fileName}: {ex.Message}\n");
                return;
            }

            // 7) Wait until fully done
            while (!sFTP.IsUploadFinished())
                Thread.Sleep(50);

            timer.Stop();
            progressBar1.Value = 0;
            statusBox.AppendText($"Upload finished: {fileName}\n");

            // 8) Insert the new node into the remote tree
            var parentNode = remoteNode;
            var newNode = new TreeNode(localNode.Text)
            {
                Tag = remoteFilePath
            };
            parentNode.Nodes.Add(newNode);
            parentNode.Expand();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            sFTP?.Disconnect();
            Application.Exit();
        }
    }
}
