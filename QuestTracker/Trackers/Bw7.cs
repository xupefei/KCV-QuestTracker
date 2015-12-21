using System;
using System.Collections.Generic;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     敵北方艦隊主力を撃滅せよ！
    /// </summary>
    internal class Bw7 : ITracker
    {
        private readonly List<string> boss_names = new List<string>
                                                   {
                                                       "深海棲艦泊地艦隊",
                                                       "深海棲艦北方艦隊中枢",
                                                       "北方増援部隊主力"
                                                   };

        private readonly int map_id = 3;
        private readonly int max_count = 5;

        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 241;

        string ITracker.WikiIndex => "Bw7";

        string ITracker.Name => "敵北方艦隊主力を撃滅せよ！";

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

                                              if (args.Rank != "S" && args.Rank != "A" && args.Rank != "B")
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
            return (double)count / 5 * 100;
        }

        public string GetDisplayProcess()
        {
            return count >= 5 ? "完成" : $"{count} / 5";
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