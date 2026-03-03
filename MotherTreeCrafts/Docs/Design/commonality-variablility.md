Analysis page for commonalities and variabilities in the MotherTreeCrafts project, based on the identified entities, roles, and attributes.

More variabilities may be added as the project evolves, but this provides a starting point for understanding the core design and potential areas of change.

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

    ### Variability 2: User Authentication Methods
- Why it may change: 
  The system currently uses ASP.NET Core Identity, but may need to support OAuth providers (Google, Facebook), two-factor authentication, passwordless authentication, or SSO for business customers.

- How it is isolated:  
  Composition over Inheritance + Delegation to Service  
  Rather than modifying UserAccount directly, use ASP.NET Core Identity's built-in extensibility with `IUserStore` and external authentication providers. Create an `IAuthenticationService` that coordinates different auth methods. UserAccount remains focused on user profile data while authentication strategies are handled separately.
  
  Related Principle: Favor composition over inheritance, Separation of Concerns

    ### Variability 3: Product Search and Filtering
- Why it may change:  
  Search algorithms may evolve from simple SQL queries to full-text search, fuzzy matching, AI-powered recommendations, or third-party search services (Elasticsearch, Azure Cognitive Search). Filter criteria will expand as product attributes grow.

- How it is isolated:
  Strategy Pattern + Interface  
  Create an `IProductSearchService` interface with implementations for different search backends (DatabaseSearch, ElasticsearchSearch). The search logic stays out of the Product model and can be swapped without affecting core domain logic. Filter specifications can use the Specification Pattern.
  
  Related Principle: Dependency Inversion Principle, Encapsulate what varies