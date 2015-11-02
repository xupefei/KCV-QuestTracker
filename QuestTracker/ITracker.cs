using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.Plugins
{
    internal interface ITracker
    {
        /// <summary>
        /// api_no of the quest. api_no(あ号作戦) = 214
        /// </summary>
        int Id { get; }

        /// <summary>
        /// api_no of the quest. wiki_id(あ号作戦) = Bw1
        /// </summary>
        string WikiIndex { get; }

        /// <summary>
        /// Name of the quest
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Type of the quest
        /// </summary>
        QuestType Type { get; }

        /// <summary>
        /// Get or set whether the tracker is tracking.
        /// </summary>
        bool IsTracking { get; set; }

        /// <summary>
        /// Register the tracker to QuestManager.
        /// </summary>
        void RegisterTracker(KanColleClient client);

        /// <summary>
        /// When a quest is being reseted (e.g. Weekly task on Monday).
        /// </summary>
        void ResetQuest();

        /// <summary>
        /// Get the EventHandler indicates process change.
        /// </summary>
        /// <returns></returns>
        event EventHandler ProcessChanged;

        /// <summary>
        /// Get process.
        /// </summary>
        /// <returns></returns>
        string GetDisplayProcess();

        /// <summary>
        /// Get process, range 0.0 to 100.
        /// </summary>
        /// <returns></returns>
        double GetPercentProcess();

        /// <summary>
        /// Save process data.
        /// </summary>
        /// <returns></returns>
        string SerializeData();

        /// <summary>
        /// Resume previous process.
        /// </summary>
        void DeserializeData(string data);
    }
}
