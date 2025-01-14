using LiteDB;
using LiteDB.Async;
using Shorty.Models;

namespace Shorty.Services
{
    public class DataService : IDataService
    {
        private readonly ILiteDatabaseAsync database;

        public DataService(ILiteDatabaseAsync database)
        {
            this.database = database;
        }

        public async Task<UrlDocument?> FindUrlDocumentByTag(string urlTag)
        {
            return await ShortUrlCollection.Value.FindOneAsync(c => c.Tag == urlTag);
        }

        public async Task<UrlDocument?> FindUrl(Uri fullUri)
        {
            return await ShortUrlCollection.Value.FindOneAsync(c => c.FullUrl == fullUri.ToString());
        }

        public async Task SaveUrl(UrlDocument url)
        {
            // NOTE: in a real world app we would need to consider how long to retain a URL.
            //       There should be some expiration date associated with the record/document.
            await ShortUrlCollection.Value.InsertAsync(url);
        }

        protected Lazy<ILiteCollectionAsync<UrlDocument>> ShortUrlCollection => 
            new(() => database.GetCollection<UrlDocument>());
    }
}
