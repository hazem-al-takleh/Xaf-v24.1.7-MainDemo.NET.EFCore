using System.Text.Json.Serialization;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;

namespace MainDemo.Module.BusinessObjects;

[ImageName("BO_FileAttachment")]
public class PortfolioFileData : BaseObject {

    [ExpandObjectMembers(ExpandObjectMembers.Never), RuleRequiredField("PortfolioFileDataRule", "Save", "File should be assigned")]
    public virtual FileData File { get; set; }

    public virtual Resume Resume { get; set; }

    public virtual DocumentType DocumentType { get; set; }

    public override void OnCreated() {
        DocumentType = DocumentType.Unknown;
    }
}

[JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum DocumentType {
    SourceCode = 1,
    Tests = 2,
    Documentation = 3,
    Diagrams = 4,
    Screenshots = 5,
    Unknown = 6
}
