using System;
using System.Collections.Generic;
using Amemiya.Extensions;

namespace Grabacr07.KanColleViewer.Plugins.Trackers
{
    /// <summary>
    ///     あ号作戦
    /// </summary>
    internal class Bw1 : ITracker
    {
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

        private int process_boss;
        private int process_boss_win;
        private int process_combat;
        private int process_combat_s;

        #region ITracker

        public event EventHandler ProcessChanged;

        int ITracker.Id => 214;

        string ITracker.WikiIndex => "Bw1";

        string ITracker.Name => "あ号作戦";

        QuestType ITracker.Type => QuestType.Weekly;

        public bool IsTracking { get; set; }

        public void RegisterEvent(ApiEvent apiEvent)
        {
            apiEvent.BattleResultEvent += (sender, args) =>
                                          {
                                              if (!IsTracking)
                                                  return;

                                              if (args.IsFirstCombat)
                                              {
                                                  if (process_combat < 36)
                                                      process_combat++;
                                              }

                                              // S win?
                                              if (args.Rank == "S")
                                                  if (process_combat_s < 6)
                                                      process_combat_s++;

                                              // is boss
                                              if (boss_names.Contains(args.EnemyName))
                                              {
                                                  if (process_boss < 24)
                                                      process_boss++;

                                                  // boss & win?
                                                  if (args.Rank == "S" || args.Rank == "A" || args.Rank == "B")
                                                      if (process_boss_win < 12)
                                                          process_boss_win++;
                                              }

                                              ProcessChanged?.Invoke(this, new EventArgs());
                                          };
        }

        public void ResetQuest()
        {
            process_combat = process_combat_s = process_boss = process_boss_win = 0;

            ProcessChanged?.Invoke(this, new EventArgs());
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
                data.Split(',').Into(e => process_combat = int.Parse(e),
                                     e => process_combat_s = int.Parse(e),
                                     e => process_boss = int.Parse(e),
                                     e => process_boss_win = int.Parse(e)
                    );
            }
            catch
            {
                process_combat = process_combat_s = process_boss = process_boss_win = 0;
            }
        }

        #endregion
    }
}