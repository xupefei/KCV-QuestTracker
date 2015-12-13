using System;
using System.Collections.Generic;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     南西諸島海域の制海権を握れ！
    /// </summary>
    internal class Bd7 : ITracker
    {
        private readonly List<string> boss_names = new List<string>
                                                   {
                                                       "敵主力艦隊",
                                                       "敵通商破壊艦隊",
                                                       "敵主力打撃群",
                                                       "敵侵攻中核艦隊",
                                                       "敵主力艦隊"
                                                   };

        private readonly int map_id = 2;
        private readonly int max_count = 5;

        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 226;

        string ITracker.WikiIndex => "Bd7";

        string ITracker.Name => "南西諸島海域の制海権を握れ！";

        QuestType ITracker.Type => QuestType.Daily;

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

                                              if (args.Rank != "S" || args.Rank != "A" || args.Rank != "B")
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