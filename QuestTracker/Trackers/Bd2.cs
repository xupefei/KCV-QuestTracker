using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amemiya.Extensions;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    /// 敵艦隊主力を撃滅せよ！
    /// </summary>
    internal class Bd2 : ITracker
    {
        private bool finished;

        protected virtual void OnProcessChanged(EventArgs e)
        {
            ProcessChanged?.Invoke(this, e);
        }

        #region ITracker
        
        public event EventHandler ProcessChanged;

        int ITracker.Id => 216;

        string ITracker.WikiIndex => "Bd2";

        string ITracker.Name => "敵艦隊主力を撃滅せよ！";

        QuestType ITracker.Type => QuestType.Daily;

        public bool IsTracking { get; set; }

        public void RegisterTracker(KanColleClient client)
        {
            client.Proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>()
                  .Subscribe(x => BattleResult(x.Data));
            client.Proxy.api_req_combined_battle_battleresult.TryParse<kcsapi_combined_battle_battleresult>()
                  .Subscribe(x => CombinedBattleResult(x.Data));
        }

        public void ResetQuest()
        {
            finished = false;

            OnProcessChanged(new EventArgs());
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
                finished = Boolean.Parse(data);
            }
            catch
            {
                finished = false;
            }
        }

        #endregion

        private void BattleResult(kcsapi_battleresult api)
        {
            if (!IsTracking)
                return;

            finished = true;

            OnProcessChanged(new EventArgs());
        }

        private void CombinedBattleResult(kcsapi_combined_battle_battleresult api)
        {
            if (!IsTracking)
                return;

            finished = true;

            OnProcessChanged(new EventArgs());
        }
    }
}
