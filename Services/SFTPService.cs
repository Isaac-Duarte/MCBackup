using System;
using System.IO;
using MCBackup.Models;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace MCBackup.Services
{
    public class SFTPService
    {
        private readonly ILogger _logger;
        private readonly SshConfig _config;
        private SftpClient client;

        public SFTPService(ILogger<SFTPService> logger, SshConfig config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Opens a SFTP Connection
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                client = new SftpClient(_config.Host, _config.Port, _config.UserName, _config.Password);
                client.Connect();
                _logger.LogInformation($"Opened SSH coponnection to [{_config.Host}]");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed opening SSH Connection to [{_config.Host}]");
            }
        }

        /// <summary>
        ///  Closes the SFTP Connection
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                client.Disconnect();
                _logger.LogInformation($"Disconnected from [{_config.Host}]");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed closing SSH Connection to [{_config.Host}]");
            }
        }

        /// <summary>
        /// Downloads a file from the server
        /// </summary>
        /// <param name="remoteFilePath">The file path on the server</param>
        /// <param name="localFilePath">The local path of the client</param>
        public void DownloadFile(string remoteFilePath, string localFilePath)
        {
            try
            {
                using (FileStream fileStream = File.Create(localFilePath))
                {
                    client.DownloadFile(remoteFilePath, fileStream);
                    _logger.LogInformation($"Finished downloading file [{localFilePath}] from [{remoteFilePath}]");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed downloading file [{localFilePath}] from [{remoteFilePath}]");
            }
        }

        /// <summary>
        /// Deletes a file on the server
        /// </summary>
        /// <param name="remoteFilePath">The file path on the server</param>
        public void DeleteFile(string remoteFilePath)
        {
            try
            {
                client.DeleteFile(remoteFilePath);
                _logger.LogInformation($"Deleted file [{remoteFilePath}]");

            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed deleting file [{remoteFilePath}]");
            }
        }
    }
}