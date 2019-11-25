using System;
using System.Collections.Generic;

namespace Model
{
    public class ShortUrlResponseModel
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public ShortURLModel Model { get; set; }
    }
}
