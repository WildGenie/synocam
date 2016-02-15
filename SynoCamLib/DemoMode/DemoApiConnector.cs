using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace SynoCamLib.DemoMode
{
    internal class DemoApiConnector : IApiConnector
    {
        public string DownloadEvent(ICamEvent camEvent, AsyncCompletedEventHandler fileDownloadCompleted, DownloadProgressChangedEventHandler progressChanged)
        {
            throw new NotImplementedException();
        }

        public Task<List<ICam>> GetCamsASync()
        {
            return Task.Factory.StartNew(()=> new List<ICam> {new DemoCam(1), new DemoCam(2)});
        }

        public Task<bool> LogoutASync()
        {
            return Task.Factory.StartNew(() => true);
        }

        public Task<IEnumerable<ICamEvent>> QueryCamEvents()
        {
            throw new NotImplementedException();
        }
    }
}
