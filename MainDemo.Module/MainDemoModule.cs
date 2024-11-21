﻿using Demos.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;
using MainDemo.Module.BusinessObjects;
using MainDemo.Module.BusinessObjects.NonPersistent;
using MainDemo.Module.CodeRules;
using MainDemo.Module.Reports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MainDemo.Module;
public sealed class MainDemoModule : ModuleBase {
    public MainDemoModule() {
        DevExpress.ExpressApp.Security.SecurityModule.UsedExportedTypes = DevExpress.Persistent.Base.UsedExportedTypes.Custom;

        this.Description = "MainDemo module";

        this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.PermissionPolicy.PermissionPolicyRole));
        this.AdditionalExportedTypes.Add(typeof(ApplicationUser));
        this.AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.ReportDataV2));
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));
        this.RequiredModuleTypes.Add(typeof(ValidationModule));
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ReportsV2.ReportsModuleV2));
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.PivotChartModuleBase));
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Office.OfficeModule));

        this.AdditionalExportedTypes.Add(typeof(CustomNonPersistentObject));
        this.AdditionalExportedTypes.Add(typeof(UseSQLAlternativeInfo));
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
    }
    public override void Setup(ApplicationModulesManager moduleManager) {
        base.Setup(moduleManager);
        ReportsModuleV2 reportModule = moduleManager.Modules.FindModule<ReportsModuleV2>();
        reportModule.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.EF.ReportDataV2);
        ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleMemberPermissionsCriteria), typeof(IRuleBaseProperties));
        ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleObjectPermissionsCriteria), typeof(IRuleBaseProperties));
    }
    public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
        base.CustomizeTypesInfo(typesInfo);
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        List<ModuleUpdater> moduleUpdaters = new List<ModuleUpdater>();
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        moduleUpdaters.Add(updater);
        PredefinedReportsUpdater predefinedReportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
        predefinedReportsUpdater.AddPredefinedReport<EmployeeListReport>("Employee List Report", typeof(Employee), true);
        moduleUpdaters.Add(predefinedReportsUpdater);
        return moduleUpdaters;
    }

    protected override IEnumerable<Type> GetDeclaredExportedTypes() {
        return new Type[] {
                typeof(Address),
                typeof(Country),
                typeof(DevExpress.Persistent.BaseImpl.EF.Event),
                typeof(DevExpress.Persistent.BaseImpl.EF.ReportDataV2),
                typeof(DevExpress.Persistent.BaseImpl.EF.Analysis),
                typeof(Note),
                typeof(Employee),
                typeof(DemoTask),
                typeof(Department),
                typeof(Location),
                typeof(Paycheck),
                typeof(PhoneNumber),
                typeof(PortfolioFileData),
                typeof(Position),
                typeof(Resume)
            };
    }

    public override IList<PopupWindowShowAction> GetStartupActions() {
        if(!IsSiteMode && !DemoDbEngineDetectorHelper.IsSqlServerAccessible()) {
            IList<PopupWindowShowAction> startupActions = base.GetStartupActions();
            PopupWindowShowAction showUseSQLAlternativeInfoAction = new PopupWindowShowAction();
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(UseSQLAlternativeInfo));
            UseSQLAlternativeInfo useSqlAlternativeInfo = objectSpace.GetObject<UseSQLAlternativeInfo>(UseSQLAlternativeInfoSingleton.Instance.Info);
            showUseSQLAlternativeInfoAction.CustomizePopupWindowParams += delegate (Object sender, CustomizePopupWindowParamsEventArgs e) {
                e.View = Application.CreateDetailView(objectSpace, useSqlAlternativeInfo, true);
                e.DialogController.CancelAction.Active["Required"] = false;
                e.IsSizeable = false;
            };
            startupActions.Add(showUseSQLAlternativeInfoAction);
            return startupActions;
        }
        else {
            return base.GetStartupActions();
        }
    }

    static MainDemoModule() {
        ResetViewSettingsController.DefaultAllowRecreateView = false;
    }
    private static bool? isSiteMode;
    public bool IsSiteMode {
        get {
            if(isSiteMode == null) {
                string siteMode;
                var config = Application.ServiceProvider.GetService<IConfiguration>();
                if(config != null) {
                    siteMode = config["SiteMode"];
                }
                else {
                    siteMode = System.Configuration.ConfigurationManager.AppSettings["SiteMode"];
                }
                isSiteMode = ((siteMode != null) && (siteMode.ToLower() == "true"));
            }
            return isSiteMode.Value;
        }
    }
}
