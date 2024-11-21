using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;

namespace MainDemo.Module.Win.Controllers;
public class WinTooltipController : ViewController<DevExpress.ExpressApp.ListView> {
    public WinTooltipController() {
        this.ViewControlsCreated += new System.EventHandler(this.WinTooltipController_ViewControlsCreated);
    }

    private void WinTooltipController_ViewControlsCreated(object sender, EventArgs e) {
        var listEditor = View.Editor as GridListEditor;
        if(listEditor != null) {
            foreach(GridColumn column in listEditor.GridView.Columns) {
                column.ToolTip = "Click to sort by " + column.Caption;
            }
        }
    }
}
