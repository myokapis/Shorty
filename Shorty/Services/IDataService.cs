using Shorty.Models;

namespace Shorty.Services
{
    public interface IDataService
    {
        Task<UrlDocument?> FindUrlDocumentByTag(string urlTag);
        Task<UrlDocument?> FindUrl(Uri fullUri);
        Task SaveUrl(UrlDocument url);
    }
}
