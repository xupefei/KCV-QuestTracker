using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Amemiya.Extensions;

namespace Grabacr07.KanColleViewer.Plugins
{
    public struct QuestItem
    {
        public int Id;
        public QuestType Type;
        public string Name;
        public bool IsTracking;
        public DateTime TrackingStart;
        public string ProcessStorageString;

        public QuestItem(int id, QuestType type)
            : this(id, type, String.Empty, false, DateTime.MinValue, string.Empty)
        {
        }

        public QuestItem(int id, QuestType type, bool isTracking, DateTime time)
            : this(id, type, String.Empty, isTracking, time, string.Empty)
        {
        }

        public QuestItem(int id,
                         QuestType type,
                         string name,
                         bool isTracking,
                         DateTime start,
                         string processStorage)
        {
            Id = id;
            Name = name;
            IsTracking = isTracking;
            Type = type;
            TrackingStart = start;
            ProcessStorageString = processStorage;
        }

        public string Serialize()
        {
            return
                $"{Id}###{Name}###{(int)Type}###{IsTracking}###" +
                $"{TrackingStart.ToString("O")}###{ProcessStorageString}";
        }

        public static QuestItem Deserialize(string data)
        {
            QuestItem result = new QuestItem();

            try
            {
                data.Split(new[] {"###"}, StringSplitOptions.None).Into(i => result.Id = Int32.Parse(i),
                                                                        i => result.Name = i,
                                                                        i => result.Type = (QuestType)Int32.Parse(i),
                                                                        i => result.IsTracking = Boolean.Parse(i),
                                                                        i => result.TrackingStart = DateTime.Parse(i),
                                                                        i => result.ProcessStorageString = i
                    );
            }
            catch
            {
                // ignored
            }
            return result;
        }
    }
}
