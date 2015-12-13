using System;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     艦娘「建造」艦隊強化！
    /// </summary>
    internal class F8 : ITracker
    {
        private readonly int max_count = 3;
        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 608;

        string ITracker.WikiIndex => "F8";

        string ITracker.Name => "艦娘「建造」艦隊強化！";

        QuestType ITracker.Type => QuestType.Daily;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.CreateShipEvent += (sender, args) =>
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