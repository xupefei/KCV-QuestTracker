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
    /// 敵艦隊を10回邀撃せよ！
    /// </summary>
    internal class Bd3 : ITracker
    {
        private int count;

        protected virtual void OnProcessChanged(EventArgs e)
        {
            ProcessChanged?.Invoke(this, e);
        }

        #region ITracker
        
        public event EventHandler ProcessChanged;

        int ITracker.Id => 210;

        string ITracker.WikiIndex => "Bd3";

        string ITracker.Name => "敵艦隊を10回邀撃せよ！";

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
            count = 0;

            OnProcessChanged(new EventArgs());
        }

        public double GetPercentProcess()
        {
            return (double)count / 10 * 100;
        }

        public string GetDisplayProcess()
        {
            return count >= 10 ? "完成" : $"{count} / 10";
        }

        public string SerializeData()
        {
            return $"{count}";
        }

        public void DeserializeData(string data)
        {
            try
            {
                count = Int32.Parse(data);
            }
            catch
            {
                count=0;
            }
        }

        #endregion

        private void BattleResult(kcsapi_battleresult api)
        {
            if (!IsTracking)
                return;

            if (count < 10)
                count++;

            OnProcessChanged(new EventArgs());
        }

        private void CombinedBattleResult(kcsapi_combined_battle_battleresult api)
        {
            if (!IsTracking)
                return;

            if (count < 10)
                count++;

            OnProcessChanged(new EventArgs());
        }
    }
}
