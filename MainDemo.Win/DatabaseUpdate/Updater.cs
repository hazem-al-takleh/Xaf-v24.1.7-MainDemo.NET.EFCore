using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.PivotChart.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using MainDemo.Module.BusinessObjects;


namespace MainDemo.Win.DatabaseUpdate;
abstract class TaskAnalysis1LayoutUpdaterBase {
    protected IObjectSpace objectSpace;
    protected abstract IAnalysisControl CreateAnalysisControl();
    protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control);
    private static void SetFieldArea(IAnalysisControl control, string fieldName, DevExpress.XtraPivotGrid.PivotArea fieldArea) {
        if(control.Fields[fieldName] == null) {
            throw new ArgumentNullException("control.Fields['" + fieldName + "']");
        }
        control.Fields[fieldName].Area = fieldArea;
    }
    public TaskAnalysis1LayoutUpdaterBase(IObjectSpace objectSpace) {
        this.objectSpace = objectSpace;
    }
    public void Update(Analysis analysis) {
        if(analysis != null && !PivotGridSettingsHelper.HasPivotGridSettings(analysis)) {
            IAnalysisControl control = CreateAnalysisControl();
            control.DataSource = new AnalysisDataSource(analysis, objectSpace.GetObjects(typeof(DemoTask)));
            if(control.Fields.Count > 0) {
                SetFieldArea(control, "Priority", DevExpress.XtraPivotGrid.PivotArea.ColumnArea);
                SetFieldArea(control, "Subject", DevExpress.XtraPivotGrid.PivotArea.DataArea);
                SetFieldArea(control, "AssignedTo.FullName", DevExpress.XtraPivotGrid.PivotArea.RowArea);
                PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(control), analysis);
            }
            objectSpace.CommitChanges();
        }
    }
}
class TaskAnalysis1LayoutUpdater : TaskAnalysis1LayoutUpdaterBase {
    protected override IAnalysisControl CreateAnalysisControl() {
        return new AnalysisControlWin();
    }
    protected override DevExpress.Persistent.Base.IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control) {
        return new PivotGridControlSettingsStore(((AnalysisControlWin)control).PivotGrid);
    }
    public TaskAnalysis1LayoutUpdater(IObjectSpace objectSpace)
        : base(objectSpace) {
    }
}
abstract class TaskAnalysis2LayoutUpdaterBase {
    protected IObjectSpace objectSpace;
    protected abstract IAnalysisControl CreateAnalysisControl();
    protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control);
    public TaskAnalysis2LayoutUpdaterBase(IObjectSpace objectSpace) {
        this.objectSpace = objectSpace;
    }
    public void Update(Analysis analysis) {
        if(analysis != null && !PivotGridSettingsHelper.HasPivotGridSettings(analysis)) {
            IAnalysisControl control = CreateAnalysisControl();
            control.DataSource = new AnalysisDataSource(analysis, objectSpace.GetObjects(typeof(DemoTask)));
            if(control.Fields.Count > 0) {
                control.Fields[nameof(DemoTask.Status)].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                control.Fields[nameof(DemoTask.Priority)].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                control.Fields[nameof(DemoTask.EstimatedWorkHours)].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                control.Fields[nameof(DemoTask.ActualWorkHours)].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                control.Fields["AssignedTo.FullName"].Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
            }
            PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(control), analysis);
            objectSpace.CommitChanges();
        }
    }
}
class TaskAnalysis2LayoutUpdater : TaskAnalysis2LayoutUpdaterBase {
    protected override IAnalysisControl CreateAnalysisControl() {
        return new AnalysisControlWin();
    }
    protected override DevExpress.Persistent.Base.IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control) {
        return new PivotGridControlSettingsStore(((AnalysisControlWin)control).PivotGrid);
    }
    public TaskAnalysis2LayoutUpdater(IObjectSpace objectSpace)
        : base(objectSpace) {
    }
}
