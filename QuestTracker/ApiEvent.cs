using System;
using System.Reactive.Linq;
using Amemiya.Extensions;
using Grabacr07.KanColleViewer.Plugins.Models.Raw;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;
using TaskCounter.Models;
using TaskCounter.Models.Raw;

namespace Grabacr07.KanColleViewer.Plugins
{
    internal class ApiEvent
    {
        private readonly EnemyShip[] enemyShips =
        {
            new EnemyShip(), new EnemyShip(), new EnemyShip(),
            new EnemyShip(), new EnemyShip(), new EnemyShip()
        };

        private readonly KanColleClient kanColleClient;
        private bool isFirstCombat;

        private int mapId;

        public ApiEvent(KanColleClient client)
        {
            kanColleClient = client;

            // 演习 - 战斗结束
            client.Proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_practice/battle_result")
                  .TryParse<practice_result>().Subscribe(x => PracticeBattleResult(x.Data));

            // 近代化改装
            client.Proxy.api_req_kaisou_powerup.TryParse<kcsapi_powerup>().Subscribe(x => PowerUp());

            // 改修
            client.Proxy.api_req_kousyou_remodel_slot.TryParse<kcsapi_remodel_slot>().Subscribe(x => ReModel());

            // 廃棄
            client.Proxy.api_req_kousyou_destroyitem2.TryParse<kcsapi_destroyitem2>().Subscribe(x => DestoryItem());

            // 解体
            client.Proxy.api_req_kousyou_destroyship.TryParse<kcsapi_destroyship>().Subscribe(x => DestoryShip());

            // 建造
            client.Proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => CreateShip());

            // 开发
            client.Proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => CreateItem());

            // 补给
            client.Proxy.api_req_hokyu_charge.TryParse<kcsapi_charge>().Subscribe(x => Charge());

            // 入渠
            client.Proxy.api_get_member_ndock.TryParse<kcsapi_ndock>().Subscribe(x => Ndock());

            // 遠征
            client.Proxy.api_req_mission_result.TryParse<mission_result>().Subscribe(x => MissionResult(x.Data));

            // 地图开始
            client.Proxy.api_req_map_start.TryParse<kcsapi_map_start>().Subscribe(x => MapStart(x.Data));

            // 通常 - 昼战
            client.Proxy.api_req_sortie_battle.TryParse<sortie_battle>().Subscribe(x => Battle(x.Data));

            // 通常 - 夜战
            client.Proxy.ApiSessionSource.Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_battle_midnight/battle")
                  .TryParse<battle_midnight_battle>().Subscribe(x => Battle(x.Data));

            // 通常 - 开幕夜战
            client.Proxy.ApiSessionSource.Where(
                                                x =>
                                                x.Request.PathAndQuery == "/kcsapi/api_req_battle_midnight/sp_midnight")
                  .TryParse<battle_midnight_sp_midnight>().Subscribe(x => Battle(x.Data));

            // 战斗结束
            client.Proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>()
                  .Subscribe(x => BattleResult(x.Data));
            client.Proxy.api_req_combined_battle_battleresult.TryParse<kcsapi_combined_battle_battleresult>()
                  .Subscribe(x => BattleResult(x.Data));
        }

        public event EventHandler<BattleResultEventArgs> BattleResultEvent;
        public event EventHandler<MissionResultEventArgs> MissionResultEvent;
        public event EventHandler<PracticeBattleResultEventArgs> PracticeBattleResultEvent;
        public event EventHandler NdockEvent;
        public event EventHandler ChargeEvent;
        public event EventHandler CreateItemEvent;
        public event EventHandler CreateShipEvent;
        public event EventHandler DestoryShipEvent;
        public event EventHandler DestoryItemEvent;
        public event EventHandler PowerUpEvent;
        public event EventHandler ReModelEvent;

        private void PracticeBattleResult(practice_result data)
        {
            PracticeBattleResultEvent?.Invoke(this,
                                              new PracticeBattleResultEventArgs
                                              {
                                                  IsSuccess =
                                                      data.api_win_rank == "S"
                                                      || data.api_win_rank == "A"
                                                      || data.api_win_rank == "B"
                                              });
        }

        private void ReModel()
        {
            ReModelEvent?.Invoke(this, new EventArgs());
        }

        private void PowerUp()
        {
            PowerUpEvent?.Invoke(this, new EventArgs());
        }

        private void DestoryItem()
        {
            DestoryItemEvent?.Invoke(this, new EventArgs());
        }

        private void DestoryShip()
        {
            DestoryShipEvent?.Invoke(this, new EventArgs());
        }

        private void CreateShip()
        {
            CreateShipEvent?.Invoke(this, new EventArgs());
        }

        private void CreateItem()
        {
            CreateItemEvent?.Invoke(this, new EventArgs());
        }

        private void Charge()
        {
            ChargeEvent?.Invoke(this, new EventArgs());
        }

        private void Ndock()
        {
            NdockEvent?.Invoke(this, new EventArgs());
        }

        private void MissionResult(mission_result data)
        {
            MissionResultEvent?.Invoke(this,
                                       new MissionResultEventArgs
                                       {
                                           IsSuccess = data.api_clear_result > 0,
                                           Name = data.api_quest_name
                                       });
        }

        private void MapStart(kcsapi_map_start api)
        {
            mapId = api.api_maparea_id;
            isFirstCombat = true;
        }

        private void BattleResult(kcsapi_combined_battle_battleresult data)
        {
            var arg = new BattleResultEventArgs
                      {
                          IsFirstCombat = isFirstCombat,
                          MapAreaId = mapId,
                          EnemyName = data.api_enemy_info.api_deck_name,
                          EnemyShips = enemyShips,
                          Rank = data.api_win_rank
                      };

            isFirstCombat = false;

            BattleResultEvent?.Invoke(this, arg);
        }

        private void BattleResult(kcsapi_battleresult data)
        {
            var arg = new BattleResultEventArgs
                      {
                          IsFirstCombat = isFirstCombat,
                          MapAreaId = mapId,
                          EnemyName = data.api_enemy_info.api_deck_name,
                          EnemyShips = enemyShips,
                          Rank = data.api_win_rank
                      };

            isFirstCombat = false;

            BattleResultEvent?.Invoke(this, arg);
        }

        private void Battle(battle_midnight_sp_midnight data)
        {
            ResetEnemy(data.api_ship_ke);

            UpdateEnemyMaxHP(data.api_maxhps);
            UpdateEnemyNowHP(data.api_nowhps);

            CalcEnemyDamages(data.api_hougeki.GetEnemyDamages());
        }

        private void Battle(sortie_battle data)
        {
            ResetEnemy(data.api_ship_ke);

            UpdateEnemyMaxHP(data.api_maxhps);
            UpdateEnemyNowHP(data.api_nowhps);

            CalcEnemyDamages(
                             data.api_support_info.GetEnemyDamages(),
                             data.api_kouku.GetEnemyDamages(),
                             data.api_opening_atack.GetEnemyDamages(),
                             data.api_hougeki1.GetEnemyDamages(),
                             data.api_hougeki2.GetEnemyDamages(),
                             data.api_raigeki.GetEnemyDamages()
                );
        }

        private void Battle(battle_midnight_battle data)
        {
            UpdateEnemyNowHP(data.api_nowhps);

            CalcEnemyDamages(data.api_hougeki.GetEnemyDamages());
        }

        private void ResetEnemy(int[] api_ship_ke)
        {
            api_ship_ke = api_ship_ke.Slice(1, 7);

            for (var i = 0; i < api_ship_ke.Length; i++)
            {
                var id = api_ship_ke[i];

                enemyShips[i].Id = id;
                enemyShips[i].Type = id == -1 ? 0 : kanColleClient.Master.Ships[id].ShipType.Id;
            }
        }

        private void UpdateEnemyMaxHP(int[] api_nowhps)
        {
            api_nowhps = api_nowhps.Slice(7, 13);

            for (var i = 0; i < api_nowhps.Length; i++)
            {
                var hp = api_nowhps[i] == -1 ? int.MaxValue : api_nowhps[i];
                enemyShips[i].MaxHp = hp;
            }
        }

        private void UpdateEnemyNowHP(int[] api_nowhps)
        {
            api_nowhps = api_nowhps.Slice(7, 13);

            for (var i = 0; i < api_nowhps.Length; i++)
            {
                var hp = api_nowhps[i] == -1 ? int.MaxValue : api_nowhps[i];
                enemyShips[i].NowHp = hp;
            }
        }

        private void CalcEnemyDamages(params FleetDamages[] damages)
        {
            foreach (var damage in damages)
            {
                enemyShips[0].NowHp -= damage.Ship1;
                enemyShips[1].NowHp -= damage.Ship2;
                enemyShips[2].NowHp -= damage.Ship3;
                enemyShips[3].NowHp -= damage.Ship4;
                enemyShips[4].NowHp -= damage.Ship5;
                enemyShips[5].NowHp -= damage.Ship6;
            }
        }
    }
}