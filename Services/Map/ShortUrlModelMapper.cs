using Model;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Map
{
    public static class ShortUrlModelMapper
    {
        public static ShortURLModel MapRequestModelToDBModel(ShortURLRequestModel requestModel)
        {
            ShortURLModel result = new ShortURLModel
            {
                CreateDate = DateTime.Now,
                LongURL = requestModel.LongURL
            };

            result.ShortURL = TokenGenerator.GenerateShortUrl();

            return result;
        }
    }
}
