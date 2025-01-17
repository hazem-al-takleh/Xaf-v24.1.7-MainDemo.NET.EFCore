﻿using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace MainDemo.Module.BusinessObjects;

[DefaultClassOptions]
[ModelDefault("Caption", "Task")]
[Appearance("FontColorRed", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView", Criteria = "Status=='Completed'", FontStyle = DevExpress.Drawing.DXFontStyle.Strikeout)]
[RuleCriteria("Task_Status", DefaultContexts.Save, "IIF(Status != 'NotStarted' and Status != 'Deferred', AssignedTo is not null, True)", CustomMessageTemplate = @"The task must have an assignee when its Status is ""In progress"", ""Waiting for someone else"", or ""Completed"".", SkipNullOrEmptyValues = false)]
[RuleCriteria("TaskIsNotStarted", DefaultContexts.Save, "Status != 'NotStarted'", CustomMessageTemplate = "Cannot set the task completed because it's not started.", TargetContextIDs = "MarkCompleted")]
public class DemoTask : Task {

    public virtual IList<Employee> Employees { get; set; } = new ObservableCollection<Employee>();

    [Appearance("PriorityBackColorPink", AppearanceItemType = "ViewItem", Criteria = "Priority=2", BackColor = "0xfff0f0")]
    public virtual Priority Priority { get; set; }

    [RuleValueComparison("Task_ActualWorkHours", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, 0)]
    public virtual int ActualWorkHours { get; set; }

    [RuleValueComparison("Task_EstimatedWorkHours", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, 0)]
    public virtual int EstimatedWorkHours { get; set; }

    public override String ToString() {
        return Subject;
    }

    [Action(ToolTip = "Postpone the task to the next day", ImageName = "State_Task_Deferred")]
    public void Postpone() {
        if(DueDate == DateTime.MinValue) {
            DueDate = DateTime.Now;
        }
        DueDate = DueDate + TimeSpan.FromDays(1);
    }

    #region IXafEntityObject

    public override void OnCreated() {
        Priority = Priority.Normal;
    }

    #endregion
}

[JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum Priority {
    [ImageName("State_Priority_Low")]
    Low = 0,
    [ImageName("State_Priority_Normal")]
    Normal = 1,
    [ImageName("State_Priority_High")]
    High = 2
}
