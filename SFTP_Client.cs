using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using static System.Net.WebRequestMethods;

namespace ELE_543_App
{
    class SFTP_Client
    {
        public string username { get; private set; }
        public string password { get; private set; }
        public string host { get; private set; }
        private SftpClient? client;
        private TreeNode? rootNode;

        public SFTP_Client(string username = "", string password = "", string host = "")
        {
            this.username = username;
            this.password = password;
            this.host = host;

            try
            {
                client = new SftpClient(host, username, password);
                client.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error connecting to SFTP server: " + e.Message);
                client = null;
            }

            if (client != null && client.IsConnected)
            {
                rootNode = new TreeNode("NAS Drive");
                rootNode.Tag = "/";
                PopulateNode(ref rootNode);
            }
            else
            {
                Console.WriteLine("Failed to populate directory listing of SFTP server.");
            }
        }

        public void Disconnect()
        {
            if (client != null && client.IsConnected)
            {
                client.Disconnect();
                client.Dispose();
                client = null;
            }
        }

        public TreeNode? GetTopLevelDirectory()
        {
            return rootNode;
        }

        public void UpdateHost(string newHost)
        {
            host = newHost;
        }

        public void UpdateUsername(string newUsername)
        {
            username = newUsername;
        }

        public void UpdatePassword(string newPassword)
        {
            password = newPassword;
        }

        /**
         * Populates the directory tree starting from the given path. This function is not recursive.
         * @param path The path to start populating from.
         * @param parentNode The parent node to add child nodes to.
         */
        public void PopulateNode(ref TreeNode parentNode)
        {
            if (client == null || !client.IsConnected)
            {
                Console.WriteLine("Client is not connected.");
                return;
            }

            var path = (string)parentNode.Tag;
            foreach (var entry in client.ListDirectory(path))
            {
                if (entry.Name is "." or "..")
                    continue;

                var child = new TreeNode(entry.Name)
                {
                    Tag = entry.FullName
                };

                if (entry.IsDirectory)
                {
                    // add a dummy so the ➕ shows up
                    child.Nodes.Add(new TreeNode("Loading...") { Tag = null });
                }
                else
                {
                    // annotate file size if you like
                    child.Text += $" ({entry.Length / 1024.0:F2} KiB)";
                }

                parentNode.Nodes.Add(child);
            }
        }

        /*
         * Populates the directory tree starting from the given path. This function is recursive.
         * @param path The path to start populating from.
         * @param parentNode The parent node to add child nodes to.
         */
        private void PopulateTotalTreeNode(string remotePath, TreeNode parentNode)
        {
            if (client == null || !client.IsConnected)
            {
                Console.WriteLine("Client is not connected.");
                return;
            }

            foreach (var entry in client.ListDirectory(remotePath))
            {
                // Skip the “.” and “..” entries
                if (entry.Name == "." || entry.Name == "..")
                    continue;

                // Create a new node and stash the full path in its Tag for later use
                var child = new TreeNode(entry.Name)
                {
                    Tag = entry.FullName
                };

                parentNode.Nodes.Add(child);

                // If it’s a directory, recurse
                if (entry.IsDirectory)
                {
                    try
                    {
                        PopulateTotalTreeNode(entry.FullName, child);
                    }
                    catch (Exception ex)
                    {
                        // Optional: mark inaccessible folders
                        child.Nodes.Add(new TreeNode($"<error: {ex.Message}>"));
                    }
                }
                else
                {
                    // Optionally, annotate file size
                    child.Text += $"  ({entry.Length / 1024.0:F2} KiB)";
                }
            }
        }
    }
}
