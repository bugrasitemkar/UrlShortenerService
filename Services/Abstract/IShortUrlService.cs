using Model;
using System.Collections.Generic;

namespace Services.Abstract
{
    public interface IShortUrlService
    {
        IEnumerable<ShortURLModel> GetCollectionFromDataStore();
        ShortURLModel GetItemFromDataStore(string shortUrl);
        ShortUrlResponseModel SaveItemToDataStore(ShortURLRequestModel model);
    }
}
