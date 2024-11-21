using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.EF;
using MainDemo.Win.DatabaseUpdate;

namespace MainDemo.Win.Controllers {
    public class CreatePivotGridSettingsController : ObjectViewController<DetailView, Analysis> {
        protected override void OnActivated() {
            base.OnActivated();
            if (ViewCurrentObject.Name == "Completed tasks") {
                new TaskAnalysis1LayoutUpdater(ObjectSpace).Update(ViewCurrentObject);
            }
            if(ViewCurrentObject.Name == "Estimated and actual work comparison") {
                new TaskAnalysis2LayoutUpdater(ObjectSpace).Update(ViewCurrentObject);
            }
        }
    }
}
