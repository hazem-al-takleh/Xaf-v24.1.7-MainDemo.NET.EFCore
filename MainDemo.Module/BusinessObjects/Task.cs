using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System.Text.Json.Serialization;

namespace MainDemo.Module.BusinessObjects;

[DefaultProperty(nameof(Subject))]
public class Task : BaseObject {

    public virtual DateTime? DateCompleted { get; set; }

    public virtual String Subject { get; set; }

    [FieldSize(FieldSizeAttribute.Unlimited)]
    public virtual String Description { get; set; }

    public virtual DateTime? DueDate { get; set; }

    public virtual DateTime? StartDate { get; set; }

    public virtual int PercentCompleted { get; set; }

    public virtual Employee AssignedTo { get; set; }

    private TaskStatus status;

    public virtual TaskStatus Status {
        get { return status; }
        set {
            status = value;
            if(isLoaded) {
                if(value == TaskStatus.Completed) {
                    DateCompleted = DateTime.Now;
                }
                else {
                    DateCompleted = null;
                }
            }
        }
    }

    [Action(ImageName = "State_Task_Completed", SelectionDependencyType = MethodActionSelectionDependencyType.RequireSingleObject)]
    public void MarkCompleted() {
        Status = TaskStatus.Completed;
    }

    private bool isLoaded = false;
    public override void OnLoaded() {
        isLoaded = true;
    }
}

[JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum TaskStatus {
    [ImageName("State_Task_NotStarted")]
    NotStarted,
    [ImageName("State_Task_InProgress")]
    InProgress,
    [ImageName("State_Task_WaitingForSomeoneElse")]
    WaitingForSomeoneElse,
    [ImageName("State_Task_Deferred")]
    Deferred,
    [ImageName("State_Task_Completed")]
    Completed
}
