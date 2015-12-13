using System;
using System.Collections.Generic;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     南方海域珊瑚諸島沖の制空権を握れ！
    /// </summary>
    internal class Bw9 : ITracker
    {
        private readonly List<string> boss_names = new List<string>
                                                   {
                                                       "敵機動部隊本隊"
                                                   };

        private readonly int map_id = 5;
        private readonly int max_count = 2;

        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 243;

        string ITracker.WikiIndex => "Bw9";

        string ITracker.Name => "南方海域珊瑚諸島沖の制空権を握れ！";

        QuestType ITracker.Type => QuestType.Weekly;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.BattleResultEvent += (sender, args) =>
                                          {
                                              if (!IsTracking)
                                                  return;

                                              if (args.MapAreaId != map_id)
                                                  return;

                                              if (!boss_names.Contains(args.EnemyName))
                                                  return;

                                              if (args.Rank != "S")
                                                  return;

                                              count += count >= max_count ? 0 : 1;

                                              ProcessChanged?.Invoke(this, new EventArgs());
                                          };
        }

        public void ResetQuest()
        {
            count = 0;

            ProcessChanged?.Invoke(this, new EventArgs());
        }

        public double GetPercentProcess()
        {
            return (double)count / max_count * 100;
        }

        public string GetDisplayProcess()
        {
            return count >= max_count ? "完成" : $"{count} / {max_count}";
        }

        public string SerializeData()
        {
            return $"{count}";
        }

        public void DeserializeData(string data)
        {
            try
            {
                count = int.Parse(data);
            }
            catch
            {
                count = 0;
            }
        }

        #endregion
    }
}