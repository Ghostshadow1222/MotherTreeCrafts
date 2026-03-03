Analysis page for commonalities and variabilities in the MotherTreeCrafts project, based on the identified entities, roles, and attributes.



## Commonalities

- Products are managed through a central catalog system
- All products have core attributes (Title, Price, Description, ImageUrl)
- User accounts are tracked through ASP.NET Core Identity
- Inventory is monitored with quantity tracking
- Product reviews include ratings and comments
- The system uses Entity Framework Core for data persistence
- All entities use integer-based primary keys (except UserAccount)

## Variabilities

### Variability 1: Inventory Reorder Logic
- Why it may change: 
  Reorder rules may vary based on product type, seasonality, supplier lead times, or sales velocity. Some products might need automatic reordering while others require manual approval.

- How it is isolated: 
  Policy Object (Rule Cluster)**  
  Create an `IInventoryReorderPolicy` interface with implementations like `StandardReorderPolicy`, `SeasonalReorderPolicy`, or `CustomReorderPolicy`. The Inventory class uses this policy to determine when and how much to reorder, keeping reorder logic separate from core inventory tracking.
  
  Related Principle: Single Responsibility Principle, Strategy Pattern

