using hvmbot.Classes;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using System;
using System.Collections.Generic;
using System.Text;

namespace hvmbot.VKPart
{
    class GetLargePhoto
    {
        public static Photo GetLargeAndThumbUrl(List<VkNet.Model.Attachments.Photo > photoList)
        {
            //var getLargePhoto = function(photo) {
            //    return photo.photo_2560 || photo.photo_1280 || photo.photo_807 || photo.photo_604 || photo.photo_130 || photo.photo_75;
            //};
            Uri largePhoto = null;
            //thumbUrl = null;
            List<Uri> getLargePhoto = new List<Uri>();
            Uri thumbUrl = null;
            //Ищем самую большую фотку
            foreach (var photo in photoList)
            {
                ulong? maxHeight = 0, maxWidth = 0, minWidth = 200;


                foreach (var size in photo.Sizes)
                {

                    var h = size.Height; //Current
                    var w = size.Width;

                    //Get Largest
                    if (w > maxWidth)
                    {
                        maxWidth = w;
                        largePhoto = size.Url;
                    }

                    //Get Smallest for thumb
                    if (w <= minWidth && w > 75)
                    {
                        minWidth = w;

                        ////Не работает пока отправка (почему-то не грузит фотку), надо её скачать и загрузить
                        thumbUrl = size.Url;
                    }
                    ////Если картинка вертикальная
                    //if (h > maxHeight)
                    //    maxHeight = h;


                    //if (size.Type == PhotoSizeType.M) //либо S
                    //    if (size.Url != null)
                    //        thumbUrl = size.Url;
                }
                getLargePhoto.Add(largePhoto);
            }
            Photo toReturn = new Photo(getLargePhoto, thumbUrl);
            return toReturn;
        }
    }
}
