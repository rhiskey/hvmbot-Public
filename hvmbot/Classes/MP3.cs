using System;
using System.Collections.Generic;
using System.Text;

namespace hvmbot.Classes
{
    public class MP3
    {
        public string Link { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string Artists { get; set; }
        public Uri Thumb { get; set; } ////200kb 90x90px
        public MP3(string link, string name, string title, string artists, string message, Uri thumb)
        {
            this.Link = link;
            this.Name = name;
            this.Message = message;
            this.Title = title;
            this.Artists = artists;
            this.Thumb = thumb;
        }
        public MP3(string link, string name, string title, string artists, string message)
        {
            this.Link = link;
            this.Name = name;
            this.Message = message;
            this.Title = title;
            this.Artists = artists;
        }
    }
}
