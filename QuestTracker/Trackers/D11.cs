using System;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     南方への鼠輸送を継続実施せよ!
    /// </summary>
    internal class D11 : ITracker
    {
        private readonly int max_count = 6;
        private int count;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 411;

        string ITracker.WikiIndex => "D11";

        string ITracker.Name => "南方への鼠輸送を継続実施せよ!";

        QuestType ITracker.Type => QuestType.Weekly;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.MissionResultEvent += (sender, args) =>
                                           {
                                               if (!IsTracking)
                                                   return;

                                               if (!args.Name.Contains("東京急行"))
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