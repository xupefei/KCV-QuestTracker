using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleViewer.Plugins
{
    internal class QuestManager
    {
        private readonly ObservableCollection<ITracker> availableTrackers = new ObservableCollection<ITracker>();

        private readonly Dictionary<int, DateTime> questsStartTracking = new Dictionary<int, DateTime>();
        private readonly ApiEvent apiEvent;

        public QuestManager(KanColleClient client)
        {
            apiEvent = new ApiEvent(client);

            availableTrackers.CollectionChanged += AvailableTrackers_CollectionChanged;

            // register all trackers
            RegisterAllTrackers();

            client.Proxy.api_get_member_questlist.TryParse<kcsapi_questlist>().Subscribe(x => ProcessQuestList(x.Data));
        }

        public QuestProcessCollectionItem[] TrackingProcessStrings
        {
            get
            {
                var result = new List<QuestProcessCollectionItem>();

                var trackers = availableTrackers.Where(t => t.IsTracking);

                if (!trackers.Any())
                    return result.ToArray();

                trackers.ToList()
                        .ForEach(
                                 t =>
                                 result.Add(new QuestProcessCollectionItem
                                            {
                                                Id = t.Id.ToString(),
                                                WikiIndex = t.WikiIndex,
                                                Name = t.Name,
                                                ProcessPercent =
                                                    t.GetPercentProcess() > 100 ? 100d : t.GetPercentProcess(),
                                                ProcessText = t.GetDisplayProcess()
                                            }));

                return result.ToArray();
            }
        }

        public event EventHandler QuestsEventChanged;

        protected virtual void OnEventProcessChanged(EventArgs e)
        {
            QuestsEventChanged?.Invoke(this, e);
        }

        private void RegisterAllTrackers()
        {
            Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .ToList()
                    .Where(t => t.Namespace == "Grabacr07.KanColleViewer.Plugins.Trackers")
                    .ToList()
                    .ForEach(i => availableTrackers.Add((ITracker)Activator.CreateInstance(i)));

            // read storage info for all trackers
            var storage = QuestFS.GetQuests();

            availableTrackers.ToList().ForEach(tracker =>
                                               {
                                                   if (!storage.Any(qi => qi.Id == tracker.Id))
                                                       return;

                                                   var questItem = storage.Where(qi => qi.Id == tracker.Id).First();

                                                   // if the tracking is still available, restore the pervious process.
                                                   if (IsTrackingAvailable(questItem.Type, questItem.TrackingStart))
                                                   {
                                                       questsStartTracking.Add(questItem.Id, questItem.TrackingStart);

                                                       tracker.IsTracking = questItem.IsTracking;

                                                       tracker.DeserializeData(questItem.ProcessStorageString);
                                                   }
                                               });

            OnEventProcessChanged(new EventArgs());
        }

        private void AvailableTrackers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            foreach (ITracker tracker in e.NewItems)
            {
                tracker.RegisterEvent(apiEvent);
                tracker.ResetQuest();
                tracker.ProcessChanged += Tracker_ProcessChanged;
            }
        }

        private void Tracker_ProcessChanged(object sender, EventArgs e)
        {
            OnEventProcessChanged(new EventArgs());

            SaveTrackerProcessesToStorage();
        }

        private void SaveTrackerProcessesToStorage()
        {
            var list = new List<QuestItem>();

            foreach (var tracker in availableTrackers)
            {
                var item = new QuestItem
                           {
                               Id = tracker.Id,
                               Type = tracker.Type,
                               Name = tracker.Name,
                               IsTracking = tracker.IsTracking
                           };

                DateTime dT;
                questsStartTracking.TryGetValue(tracker.Id, out dT);
                item.TrackingStart = dT;

                item.ProcessStorageString = tracker.SerializeData();

                list.Add(item);
            }

            QuestFS.SaveQuests(list.ToArray());
        }

        private void ProcessQuestList(kcsapi_questlist api)
        {
            if (api.api_list == null || !api.api_list.Any())
                return;

            var minId = api.api_list.Min(l => l.api_no);
            var maxId = api.api_list.Max(l => l.api_no);

            // if a tracker should in this page but not appeared, we should treat it as expired.
            if (availableTrackers.Any(t => t.Id > minId && t.Id < maxId))
            {
                availableTrackers
                    .Where(t => t.Id > minId && t.Id < maxId)
                    .ToList()
                    .ForEach(t =>
                             {
                                 if (api.api_list.All(kq => kq.api_no != t.Id))
                                 {
                                     t.IsTracking = false;
                                     if (questsStartTracking.ContainsKey(t.Id))
                                         questsStartTracking.Remove(t.Id);
                                 }
                             });
            }

            foreach (var quest in api.api_list)
            {
                var tracker = availableTrackers.Where(t => t.Id == quest.api_no);
                if (!tracker.Any())
                    continue;

                switch (quest.api_state)
                {
                    case 1:
                        tracker.First().IsTracking = false;
                        break;

                    case 2:
                        tracker.First().IsTracking = true; // quest taking
                        // update tracking date, if nessessary
                        if (!questsStartTracking.ContainsKey(quest.api_no))
                            questsStartTracking.Add(quest.api_no, GetTokyoDateTime());
                        break;

                    case 3:
                        tracker.First().IsTracking = false;

                        // delete tracking date
                        if (questsStartTracking.ContainsKey(quest.api_no))
                            questsStartTracking.Remove(quest.api_no);
                        break;
                }
            }

            OnEventProcessChanged(new EventArgs());

            SaveTrackerProcessesToStorage();
        }

        private bool IsTrackingAvailable(QuestType type, DateTime time)
        {
            // The quests are refreshed everyday/week at 5AM(UTC+9).
            // if we subtract the time by 5h, we can then say the refresh time is 0AM(UTC+4).
            // It will be easier to check the availibility.
            // One example is "Arabian Standard Time (ar-AE)": UTC+4, no daylight saving

            if (time == DateTime.MinValue)
                return false;

            var no = GetTokyoDateTime().AddHours(-5);
            time = time.AddHours(-5);

            switch (type)
            {
                case QuestType.OneTime:
                    return true;

                case QuestType.Daily:
                    return time.Date == no.Date;

                case QuestType.Weekly:
                    var cal = CultureInfo.CreateSpecificCulture("ar-AE").Calendar;
                    var w_time = cal.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    var w_now = cal.GetWeekOfYear(no, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    return w_time == w_now && time.Year == no.Year;

                case QuestType.Monthly:
                    return time.Month == no.Month && time.Year == no.Year;

                default:
                    return false;
            }
        }

        private DateTime GetTokyoDateTime()
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Tokyo Standard Time");
        }
    }
}