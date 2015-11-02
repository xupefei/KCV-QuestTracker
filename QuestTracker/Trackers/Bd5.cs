using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amemiya.Extensions;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    /// 敵補給艦を3隻撃沈せよ！
    /// </summary>
    internal class Bd5 : ITracker
    {
        private int count;

        private KanColleClient kanColleClient;

        protected virtual void OnProcessChanged(EventArgs e)
        {
            ProcessChanged?.Invoke(this, e);
        }

        #region ITracker
        
        public event EventHandler ProcessChanged;

        int ITracker.Id => 218;

        string ITracker.WikiIndex => "Bd5";

        string ITracker.Name => "敵補給艦を3隻撃沈せよ！";

        QuestType ITracker.Type => QuestType.Daily;

        public bool IsTracking { get; set; }

        public void RegisterTracker(KanColleClient client)
        {
            kanColleClient = client;

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
            return (double)count / 3 * 100;
        }

        public string GetDisplayProcess()
        {
            return count >= 3 ? "完成" : $"{count} / 3";
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
                count = Int32.Parse(data);
            }
            catch
            {
                count = 0;
            }
        }

        #endregion

        private void BattleResult(kcsapi_battleresult api)
        {
            CheckShipCount(api.api_ship_id);

            OnProcessChanged(new EventArgs());
        }

        private void CombinedBattleResult(kcsapi_combined_battle_battleresult api)
        {
            CheckShipCount(api.api_ship_id);

            OnProcessChanged(new EventArgs());
        }

        private void CheckShipCount(int[] api_ship_id)
        {
            if (!IsTracking)
                return;

            foreach (int id in api_ship_id)
            {
                if (id == -1)
                    continue;

                // 15 = AP
                if (kanColleClient.Master.Ships[id].ShipType.Id == 15)
                    if (count < 3)
                        count++;
            }
        }

    }
}
