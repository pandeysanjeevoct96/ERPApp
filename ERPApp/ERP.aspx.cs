using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App_Code.BLL;
using App_Code.Models;
using System.Linq.Dynamic.Core;

namespace ERPApp
{
    public partial class ERP : System.Web.UI.Page
    {
        // Dependencies
        private readonly TruckService _truckService;

        // Property to manage Truck ID in ViewState
        private int? _truckId
        {
            get => (int?)ViewState["TruckId"];
            set => ViewState["TruckId"] = value;
        }

        // Constructor
        public ERP()
        {
            // Initialize the TruckService instance
            _truckService = new TruckService();
        }

        // Page Load event
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTruckList();
            }
        }

        private void BindTruckList()
        {
            IEnumerable<Truck> trucks = _truckService.GetAllTrucks();
            gvTrucks.DataSource = trucks;
            gvTrucks.DataBind();
            ClearForm();
        }

        protected void gvTrucks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var truck = e.Row.DataItem as Truck;
                DropDownList ddlStatusUpdate = (DropDownList)e.Row.FindControl("ddlStatusUpdate");
                if (ddlStatusUpdate != null)
                {
                    ddlStatusUpdate.SelectedValue = truck.Status.ToString(); 
                }
            }
        }

        protected void gvTrucks_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int truckId = (int)e.Keys["TruckId"];
            bool success = _truckService.DeleteTruck(truckId);

            if (success)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alertSuccess", "alert('Truck deleted successfully.');", true);

                BindTruckList();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alertError", "alert('Failed to delete the truck. Please try again.');", true);
            }
            ClearForm();
        }

        protected void GridViewTrucks_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditTruck")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int truckId = Convert.ToInt32(gvTrucks.DataKeys[rowIndex]["TruckId"]);

                Truck truck = _truckService.GetTruckById(truckId);
                _truckId = truck.TruckId;

                txtTruckName.Text = truck.TruckName;
                ddlStatus.SelectedValue = truck.Status.ToString();
                txtDescription.Text = truck.Description;
            }
        }

        protected void gvTrucks_Sorting(object sender, GridViewSortEventArgs e)
        {
            BindTruckData(e.SortExpression);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string uniqueIdentifier = DateTime.Now.ToString("yy");
            string truckCode = $"TRK-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}-{uniqueIdentifier}";

            Truck truck = new Truck
            {
                TruckId = _truckId ?? 0,
                TruckCode = truckCode,
                TruckName = txtTruckName.Text,
                Status = Enum.TryParse<TruckStatus>(ddlStatus.SelectedValue, out var status) ? status : TruckStatus.OutOfService,
                Description = txtDescription.Text
            };

            // Check if truck code is unique
            if (_truckService.IsTruckCodeUnique(truck.TruckCode))
            {
                if (_truckId.HasValue)
                {
                    // Existing truck: Validate status transition before updating
                    bool isStatusValid = _truckService.ChangeTruckStatus(truck.TruckId, truck.Status, validateOnly: true);

                    if (isStatusValid)
                    {
                        // Proceed to update the truck record in the database
                        _truckService.UpdateTruck(truck);
                        BindTruckList();
                        ClearForm();
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alertError", "alert('Invalid status transition!');", true);
                    }
                }
                else
                {
                    // New truck: Skip status validation and create the truck directly
                    _truckService.CreateTruck(truck);
                    BindTruckList();
                    ClearForm();
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alertError", "alert('Truck code must be unique!');", true);
            }
        }

        private void BindTruckData(string sortExpression = null)
        {
            var trucks = _truckService.GetAllTrucks(); 

            if (!string.IsNullOrEmpty(txtFilterCode.Text))
            {
                trucks = trucks.Where(t => t.TruckCode.Contains(txtFilterCode.Text)).ToList();
            }
            if (!string.IsNullOrEmpty(txtFilterName.Text))
            {
                trucks = trucks.Where(t => t.TruckName.Contains(txtFilterName.Text)).ToList();
            }
            if (!string.IsNullOrEmpty(txtFilterStatus.Text))
            {
                trucks = trucks.Where(t => t.Status.ToString().Contains(txtFilterStatus.Text)).ToList();
            }

            if (!string.IsNullOrEmpty(sortExpression))
            {
                trucks = trucks.AsQueryable().OrderBy(sortExpression).ToList();
            }

            gvTrucks.DataSource = trucks;
            gvTrucks.DataBind();
        }

        protected void FilterTrucks(object sender, EventArgs e)
        {
            BindTruckData();
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlStatus = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddlStatus.NamingContainer;
            int truckId = Convert.ToInt32(gvTrucks.DataKeys[row.RowIndex].Value);
            TruckStatus newStatus;

            if (Enum.TryParse(ddlStatus.SelectedValue, out newStatus))
            {
                if (_truckService.ChangeTruckStatus(truckId, newStatus, validateOnly: false))
                {
                    BindTruckList();
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alertError", "alert('Invalid status change.');", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alertError", "alert('Failed to parse selected status.');", true);
            }
        }

        private void ClearForm()
        {
            _truckId = null;
            txtTruckName.Text = string.Empty;
            ddlStatus.SelectedIndex = 0; 
            txtDescription.Text = string.Empty;
        }
        protected void ClearFilters(object sender, EventArgs e)
        {
            txtFilterCode.Text = "";
            txtFilterName.Text = "";
            txtFilterStatus.Text = "";
            BindTruckData();
        }
    }
}