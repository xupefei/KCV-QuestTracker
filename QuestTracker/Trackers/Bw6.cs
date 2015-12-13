using System;
using System.Collections.Generic;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     敵東方艦隊を撃滅せよ！
    /// </summary>
    internal class Bw6 : ITracker
    {
        private readonly List<string> boss_names = new List<string>
                                                   {
                                                       "東方派遣艦隊", //4-1
                                                       "東方主力艦隊",
                                                       // 4-3: 東方主力艦隊
                                                       "敵東方中枢艦隊",
                                                       "リランカ島港湾守備隊"
                                                   };

        private readonly int map_id = 4;
        private readonly int max_count = 12;

        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 229;

        string ITracker.WikiIndex => "Bw6";

        string ITracker.Name => "敵東方艦隊を撃滅せよ！";

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
            return (double)count / 12 * 100;
        }

        public string GetDisplayProcess()
        {
            return count >= 12 ? "完成" : $"{count} / 12";
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