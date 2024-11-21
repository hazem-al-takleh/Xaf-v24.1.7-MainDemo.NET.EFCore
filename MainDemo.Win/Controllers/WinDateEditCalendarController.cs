using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors;
using MainDemo.Module.BusinessObjects;

namespace MainDemo.Win.Controllers;
public class WinDateEditCalendarController : ObjectViewController<DetailView, Employee> {
    protected override void OnActivated() {
        base.OnActivated();
        View.CustomizeViewItemControl(this, SetCalendarView, nameof(Employee.Birthday));
    }
    private void SetCalendarView(ViewItem viewItem) {
        DateEdit dateEdit = (DateEdit)viewItem.Control;
        dateEdit.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.TouchUI;
    }
}
