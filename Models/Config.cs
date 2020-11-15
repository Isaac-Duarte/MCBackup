using System;
using System.Collections.Generic;

namespace MCBackup.Models
{
    public class SshConfig
    {
        // Host of the SSH Server
        public string Host { get; set; }

        // Port of the SSH Server
        public int Port { get; set; }
        
        // Username of the SSH Server
        public string UserName { get; set; }
        
        // Password of the User
        public string Password { get; set; }
    }

    public class Config
    {
        // SSH Config
        public SshConfig SshConfig { get; set; }

        // Folders that are going to be backed up
        public List<string> BackupFolders { get; set; }

        // Minute interval to backup the files
        public int MinutesToBackup { get; set; }

        // The location to backup the file
        public string BackupFolderLocation { get; set; }

        // The temp folder location on the server
        public string TempFileLocation { get; set; }
    }
}