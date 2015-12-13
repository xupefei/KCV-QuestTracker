using System;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     精鋭艦隊演習
    /// </summary>
    internal class C8 : ITracker
    {
        private readonly int max_count = 7;
        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 311;

        string ITracker.WikiIndex => "C8";

        string ITracker.Name => "精鋭艦隊演習";

        QuestType ITracker.Type => QuestType.Monthly;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.PracticeBattleResultEvent += (sender, args) =>
                                                  {
                                                      if (!IsTracking)
                                                          return;

                                                      if (!args.IsSuccess)
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