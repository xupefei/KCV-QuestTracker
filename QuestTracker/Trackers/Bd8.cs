using System;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     敵潜水艦を制圧せよ！
    /// </summary>
    internal class Bd8 : ITracker
    {
        private readonly int max_count = 6;
        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 230;

        string ITracker.WikiIndex => "Bd8";

        string ITracker.Name => "敵潜水艦を制圧せよ！";

        QuestType ITracker.Type => QuestType.Daily;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.BattleResultEvent += (sender, args) =>
                                          {
                                              if (!IsTracking)
                                                  return;

                                              foreach (var ship in args.EnemyShips)
                                              {
                                                  // 13 = SS
                                                  if (ship.Type == 13)
                                                      if (ship.MaxHp != int.MaxValue && ship.NowHp <= 0)
                                                          count += count >= max_count ? 0 : 1;
                                              }

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
            return
                $"{count}";
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