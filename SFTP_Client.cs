using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace ELE_543_App
{
    class SFTP_Client
    {
        public string username { get; private set; }
        public string password { get; private set; }
        public string host { get; private set; }
        private SftpClient client;

        public SFTP_Client(string username = "", string password = "", string host = "")
        {
            this.username = username;
            this.password = password;
            this.host = host;
            client = new SftpClient(host, username, password);
            client.Connect();
        }

        // TODO: Populate this function
        public DirectoryInfo GetFileDirectory()
        {
            return new DirectoryInfo("");
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
    }
}
