﻿using System.Collections.Generic;

namespace NycMeetupsCalendar.Models
{
    public class CalendarModels
    {
        public class Event
        {
            public string Url { get; set; }
            public string Title { get; set; }
        }

        public class Listing
        {
            public ListingData data { get; set; }
        }

        public class ListingData
        {
            public List<PostWrapper> children { get; set; }
        }

        public class PostWrapper
        {
            public Post data { get; set; }
        }

        public class Post
        {
            public string title { get; set; }
            public string url { get; set; }
            public string link_flair_text { get; set; }

        }
    }
}