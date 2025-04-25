using System;
using System.IO;
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

        // These are used to track the progress of the download
        private long _downloadTotalBytes;
        private long _downloadedBytes;
        private bool isDownloading;
        private Task? _downloadTask;

        // These are used to track the progress of the upload
        private long _uploadTotalBytes;
        private long _uploadedBytes;
        private Task? _uploadTask;

        public SFTP_Client(string username = "", string password = "", string host = "")
        {
            this.username = username;
            this.password = password;
            this.host = host;
            isDownloading = false;

            try
            {
                client = new SftpClient(host, 42069, username, password);
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

        public Task DownloadFile(string remoteFilePath, string localFilePath)
        {
            if (client == null) throw new Exception("Client is not connected");

            // 1) figure out how big the remote file is
            var attrs = client.GetAttributes(remoteFilePath);
            Interlocked.Exchange(ref _downloadTotalBytes, attrs.Size);
            Interlocked.Exchange(ref _downloadedBytes, 0L);

            // 2) fire off the download on a thread‐pool thread
            return Task.Run(() =>
            {
                // ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(localFilePath)!);

                using var fs = System.IO.File.OpenWrite(localFilePath);
                // the DownloadFile overload will call our lambda periodically
                client.DownloadFile(remoteFilePath, fs, downloadedBytes =>
                {
                    // update in a thread‐safe way
                    Interlocked.Exchange(ref _downloadedBytes, (long)downloadedBytes);
                });
            });
        }

        /// <summary>
        /// Returns a value from 0.0 to 1.0 indicating download progress.
        /// </summary>
        public float GetDownloadProgress()
        {
            long total = Interlocked.Read(ref _downloadTotalBytes);
            if (total == 0) return 0f;
            long done = Interlocked.Read(ref _downloadedBytes);
            return Math.Min(1f, (float)done / total);
        }

        public bool IsDownloadFinished() => _downloadTask?.IsCompleted ?? true;

        /// <summary>
        /// Begins an asynchronous upload of the local file to the given remote path.
        /// </summary>
        public Task UploadFile(string localFilePath, string remoteFilePath)
        {
            if (client == null)
                throw new InvalidOperationException("Client is not connected");

            // 1) figure out how big the local file is
            var fileInfo = new FileInfo(localFilePath);
            Interlocked.Exchange(ref _uploadTotalBytes, fileInfo.Length);
            Interlocked.Exchange(ref _uploadedBytes, 0L);

            // 2) fire off the upload on a background thread
            _uploadTask = Task.Run(() =>
            {
                using var fs = System.IO.File.OpenRead(localFilePath);
                client.UploadFile(fs, remoteFilePath, uploadedBytes =>
                {
                    // update in a thread‐safe way
                    Interlocked.Exchange(ref _uploadedBytes, (long)uploadedBytes);
                });
            });

            return _uploadTask;
        }

        /// <summary>
        /// Returns a value from 0.0 to 1.0 indicating upload progress.
        /// </summary>
        public float GetUploadProgress()
        {
            long total = Interlocked.Read(ref _uploadTotalBytes);
            if (total == 0) return 0f;
            long done = Interlocked.Read(ref _uploadedBytes);
            return Math.Min(1f, (float)done / total);
        }

        public bool IsUploadFinished() => _uploadTask?.IsCompleted ?? true;

        public void Disconnect()
        {
            if (client != null && client.IsConnected)
            {
                client.Disconnect();
                client.Dispose();
                client = null;
            }
        }

        public bool IsDirectory(string path)
        {
            if (client == null || !client.IsConnected)
            {
                Console.WriteLine("Client is not connected.");
                return false;
            }
            // Check if the path is a directory
            return client.GetAttributes(path).IsDirectory;
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
