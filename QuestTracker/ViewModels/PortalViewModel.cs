using System.Collections.ObjectModel;
using System.Linq;
using Grabacr07.KanColleWrapper;
using Livet;

namespace Grabacr07.KanColleViewer.Plugins.ViewModels
{
    public class PortalViewModel : ViewModel
    {
        private readonly QuestManager questManager;

        private ObservableCollection<QuestProcessCollectionItem> questProcessCollection =
            new ObservableCollection<QuestProcessCollectionItem>();

        public PortalViewModel(KanColleClient client)
        {
            questManager = new QuestManager(client);

            questManager
                .QuestsEventChanged += (sender, e) =>
                                       {
                                           var newQC =
                                               new ObservableCollection<QuestProcessCollectionItem>();

                                           questManager
                                               .TrackingProcessStrings
                                               .ToList()
                                               .ForEach(qpc =>
                                                        {
                                                            newQC.Add(new QuestProcessCollectionItem
                                                                      {
                                                                          Id = qpc.Id,
                                                                          WikiIndex = qpc.WikiIndex,
                                                                          Name = qpc.Name,
                                                                          ProcessPercent = qpc.ProcessPercent,
                                                                          ProcessText = qpc.ProcessText
                                                                      });
                                                        });

                                           QuestProcessCollection = newQC;
                                       };
        }

        public ObservableCollection<QuestProcessCollectionItem> QuestProcessCollection
        {
            get { return questProcessCollection; }
            set
            {
                if (questProcessCollection != value)
                {
                    questProcessCollection = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}