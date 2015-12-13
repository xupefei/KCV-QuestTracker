using System;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     資源の再利用
    /// </summary>
    internal class F12 : ITracker
    {
        private readonly int max_count = 24;
        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 613;

        string ITracker.WikiIndex => "F12";

        string ITracker.Name => "資源の再利用";

        QuestType ITracker.Type => QuestType.Weekly;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.DestoryItemEvent += (sender, args) =>
                                         {
                                             if (!IsTracking)
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