namespace TaskCounter.Models.Raw
{
    public class practice_result
    {
        public int[] api_ship_id { get; set; }

        public string api_win_rank { get; set; }

        public int api_get_exp { get; set; }

        public int api_member_lv { get; set; }

        public int api_member_exp { get; set; }

        public int api_get_base_exp { get; set; }

        public int api_mvp { get; set; }

        public int[] api_get_ship_exp { get; set; }

        public int[][] api_get_exp_lvup { get; set; }

        public int api_dests { get; set; }

        public int api_destsf { get; set; }

        public Api_Enemy_Info api_enemy_info { get; set; }
    }

    public class Api_Enemy_Info
    {
        public string api_user_name { get; set; }

        public int api_level { get; set; }

        public string api_rank { get; set; }

        public string api_deck_name { get; set; }
    }
}