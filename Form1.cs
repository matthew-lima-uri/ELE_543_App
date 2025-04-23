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
            // Load the current directory, and make one if it doesn't exist
            localDirectory = new DirectoryInfo(localFilePath);

            rootNode = new TreeNode();
            rootNode.Text = localDirectory.FullName;
            localFileTree.Nodes.Add(rootNode);

            foreach (DirectoryInfo dir in localDirectory.GetDirectories())
            {
                localFileTree.Nodes.Add(new TreeNode(dir.Name));
            }

            foreach (FileInfo file in localDirectory.GetFiles())
            {
                localFileTree.Nodes.Add(new TreeNode(file.Name + " ... " + file.Length / (1024*1024) + "MiB"));
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
            catch
            {
                statusBox.Text += "Failed to connect to the SFTP Server...\n";
            }

            DirectoryInfo files = sFTP.GetFileDirectory();
            foreach (FileInfo file in files.GetFiles())
            {
            }
        }
    }
}
