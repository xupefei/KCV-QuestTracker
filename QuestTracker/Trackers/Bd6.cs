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
    /// 敵輸送船団を叩け！
    /// </summary>
    internal class Bd6 : ITracker
    {
        private int count;

        private KanColleClient kanColleClient;

        protected virtual void OnProcessChanged(EventArgs e)
        {
            ProcessChanged?.Invoke(this, e);
        }

        #region ITracker
        
        public event EventHandler ProcessChanged;

        int ITracker.Id => 212;

        string ITracker.WikiIndex => "Bd6";

        string ITracker.Name => "敵輸送船団を叩け！";

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
            return (double)count / 5 * 100;
        }

        public string GetDisplayProcess()
        {
            return count >= 5 ? "完成" : $"{count} / 5";
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
                // 15 = AP
                if (kanColleClient.Master.Ships[id].ShipType.Id == 15)
                    if (count < 5)
                        count++;
            }
        }

    }
}
