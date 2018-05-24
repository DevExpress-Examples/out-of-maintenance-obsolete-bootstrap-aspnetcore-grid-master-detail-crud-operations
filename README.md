# BootstrapGridView for ASP.NET Core - How to implement the Master Detail Scenario with CRUD operations
This example shows how to implement a master-detail relationship and how to insert, update and delete records from both master and detail grids. For this, we will use a Booststrap-based component for ASP.NET Core: [BootststrapGridView](https://demos.devexpress.com/aspnetcore-bootstrap/GridView).

## Steps to implement:
* Create two Partial Views: one keeps the Master grid, another keeps the Detail one.
* Add the "Master" Partial View to the required View:

```csharp 
@Html.Partial("MasterGridView", Model)
```
* Add BootstrapGridView to the "Master" Partial View. Specify the `KeyFieldName` property and enable the "New", "Edit" and "Delete" buttons via the `AddCommandColumn` method:

```csharp 
.KeyFieldName(k => k.CategoryID)
.Columns(columns => {
    columns.AddCommandColumn()
        .ShowNewButtonInHeader(true)
        .ShowEditButton(true)
        .ShowDeleteButton(true);
        ...
```
* Specify routes for handling data operations:

```csharp 
.Routes(routes => routes
    .MapRoute(r => r.Controller("Home").Action("MasterDetailView"))
    .MapRoute(r => r.RouteType(GridViewRouteType.AddRow).Controller("Home").Action("MasterAddAction"))
    .MapRoute(r => r.RouteType(GridViewRouteType.UpdateRow).Controller("Home").Action("MasterUpdateRowAction"))
    .MapRoute(r => r.RouteType(GridViewRouteType.DeleteRow).Controller("Home").Action("MasterDeleteRowAction"))
)
```
* Enable the Detail area in the Master grid:

```csharp  
.SettingsDetail(settings => settings.ShowDetailRow(true))
``` 
* Specify the Detail Row template. Add the "Detail" Partial View there and pass the master row key value to it:

```csharp 
.Templates(templates => templates
        .DetailRow(detailRow => @<text>
        @{ 
            int keyValue = (int)detailRow.KeyValue;
            var detailModel = Northwind.Products.Where(p => p.CategoryID == keyValue);
            @Html.Partial("DetailGridView", 
                detailModel, 
                new ViewDataDictionary(ViewData) { { "categoryID", keyValue } });
        }
    </text>))
```
* Add BootstrapGridView to the "Detail" Partial View. Since there can be several Detail grids on a page, their names should be unique. So, set the detail grid's `Name` property dynamically using the master row key passed in Step 6:

```csharp  
.BootstrapGridView<Product>()
.Name($"detailGridView{ViewData["categoryID"]}")
```
* Specify `KeyFieldName`, Routes and enable command buttons like in the "Master" grid. Use the current master row key as a parameter in **all** detail Routes:

```csharp 
.Routes(routes => routes
    ...
    .MapRoute(r =>r.RouteType(GridViewRouteType.AddRow)
        .RouteValues(new {
            Controller = "Home",
            Action = "DetailAddAction",
            categoryID = ViewData["categoryID"]
        }))
        ...
```
* Handle CRUD operations in corresponding Controller's Action methods. Use the master row key to get corresponding detail data:

```csharp 
public IActionResult DetailAddAction(Product product, int categoryID) {
    try {
        if (ModelState.IsValid) {
            _northwindContext.Products.Add(product);
            _northwindContext.SaveChanges();
            }
        } catch (Exception e) {
                ViewData["error"] = e.Message;
    }
    ViewData["categoryID"] = categoryID;
    return PartialView("DetailGridView", _northwindContext.Products.Where(p => p.CategoryID == categoryID));
}
```

