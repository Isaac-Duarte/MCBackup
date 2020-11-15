using System;
using System.Collections.Generic;
using System.Text;
using MCBackup.Models;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace MCBackup.Services
{
    public class SSHService
    {
        private readonly ILogger _logger;
        private readonly SshConfig _config;
        private SshClient client;

        public SSHService(ILogger<SSHService> logger, SshConfig config)
        {
            _logger = logger;
            _config = config;
        }
        
        /// <summary>
        /// Open's the SSH Connection
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                client = new SshClient(_config.Host, _config.Port, _config.UserName, _config.Password); 
                client.Connect();
                _logger.LogInformation($"Opened SSH coponnection to [{_config.Host}]");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed opening SSH Connection to [{_config.Host}]");
            }
        }
        
        /// <summary>
        /// Closes the SSH Connection
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
        /// Runs a command on the SSH Connection
        /// </summary>
        /// <param name="command">Command to run</param>
        /// <returns>SshCommand object</returns>
        public SshCommand RunCommand(string command)
        {
            try
            {
                return client.RunCommand(command);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed while runing SSH command [{command}]");
                return null;
            }
        }

        /// <summary>
        /// Zips all the folders in a list
        /// </summary>
        /// <param name="folders">The list of folders wanting to be zipped</param>
        /// <param name="outputFile">The output zip file</param>
        /// <returns>The SSHCommand object</returns>
        public SshCommand ZipFolders(List<string> folders, string outputFile)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"zip -r {outputFile} ");

            foreach (string folder in folders)
            {
                stringBuilder.Append($"{folder} ");
            }

            string command = stringBuilder.ToString();

            try
            {
                _logger.LogInformation($"Zipping requested folders.");
                return client.RunCommand(command);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed while runing SSH command [{command}]");
                return null;
            }
        }
    }    
}