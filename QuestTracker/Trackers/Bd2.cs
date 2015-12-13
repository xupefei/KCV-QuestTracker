using System;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     敵艦隊主力を撃滅せよ！
    /// </summary>
    internal class Bd2 : ITracker
    {
        private bool finished;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 216;

        string ITracker.WikiIndex => "Bd2";

        string ITracker.Name => "敵艦隊主力を撃滅せよ！";

        QuestType ITracker.Type => QuestType.Daily;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.BattleResultEvent += (sender, args) =>
                                          {
                                              if (!IsTracking)
                                                  return;

                                              finished = true;

                                              ProcessChanged?.Invoke(this, new EventArgs());
                                          };
        }

        public void ResetQuest()
        {
            finished = false;

            ProcessChanged?.Invoke(this, new EventArgs());
        }

        public double GetPercentProcess()
        {
            return finished ? 100d : 0d;
        }

        public string GetDisplayProcess()
        {
            return finished ? "完成" : "0 / 1";
        }

        public string SerializeData()
        {
            return
                $"{finished}";
        }

        public void DeserializeData(string data)
        {
            try
            {
                finished = bool.Parse(data);
            }
            catch
            {
                finished = false;
            }
        }

        #endregion
    }
}