Analysis page for nouns and verbs in MotherTreeCrafts project, as well as a page that will help oragnize potiential changes to the project.

Some parts of the analysis will be coming from the code itself after review those parts are marked with (*),
new addtions will be marked with (++) to better identify changes as project grows.

Current nouns and verbs are from before the start of large review and refactoring effort of the project so there
may be some changes and inconsistencies that will be fixed as the review and refactoring continues.

## Entities --
(*)- Product 
  - verbs: create, edit, update, delete, view, display, search, filter, sort, feature, activate, deactivate
(*)- Inventory
  - verbs: track, update, reorder, reserve, check, adjust, replenish
(*)- ProductReview (Debating shortening to just Review but might be too general)
  - verbs: create, submit, edit, update, delete, display, moderate, approve, flag
(*)- Wishlist
  - verbs: add, remove, view, prioritize, update, manage
(*)- UserAccount (Similar debating as ProductReview)
  - verbs: register, login, logout, update, authenticate, authorize, manage
(++)- Verification (should be for verifying users, but could be handled in UserAccount maybe?)
	- verbs: send, verify, confirm, expire, resend
(++)- Order
	- verbs: create, update, cancel, view, track, complete, refund
(++)- Cart
	- verbs: add, remove, view, update, checkout, clear
(++)- ProductType (Is categroy for products and possibly tags as well)
	- verbs: create, edit, delete, view, assign, unassign, feature
(++)- 


## Roles/Actors --
- User/Customer
  - verbs: browse, view, register, login, logout, review, wishlist, customize, search
- Admin
  - verbs: manage, authorize, moderate, approve, delete, configure
- Owner
  - verbs: manage, authorize, create, edit, delete, configure, oversee

## Attributes --
- ProductId
- Title
- Price
- Description
- ImageUrl
- CraftType
- Material
- Dimensions
- WeightInGrams
- SKU
- QuantityOnHand
- ReorderLevel
- MaxStockLevel
- Rating
- ReviewDate
- FirstName
- LastName
- Email
- ShippingAddress
- BillingAddress
- IsActive
- IsFeatured
- IsCustomizable
- IsDigitalProduct
- IsMadeToOrder
- Priority
- StorageLocation
- Tags
- ColorOptions
- DifficultyLevel
- DesignerName

## System/Technical -- 
- Database
- Login Page
- Register Page
- Navigation Menu
- Form
- Button
- View
- Logger
- TempData
- Migration
