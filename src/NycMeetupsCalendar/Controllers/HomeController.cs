using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Newtonsoft.Json;
using NycMeetupsCalendar.Models;

namespace NycMeetupsCalendar.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Duration = 300, Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            var calendar = GetCalendar();
            var jsonEvents = GetEventsFromReddit(calendar);
            PopulateCalendar(jsonEvents, calendar);
            
            return View(calendar);

        }

        private static void PopulateCalendar(List<CalendarModels.Post> jsonEvents, Dictionary<DateTime, List<CalendarModels.Event>> calendar)
        {
            foreach (var e in jsonEvents)
            {
                var datePart = Regex.Match(e.link_flair_text, "[01][0-9]/[0-3][0-9]");
                if (datePart.Success)
                {
                    var correctKey = GetDateTimeKeyForEvent(calendar, datePart);
                    if (correctKey != default(DateTime))
                        calendar[correctKey].Add(new CalendarModels.Event {Url = e.url, Title = e.title,});
                }
            }
        }

        private static DateTime GetDateTimeKeyForEvent(Dictionary<DateTime, List<CalendarModels.Event>> calendar, Match datePart)
        {
            string monthDate = datePart.Value;
            var month = int.Parse(monthDate.Substring(0, 2));
            var day = int.Parse(monthDate.Substring(3, 2));
            var correctKey = calendar.Keys.SingleOrDefault(k => k.Month == month && k.Day == day);
            return correctKey;
        }

        private static List<CalendarModels.Post> GetEventsFromReddit(Dictionary<DateTime, List<CalendarModels.Event>> calendar)
        {
            var searchString = string.Join("+OR+",
                calendar.Keys.Select(r => HttpUtility.UrlEncode("flair:" + r.ToString("MM/dd"))));
            var searchUrl =
                $"https://www.reddit.com/r/nycmeetups/search.json?q={searchString}&sort=relevance&restrict_sr=on&t=month";
            var wc = new WebClient();
            var json = wc.DownloadString(searchUrl);
            var jsonEvents = JsonConvert.DeserializeObject<CalendarModels.Listing>(json).data.children.Select(d => d.data).ToList();
            return jsonEvents;
        }

        private static Dictionary<DateTime, List<CalendarModels.Event>> GetCalendar()
        {
            var today = DateTime.Today;
            var sunday = today.AddDays(-1*(int) today.DayOfWeek);
            var calendar = Enumerable.Range(0, 14)
                .Select(d => sunday.AddDays(d))
                .ToList()
                .ToDictionary(d => d, d => new List<CalendarModels.Event>());
            return calendar;
        }
    }


}