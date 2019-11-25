using Model;
using System.Collections.Generic;

namespace Repository.Abstract
{
    public interface IRepository
    {
        IEnumerable<ShortURLModel> GetCollectionFromDataStore();
        ShortURLModel GetItemFromDataStoreByShortUrl(string shortUrl);
        ShortURLModel GetItemFromDataStoreByLongUrl(string shortUrl);

        ShortURLModel SaveItemToDataStore(ShortURLModel model);
    }
}
