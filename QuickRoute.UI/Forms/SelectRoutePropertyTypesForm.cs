using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.RouteProperties;
using QuickRoute.Resources;
using QuickRoute.UI.Classes;

namespace QuickRoute.UI.Forms
{
  public partial class SelectRoutePropertyTypesForm : Form
  {
    public SelectRoutePropertyTypesForm(SelectableRoutePropertyTypeCollection routePropertyTypes)
    {
      InitializeComponent();

      // add the names and visibility status of the route properties
      foreach (var item in routePropertyTypes)
      {
        routePropertyTypeCheckboxList.Items.Add(item, item.Selected);
      }
      UpdateUI();
    }

    public SelectableRoutePropertyTypeCollection RoutePropertyTypes
    {
      get
      {
        var routePropertyTypes = new SelectableRoutePropertyTypeCollection();
        for(var i=0; i<routePropertyTypeCheckboxList.Items.Count; i++)
        {
          var item = (SelectableRoutePropertyType) routePropertyTypeCheckboxList.Items[i];
          routePropertyTypes.Add(new SelectableRoutePropertyType(item.RoutePropertyType, routePropertyTypeCheckboxList.GetItemChecked(i)));
        }
        return routePropertyTypes;
      }
    }

    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = System.Windows.Forms.DialogResult.OK;
      Close();
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = System.Windows.Forms.DialogResult.Cancel;
      Close();
    }

    private void moveUp_Click(object sender, EventArgs e)
    {
      MoveUp(routePropertyTypeCheckboxList.SelectedIndex);
    }

    private void moveDown_Click(object sender, EventArgs e)
    {
      MoveDown(routePropertyTypeCheckboxList.SelectedIndex);
    }

    private void UpdateUI()
    {
      moveUp.Enabled = (routePropertyTypeCheckboxList.SelectedIndex > 0);
      moveDown.Enabled = (routePropertyTypeCheckboxList.SelectedIndex < routePropertyTypeCheckboxList.Items.Count - 1 && routePropertyTypeCheckboxList.SelectedIndex != -1);
    }

    private void MoveUp(int index)
    {
      if (index < 1) return;
      var itemToMove = routePropertyTypeCheckboxList.Items[index];
      var isChecked = routePropertyTypeCheckboxList.CheckedItems.Contains(itemToMove);
      routePropertyTypeCheckboxList.Items.Remove(itemToMove);
      routePropertyTypeCheckboxList.Items.Insert(index-1, itemToMove);
      routePropertyTypeCheckboxList.SetItemChecked(index - 1, isChecked);
      routePropertyTypeCheckboxList.SelectedIndex = index - 1;
    }

    private void MoveDown(int index)
    {
      if (index > routePropertyTypeCheckboxList.Items.Count-2) return;
      var itemToMove = routePropertyTypeCheckboxList.Items[index];
      var isChecked = routePropertyTypeCheckboxList.CheckedItems.Contains(itemToMove);
      routePropertyTypeCheckboxList.Items.Remove(itemToMove);
      routePropertyTypeCheckboxList.Items.Insert(index + 1, itemToMove);
      routePropertyTypeCheckboxList.SetItemChecked(index + 1, isChecked);
      routePropertyTypeCheckboxList.SelectedIndex = index + 1;
    }

    private void routePropertyTypes_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Control && e.KeyCode == Keys.Up)
      {
        moveUp_Click(this, EventArgs.Empty);
        e.Handled = true;
        e.SuppressKeyPress = true;
      }
      if (e.Control && e.KeyCode == Keys.Down)
      {
        moveDown_Click(this, EventArgs.Empty);
        e.Handled = true;
        e.SuppressKeyPress = true;
      }
    }

    private void routePropertyTypeCheckboxList_SelectedIndexChanged(object sender, EventArgs e)
    {
      UpdateUI();
    }

  }
}
