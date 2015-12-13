using System;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     ろ号作戦
    /// </summary>
    internal class Bw4 : ITracker
    {
        private readonly int max_count = 50;
        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 221;

        string ITracker.WikiIndex => "Bw4";

        string ITracker.Name => "ろ号作戦";

        QuestType ITracker.Type => QuestType.Weekly;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.BattleResultEvent += (sender, args) =>
                                          {
                                              if (!IsTracking)
                                                  return;

                                              foreach (var ship in args.EnemyShips)
                                              {
                                                  // 15 = AP
                                                  if (ship.Type == 15)
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