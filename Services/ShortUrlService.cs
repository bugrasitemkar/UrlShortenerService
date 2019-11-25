using Model;
using Repository.Abstract;
using Service.Map;
using Services.Abstract;
using System.Collections.Generic;

namespace Services
{
    public class ShortUrlServices : IShortUrlService
    {
        private IRepository shortUrlRepository;

        public ShortUrlServices(IRepository repository)
        {
            shortUrlRepository = repository;
        }

        public IEnumerable<ShortURLModel> GetCollectionFromDataStore()
        {
            return shortUrlRepository.GetCollectionFromDataStore();
        }

        public ShortURLModel GetItemFromDataStore(string shortUrl)
        {
            return shortUrlRepository.GetItemFromDataStoreByShortUrl(shortUrl);

        }

        public ShortUrlResponseModel SaveItemToDataStore(ShortURLRequestModel model)
        {
            ShortURLModel previouslySaved = shortUrlRepository.GetItemFromDataStoreByLongUrl(model.LongURL);
            if(previouslySaved!=null)
            {
                return new ShortUrlResponseModel { Model = previouslySaved, Success = true, Message = "This url has been saved previously" };
            }
            else
            {
                ShortURLModel savedModel = shortUrlRepository.SaveItemToDataStore(ShortUrlModelMapper.MapRequestModelToDBModel(model));

                return new ShortUrlResponseModel
                {
                    Model = savedModel,
                    Success = true,
                    Message = "Saved successfully"
                };
            }
            
        }
    }
}
