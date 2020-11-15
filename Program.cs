using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using MCBackup.Models;
using MCBackup.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Renci.SshNet;

namespace MCBackup
{
    class Program
    {
        private static Config _config;
        private static ILoggerFactory loggerFactory;

        /// <summary>
        /// Binds the config file to the 'Config' object
        /// </summary>
        private static void loadConfig()
        {
            ConfigurationBuilder configurationBuilder;
            IConfigurationRoot configurationRoot;
        
            configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("config.json", false, false);
            configurationRoot = configurationBuilder.Build();

            _config = configurationRoot.Get<Config>();
        }

        /// <summary>
        /// Backs up the world folders
        /// </summary>
        static void backupWorlds()
        {
            string fileLocation = $"minecraft-backup-{DateTime.Now.ToFileTimeUtc()}.zip";
            
            SSHService sshService = new SSHService(loggerFactory.CreateLogger<SSHService>(), _config.SshConfig);
            sshService.OpenConnection();
            sshService.ZipFolders(_config.BackupFolders, $"{_config.TempFileLocation}/{fileLocation}");
            sshService.CloseConnection();

            SFTPService sftpService = new SFTPService(loggerFactory.CreateLogger<SFTPService>(), _config.SshConfig);
            sftpService.OpenConnection();
            sftpService.DownloadFile($"{_config.TempFileLocation}/{fileLocation}", $"{_config.BackupFolderLocation}/{fileLocation}");
            sftpService.DeleteFile($"{_config.TempFileLocation}/{fileLocation}");
            sftpService.OpenConnection();
            sftpService.CloseConnection();
        }

        /// <summary>
        /// Handles the time event.
        /// </summary>
        private static void onTimedEvent(object source, ElapsedEventArgs e)
        {
            backupWorlds();
        }

        /// <summary>
        /// Main method for the program
        /// </summary>
        static void Main(string[] args)
        {
            loadConfig();

            if (!Directory.Exists(_config.BackupFolderLocation))
                Directory.CreateDirectory(_config.BackupFolderLocation);

            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole();
            });

            backupWorlds();

            var timer = new System.Timers.Timer(_config.MinutesToBackup * 60 * 1000);
            timer.Elapsed += new ElapsedEventHandler(onTimedEvent);
            timer.Start();

            Console.WriteLine("Press enter to close");
            Console.ReadLine();
        }
    }
}
