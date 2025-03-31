namespace ELE_543_App
{
    public partial class Form1 : Form
    {
        string localFilePath = "C:\\MattSFTP";
        SFTP_Client sFTP;
        DirectoryInfo localDirectory;
        string localPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load the current directory, and make one if it doesn't exist
            localDirectory = new DirectoryInfo(localFilePath);

            foreach (FileInfo file in localDirectory.GetFiles())
            {
                
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

            FileDirectory files = sFTP.GetFileDirectory();
            remoteFileTree.TopNode = new TreeNode(files.directoryName);
            foreach (FileDirectory dir in files.folders)
            {
            }
        }
    }
}
