using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace SynoCamLib
{
    public interface IApiConnector
    {
        string DownloadEvent(ICamEvent camEvent, AsyncCompletedEventHandler fileDownloadCompleted, DownloadProgressChangedEventHandler progressChanged);
        Task<List<ICam>> GetCamsASync();
        Task<bool> LogoutASync();
        Task<IEnumerable<ICamEvent>> QueryCamEvents();
    }
}