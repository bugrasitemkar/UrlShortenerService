using System;
using System.ComponentModel.DataAnnotations;
using static Model.Validation.CustomValidation;

namespace Model
{
    public class ShortURLRequestModel
    {
        [Required]
        [CheckUrlValid(ErrorMessage = "Please enter a valid Url")]
        public string LongURL { get; set; }

        public DateTime TimeStamp
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}
