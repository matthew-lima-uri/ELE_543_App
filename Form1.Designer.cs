namespace ELE_543_App
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            userNameInput = new TextBox();
            userNameLabel = new Label();
            passwordLabel = new Label();
            passwordInput = new TextBox();
            connectButton = new Button();
            statusBox = new RichTextBox();
            statusLabel = new Label();
            remoteFileTree = new TreeView();
            localFileTree = new TreeView();
            localFileLabel = new Label();
            remoteFileLabel = new Label();
            downloadButton = new Button();
            uploadFileButton = new Button();
            hostLabel = new Label();
            hostInput = new TextBox();
            SuspendLayout();
            // 
            // userNameInput
            // 
            userNameInput.Location = new Point(12, 87);
            userNameInput.Name = "userNameInput";
            userNameInput.Size = new Size(174, 23);
            userNameInput.TabIndex = 0;
            userNameInput.Text = "sftpuser";
            // 
            // userNameLabel
            // 
            userNameLabel.AutoSize = true;
            userNameLabel.Location = new Point(69, 69);
            userNameLabel.Name = "userNameLabel";
            userNameLabel.Size = new Size(60, 15);
            userNameLabel.TabIndex = 1;
            userNameLabel.Text = "Username";
            userNameLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(69, 113);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(57, 15);
            passwordLabel.TabIndex = 3;
            passwordLabel.Text = "Password";
            passwordLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // passwordInput
            // 
            passwordInput.Location = new Point(12, 131);
            passwordInput.MaxLength = 16;
            passwordInput.Name = "passwordInput";
            passwordInput.PasswordChar = '*';
            passwordInput.Size = new Size(174, 23);
            passwordInput.TabIndex = 2;
            // 
            // connectButton
            // 
            connectButton.Location = new Point(12, 160);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(174, 23);
            connectButton.TabIndex = 4;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // statusBox
            // 
            statusBox.Location = new Point(12, 342);
            statusBox.Name = "statusBox";
            statusBox.Size = new Size(776, 96);
            statusBox.TabIndex = 5;
            statusBox.Text = "";
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(12, 324);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(93, 15);
            statusLabel.TabIndex = 6;
            statusLabel.Text = "Status Messages";
            // 
            // remoteFileTree
            // 
            remoteFileTree.Location = new Point(496, 32);
            remoteFileTree.Name = "remoteFileTree";
            remoteFileTree.Size = new Size(292, 304);
            remoteFileTree.TabIndex = 7;
            remoteFileTree.BeforeExpand += remoteFileTree_BeforeExpand;
            // 
            // localFileTree
            // 
            localFileTree.Location = new Point(198, 32);
            localFileTree.Name = "localFileTree";
            localFileTree.Size = new Size(292, 306);
            localFileTree.TabIndex = 8;
            // 
            // localFileLabel
            // 
            localFileLabel.AutoSize = true;
            localFileLabel.Location = new Point(316, 14);
            localFileLabel.Name = "localFileLabel";
            localFileLabel.Size = new Size(61, 15);
            localFileLabel.TabIndex = 9;
            localFileLabel.Text = "Local Files";
            localFileLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // remoteFileLabel
            // 
            remoteFileLabel.AutoSize = true;
            remoteFileLabel.Location = new Point(605, 14);
            remoteFileLabel.Name = "remoteFileLabel";
            remoteFileLabel.Size = new Size(74, 15);
            remoteFileLabel.TabIndex = 10;
            remoteFileLabel.Text = "Remote Files";
            remoteFileLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // downloadButton
            // 
            downloadButton.Location = new Point(12, 214);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(174, 23);
            downloadButton.TabIndex = 11;
            downloadButton.Text = "Download Files";
            downloadButton.UseVisualStyleBackColor = true;
            // 
            // uploadFileButton
            // 
            uploadFileButton.Location = new Point(12, 243);
            uploadFileButton.Name = "uploadFileButton";
            uploadFileButton.Size = new Size(174, 23);
            uploadFileButton.TabIndex = 12;
            uploadFileButton.Text = "Upload Files";
            uploadFileButton.UseVisualStyleBackColor = true;
            // 
            // hostLabel
            // 
            hostLabel.AutoSize = true;
            hostLabel.Location = new Point(82, 25);
            hostLabel.Name = "hostLabel";
            hostLabel.Size = new Size(32, 15);
            hostLabel.TabIndex = 14;
            hostLabel.Text = "Host";
            hostLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // hostInput
            // 
            hostInput.Location = new Point(12, 43);
            hostInput.Name = "hostInput";
            hostInput.Size = new Size(174, 23);
            hostInput.TabIndex = 13;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(hostLabel);
            Controls.Add(hostInput);
            Controls.Add(uploadFileButton);
            Controls.Add(downloadButton);
            Controls.Add(remoteFileLabel);
            Controls.Add(localFileLabel);
            Controls.Add(localFileTree);
            Controls.Add(remoteFileTree);
            Controls.Add(statusLabel);
            Controls.Add(statusBox);
            Controls.Add(connectButton);
            Controls.Add(passwordLabel);
            Controls.Add(passwordInput);
            Controls.Add(userNameLabel);
            Controls.Add(userNameInput);
            Name = "Form1";
            Text = "Matt's NAS";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox userNameInput;
        private Label userNameLabel;
        private Label passwordLabel;
        private TextBox passwordInput;
        private Button connectButton;
        private RichTextBox statusBox;
        private Label statusLabel;
        private TreeView remoteFileTree;
        private TreeView localFileTree;
        private Label localFileLabel;
        private Label remoteFileLabel;
        private Button downloadButton;
        private Button uploadFileButton;
        private Label hostLabel;
        private Label label2;
        private TextBox hostInput;
    }
}
