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
            Uri largePhoto = null;
            List<Uri> getLargePhoto = new List<Uri>();
            Uri thumbUrl = null;
            //Find Largest photo
            foreach (var photo in photoList)
            {
                ulong? maxHeight = 0, maxWidth = 0, minWidth = 200;

                foreach (var size in photo.Sizes)
                {
                    var h = size.Height;
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
                        thumbUrl = size.Url;
                    }
                }
                getLargePhoto.Add(largePhoto);
            }
            Photo toReturn = new Photo(getLargePhoto, thumbUrl);
            return toReturn;
        }
    }
}
