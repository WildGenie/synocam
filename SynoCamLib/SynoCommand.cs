using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using SynoCamLib.DemoMode;

namespace SynoCamLib
{
    public class SynoCommand
    {
        private IApiConnector _connector;
        
        /// <summary>
        /// Use this empty constructor for demonstration/UI design purposes
        /// </summary>
        public SynoCommand()
        {
            _connector = new DemoApiConnector(); 
        }

        /// <summary>
        /// Use this constructor to create a real connection to a Synology API
        /// </summary>
        public SynoCommand(string url, string username, string password)
        {
            _connector = new ApiConnector(url, username, password);
        }

        public string DownloadEvent(ICamEvent camEvent, AsyncCompletedEventHandler fileDownloadCompleted, DownloadProgressChangedEventHandler progressChanged)
        {
            return _connector.DownloadEvent(camEvent, fileDownloadCompleted, progressChanged);
        }

        public async Task<List<ICam>> GetCamsASync()
        {
            return await _connector.GetCamsASync();
        }

        public async Task<bool> LogoutASync()
        {
            return await _connector.LogoutASync();
        }

        public Task<IEnumerable<ICamEvent>> QueryCamEvents()
        {
            return _connector.QueryCamEvents();
        }
    }
}