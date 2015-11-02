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
    /// あ号作戦
    /// </summary>
    internal class Bw1 : ITracker
    {
        private int process_combat;
        private int process_combat_s;
        private int process_boss;
        private int process_boss_win;

        private bool is_first_combat;

        protected virtual void OnProcessChanged(EventArgs e)
        {
            ProcessChanged?.Invoke(this, e);
        }

        #region ITracker
        
        public event EventHandler ProcessChanged;

        int ITracker.Id => 214;

        string ITracker.WikiIndex => "Bw1";

        string ITracker.Name => "あ号作戦";

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
            client.Proxy.api_req_combined_battle_battleresult.TryParse<kcsapi_combined_battle_battleresult>()
                  .Subscribe(x => CombinedBattleResult(x.Data));
        }

        public void ResetQuest()
        {
            process_combat = process_combat_s = process_boss = process_boss_win = 0;

            OnProcessChanged(new EventArgs());
        }

        public double GetPercentProcess()
        {
            return (double)
                   (process_combat + process_combat_s + process_boss + process_boss_win) / (36 + 6 + 24 + 12) * 100;
        }

        public string GetDisplayProcess()
        {
            if (process_combat >= 36 && process_combat_s >= 6 && process_boss >= 24 && process_boss_win >= 12)
                return "完成";

            return
                $"出撃 {process_combat}/36, S勝 {process_combat_s}/6," +
                $" ボス {process_boss}/24, ボス勝 {process_boss_win}/12";
        }

        public string SerializeData()
        {
            return
                $"{process_combat},{process_combat_s},{process_boss},{process_boss_win}";
        }

        public void DeserializeData(string data)
        {
            try
            {
                data.Split(',').Into(e => process_combat = Int32.Parse(e),
                                     e => process_combat_s = Int32.Parse(e),
                                     e => process_boss = Int32.Parse(e),
                                     e => process_boss_win = Int32.Parse(e)
                    );
            }
            catch
            {
                process_combat = process_combat_s = process_boss = process_boss_win = 0;
            }
        }

        #endregion

        private readonly List<string> boss_names = new List<string>
                                                   {
                                                       "敵主力艦隊", //1-1
                                                       "敵主力部隊",
                                                       // 1-3: 敵主力艦隊
                                                       "敵機動部隊",
                                                       "敵通商破壊主力艦隊",
                                                       // 2-1: 敵主力艦隊
                                                       "敵通商破壊艦隊", //2-2
                                                       "敵主力打撃群",
                                                       "敵侵攻中核艦隊",
                                                       // 2-5: 敵主力艦隊
                                                       "敵北方侵攻艦隊", //3-1
                                                       "敵キス島包囲艦隊",
                                                       "深海棲艦泊地艦隊",
                                                       "深海棲艦北方艦隊中枢",
                                                       "北方増援部隊主力",
                                                       "東方派遣艦隊", //4-1
                                                       "東方主力艦隊",
                                                       // 4-3: 東方主力艦隊
                                                       "敵東方中枢艦隊",
                                                       "リランカ島港湾守備隊",
                                                       "敵前線司令艦隊", //5-1
                                                       "敵機動部隊本隊",
                                                       "敵サーモン方面主力艦隊",
                                                       "敵補給部隊本体",
                                                       "敵任務部隊本隊",
                                                       "敵回航中空母", //6-1
                                                       "敵攻略部隊本体",
                                                       "留守泊地旗艦艦隊"
                                                   };

        private void BattleResult(kcsapi_battleresult api)
        {
            CheckResult(api.api_enemy_info.api_deck_name, api.api_win_rank);
        }

        private void CombinedBattleResult(kcsapi_combined_battle_battleresult api)
        {
            CheckResult(api.api_enemy_info.api_deck_name, api.api_win_rank);
        }

        private void MapStart(kcsapi_map_start api)
        {
            is_first_combat = true;
        }

        private void CheckResult(string api_deck_name, string api_win_rank)
        {
            if (!IsTracking)
                return;

            if (is_first_combat)
            {
                if (process_combat < 36)
                    process_combat++;

                is_first_combat = false;
            }

            // win?
            if (api_win_rank == "S")
                if (process_combat_s < 6)
                    process_combat_s++;

            // is boss
            if (boss_names.Contains(api_deck_name))
            {
                if (process_boss < 24)
                    process_boss++;

                // boss & win?
                if (api_win_rank == "S" || api_win_rank == "A" || api_win_rank == "B")
                    if (process_boss_win < 12)
                        process_boss_win++;
            }

            OnProcessChanged(new EventArgs());
        }
    }
}
