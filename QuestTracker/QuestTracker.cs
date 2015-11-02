using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Plugins.ViewModels;
using Grabacr07.KanColleViewer.Plugins.Views;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.Plugins
{
    [Export(typeof(IPlugin))]
    [Export(typeof(ITool))]
    [ExportMetadata("Guid", "73584674-DC71-4CE2-AEF6-29F2DF8A41E6")]
    [ExportMetadata("Title", "QuestTracker")]
    [ExportMetadata("Description", "任務の進行度を追跡する。")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "+PaddyXu")]
	public class QuestTracker : IPlugin, ITool
	{
		private PortalViewModel viewerViewModel;

        string ITool.Name => "QuestTrack";

        object ITool.View => new Portal { DataContext = this.viewerViewModel };
        
        public void Initialize()
        {
            this.viewerViewModel = new PortalViewModel(KanColleClient.Current);
        }
	}
}
