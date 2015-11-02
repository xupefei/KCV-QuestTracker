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
    /// 敵東方艦隊を撃滅せよ！
    /// </summary>
    internal class Bw6 : ITracker
    {
        private int count;
        private bool isMap;

        protected virtual void OnProcessChanged(EventArgs e)
        {
            ProcessChanged?.Invoke(this, e);
        }

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 229;

        string ITracker.WikiIndex => "Bw6";

        string ITracker.Name => "敵東方艦隊を撃滅せよ！";

        QuestType ITracker.Type => QuestType.Weekly;

        public bool IsTracking { get; set; }

        public void RegisterTracker(KanColleClient client)
        {
            IsTracking = false;

            // KCV does not handle "kcsapi/api_req_map/next" API, so we can't use "kcsapi/api_req_map/next/@api_bosscell_no"
            // to determine whether a position is boss.
            client.Proxy.api_req_map_start.TryParse<kcsapi_map_start>().Subscribe(x => MapStart(x.Data));
            client.Proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>()
                  .Subscribe(x => BattleResult(x.Data));
        }

        public void ResetQuest()
        {
            count = 0;

            OnProcessChanged(new EventArgs());
        }

        public double GetPercentProcess()
        {
            return (double)count / 12 * 100;
        }

        public string GetDisplayProcess()
        {
            return count >= 12 ? "完成" : $"{count} / 12";
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
                count = 0;
            }
        }

        #endregion

        private readonly List<string> boss_names = new List<string>
                                                   {
                                                       "東方派遣艦隊", //4-1
                                                       "東方主力艦隊",
                                                       // 4-3: 東方主力艦隊
                                                       "敵東方中枢艦隊",
                                                       "リランカ島港湾守備隊",
                                                   };

        private void BattleResult(kcsapi_battleresult api)
        {
            if (!IsTracking)
                return;

            if (!isMap)
                return;

            // is boss
            if (boss_names.Contains(api.api_enemy_info.api_deck_name))
            {
                // boss & win?
                if (api.api_win_rank == "S" || api.api_win_rank == "A" || api.api_win_rank == "B")
                    if (count < 12)
                        count++;
            }

            OnProcessChanged(new EventArgs());
        }

        private void MapStart(kcsapi_map_start api)
        {
            isMap = api.api_maparea_id == 4;
        }
    }
}
