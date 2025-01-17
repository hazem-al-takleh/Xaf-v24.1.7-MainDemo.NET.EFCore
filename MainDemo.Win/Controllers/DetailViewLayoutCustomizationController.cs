﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraLayout;

namespace MainDemo.Win.Controllers;
public class DetailViewLayoutCustomizationController : ViewController<DetailView> {
    WinLayoutManager layoutManager;

    protected override void OnActivated() {
        base.OnActivated();
        layoutManager = View.LayoutManager as WinLayoutManager;
        if(layoutManager != null) {
            layoutManager.ItemCreated += LayoutManager_ItemCreated;
        }
    }
    protected override void OnDeactivated() {
        if(layoutManager != null) {
            layoutManager.ItemCreated -= LayoutManager_ItemCreated;
        }
        base.OnDeactivated();
    }
    private void LayoutManager_ItemCreated(object sender, ItemCreatedEventArgs e) {
        var group = e.Item as LayoutControlGroup;
        if(group != null && group.GroupStyle == DevExpress.Utils.GroupStyle.Title) {
            group.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, group.Spacing.Bottom);
        }
    }
}
