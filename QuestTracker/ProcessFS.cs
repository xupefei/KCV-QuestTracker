using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace Grabacr07.KanColleViewer.Plugins
{
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    internal class QuestFS
    {
        public static string StoragePath =
            Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                         "QuestStorage",
                         "TrackingQuests.xml");

        public static QuestItem[] GetQuests()
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(StoragePath)))
                    return new QuestItem[0];

                if (!File.Exists(StoragePath))
                    return new QuestItem[0];

                var items = new List<QuestItem>();

                XElement.Load(StoragePath)
                        .Descendants("Quest")
                        .ToList()
                        .ForEach(e => items.Add(QuestItem.Deserialize(e.Value)));

                return items.ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show($"QuestTracker error: \r\n{e}");

                return new QuestItem[0];
            }
        }

        public static void SaveQuests(QuestItem[] items)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(StoragePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(StoragePath));

                var root = new XElement("Quests");

                foreach (var item in items)
                {
                    root.Add(new XElement("Quest",
                                          new XAttribute("Id", item.Id),
                                          item.Serialize()));
                }

                root.Save(StoragePath);
            }
            catch (Exception e)
            {
                MessageBox.Show($"QuestTracker error: \r\n{e}");
            }
        }
    }
}