<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ERP.aspx.cs" Inherits="ERPApp.ERP" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ERP</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://ajax.aspnetcdn.com/ajax/jquery.validation/1.19.3/jquery.validate.min.js"></script>
    <script src="https://ajax.aspnetcdn.com/ajax/jquery.validation/1.19.3/jquery.validate.unobtrusive.min.js"></script>
</head>
<body>
    
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div class="container">            
                <h3 style="text-align:center">Truck</h3>
            <div class="col-md-4">
                <div class="form-group">
                <label for="txtTruckName">Truck Name:</label>
                <asp:TextBox ID="txtTruckName" CssClass="form-control" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator 
                    ID="rfvTruckName" 
                    runat="server" 
                    ControlToValidate="txtTruckName" 
                    ErrorMessage="Truck Name is required." 
                    CssClass="text-danger" 
                    Display="Dynamic" 
                    ValidationGroup="TruckValidation" />
                </div>
                <div class="form-group">
                    <label for="ddlStatus">Status:</label>
                    <asp:DropDownList ID="ddlStatus" CssClass="form-control" runat="server">
                        <asp:ListItem Value="Out Of Service">Out Of Service</asp:ListItem>
                        <asp:ListItem Value="Loading">Loading</asp:ListItem>
                        <asp:ListItem Value="ToJob">To Job</asp:ListItem>
                        <asp:ListItem Value="AtJob">At Job</asp:ListItem>
                        <asp:ListItem Value="Returning">Returning</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label for="txtDescription">Description:</label>
                    <asp:TextBox ID="txtDescription" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                </div>
               <%-- <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="btn btn-success" ValidationGroup="TruckValidation" />--%>
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="validateAndSubmit(); return false;" />
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            </div>
            <div class="col-md-8">
                <asp:Label ID="lblTruckId" runat="server" Text="" Visible="false"></asp:Label><br />
                <asp:TextBox ID="txtFilterCode" runat="server" Placeholder="Filter by Code"></asp:TextBox>
                <asp:TextBox ID="txtFilterName" runat="server" Placeholder="Filter by Name"></asp:TextBox>
                <asp:TextBox ID="txtFilterStatus" runat="server" Placeholder="Filter by Status"></asp:TextBox>
                <asp:Button ID="btnFilter" runat="server" Text="Filter" OnClick="FilterTrucks" />
                <asp:Button ID="btnClearFilter" runat="server" Text="Clear" OnClick="ClearFilters" />
                <hr />
                <asp:GridView ID="gvTrucks" CssClass="table" 
                    runat="server" AutoGenerateColumns="false" 
                    OnRowCommand="GridViewTrucks_RowCommand"
                    OnRowDeleting="gvTrucks_RowDeleting" 
                    OnRowDataBound="gvTrucks_RowDataBound"
                    AllowSorting="true" OnSorting="gvTrucks_Sorting"
                    DataKeyNames="TruckId" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="TruckCode" HeaderText="Code" />
                        <asp:BoundField DataField="TruckName" HeaderText="Name" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:TemplateField HeaderText="Change Status">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlStatusUpdate" runat="server" AutoPostBack="True"
                                                  OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                    <asp:ListItem Value="OutOfService">Out Of Service</asp:ListItem>
                                    <asp:ListItem Value="Loading">Loading</asp:ListItem>
                                    <asp:ListItem Value="ToJob">To Job</asp:ListItem>
                                    <asp:ListItem Value="AtJob">At Job</asp:ListItem>
                                    <asp:ListItem Value="Returning">Returning</asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField CommandName="EditTruck" Text="Edit" />
                        <asp:CommandField ShowDeleteButton="True" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div style="text-align: center; font-weight: bold;">No Record found</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </form>
    
    <script type="text/javascript">
       // window.onload = function () {
            function validateForm() {
                var isValid = true;
                var txtName = document.getElementById('<%= txtTruckName.ClientID %>');
                if (txtName.value.trim() === "") {
                    alert("Truck Name is required.");
                    isValid = false;
                }
                return isValid;
            }

            function validateAndSubmit() {
                if (validateForm()) {
                    __doPostBack('<%= btnSave.UniqueID %>', '');
                }
            }
        //}
</script>
</body>
</html>
