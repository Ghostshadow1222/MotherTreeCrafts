Analysis page for class relationships for the project setup as CRC Cards, this page will act as both a reference and plan for future changes

Changes will be made to the page as the project undergoes refactoring and review, so some of the information may be outdated or inaccurate as the project evolves.

Connections and relationships open to change and feedback

### CRC Cards --

#### Product
Responsibilities:
- Holds information relating to a product
- Holds product status

Collaborators:
- Inventory
- ProductReview
- Wishlist
- UserAccount (for admin and owner)

#### Inventory
Responsibilities:
- Tracks and manages product numbers
- Filter products on status 
- Keeps track of number of products

Collaborators:
- Product
- Wishlist
- UserAccount (for admin and owner)

#### ProductReview
Responsibilities:
- Holds information relating to a product review
- Adjustable by users, admins and owners
- Calcualates average rating for a product

Collaborators:
- Product
- UserAccount

#### Wishlist
Responsibilities:
- Holds information of a user's wanted products
- Adjustable by user
- Keeps track of when product was wishlisted
- Notify user of product changes (product out of stock, product removed from sale, etc.)

Collaborators:
- Product
- Inventory
- UserAccount

#### UserAccount
Responsibilities:
- Stores user credentials
- Stores user address information
- Stores user status (customer, admin, owner)
- Stores user activity (reviews, wishlists, etc.)
- Manages user information

Collaborators:
- ProductReview
- Wishlist
- Inventory (for admin and owner)
- Product (for admin and owner)

