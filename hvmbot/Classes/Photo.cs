using System;
using System.Collections.Generic;
using System.Text;

namespace hvmbot.Classes
{
    public class Photo
    {
        public string LocalPath { get; set; }
        public string Caption { get; set; }
        public List<Uri> largePhotoUris { get; set; }
        public Uri smallPhotoUri { get; set; }
        public Photo(string localpath, string caption)
        {
            this.LocalPath = localpath;
            this.Caption = caption;
        }

        public Photo(List<Uri> Uris, Uri smallPhoto)
        {
            this.largePhotoUris = Uris;
            this.smallPhotoUri = smallPhoto;
        }
    }
}
