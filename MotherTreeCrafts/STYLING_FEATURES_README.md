# Mother Tree Crafts - Styling and Features Update

## Overview
This branch implements comprehensive styling updates and new features for the Mother Tree Crafts e-commerce website, following the provided design sketch. The implementation uses Bootstrap 5 and custom CSS to create a professional, cohesive design with brown and green color schemes representing the crafts theme.

## Key Features Implemented

### 1. Stylized Header
- **Store Name Header**: Added a prominent header above the navigation with "Mother Tree Crafts" in a decorative font (Cinzel Decorative)
- **Color Scheme**: Green header (#2C5F2D) with gold accents (#D4AF37) for hover effects
- **Account Section**: Dynamic account link in the top-right corner
  - Shows "Account" dropdown with Login/Sign Up options for non-authenticated users
  - Shows direct "Account" link for authenticated users that leads to their account management page

### 2. Navigation Bar
- **Category-Based Navigation**: 
  - Home: Shows all active products
  - Handicrafts: Filters products by "Handicrafts" craft type
  - 3-D Prints: Filters products by "3D Prints" craft type
- **Admin Access**: "Add to Inventory" link only visible to Admin and Owner roles
- **Styling**: Dark navigation bar with hover effects and separated menu items

### 3. Main Content Area
- **Background**: Gray background (#A9A9A9) matching the sketch
- **Product Tiles**: Grid-based layout with responsive design
  - Each tile shows product image, title, brief description, and price
  - Hover effect with elevation and shadow
  - Clickable tiles that navigate to product details
  - Featured products display a "Featured" badge
- **Responsive Grid**: Automatically adjusts columns based on screen size

### 4. Account Management System
- **Account Page** (`/Account/Index`):
  - Displays user profile information (username, email, name, addresses)
  - Shows quick action buttons (Edit Profile, Change Password, Change Email, Logout)
  - Wishlist management section with product cards
  - Priority system for wishlist items (1-5 stars)
  
- **Edit Profile** (`/Account/EditProfile`):
  - Update first name, last name, shipping address, and billing address
  - Form validation and success/error messaging

- **Wishlist Features**:
  - Add products to wishlist with priority levels
  - View all wishlist items with product images and stock status
  - Remove items from wishlist
  - Visual priority indicators using star ratings

### 5. Inventory Management (Admin/Owner Only)
- **Products Index** (`/Products/Index`):
  - Table view of all products with images, prices, categories, stock levels, and status
  - Quick access to Details, Edit, and Delete actions
  - Total product count display
  - Success/error message notifications
  
- **Product CRUD Operations**:
  - Create: Comprehensive form for adding new products with inventory setup
  - Read: Public product details page with all information
  - Update: Edit existing products (restricted to Admin/Owner)
  - Delete: Confirm and delete products (restricted to Admin/Owner)

### 6. Role-Based Authorization
- **Roles Configured**: Admin and Owner roles added to Identity
- **Access Control**:
  - Public: Home, Category pages, Product Details
  - Authenticated: Account management, Wishlist
  - Admin/Owner: Inventory management, Product CRUD operations

## Technical Implementation

### Files Created
1. **Controllers**:
   - `AccountController.cs`: Handles account management and wishlist operations

2. **Views**:
   - `Views/Account/Index.cshtml`: Account management dashboard
   - `Views/Account/EditProfile.cshtml`: Profile editing form
   - `Views/Home/Category.cshtml`: Category-filtered product display

3. **CSS**:
   - Updated `wwwroot/css/site.css` with comprehensive styling

### Files Modified
1. **Layout**:
   - `Views/Shared/_Layout.cshtml`: Added store header, updated navigation, integrated account section

2. **Home Pages**:
   - `Views/Home/Index.cshtml`: Updated to display products in tile format
   - `Controllers/HomeController.cs`: Added product fetching and category filtering

3. **Products**:
   - `Controllers/ProductsController.cs`: Added role-based authorization
   - `Views/Products/Index.cshtml`: Converted to inventory management table view

4. **Configuration**:
   - `Program.cs`: Added role support to Identity configuration

### CSS Classes and Styling

#### Header Classes
- `.store-header`: Main header container with green background
- `.store-title`: Decorative font for store name
- `.account-section`: Account link positioning
- `.account-link`: Styled account link with hover effects

#### Navigation Classes
- `.main-nav`: Dark navigation bar styling
- `.nav-link`: Navigation link styling with borders and hover effects

#### Content Classes
- `.main-content`: Gray background content area
- `.products-grid`: Responsive grid layout for product tiles
- `.product-tile`: Individual product card styling
- `.product-tile-img`: Product image container
- `.product-tile-body`: Product information section
- `.product-tile-title`: Product title styling
- `.product-tile-price`: Price display in green

### Color Palette
- **Primary Green**: #2C5F2D (header background)
- **Dark Green**: #1a3a1b (borders)
- **Gold Accent**: #D4AF37 (hover effects)
- **Dark Nav**: #1a1a1a (navigation bar)
- **Content Gray**: #A9A9A9 (main background)
- **Tile Gray**: #C0C0C0 (product tiles)
- **Brown Accent**: #8B7355 (body background)

### Fonts
- **Decorative**: 'Cinzel Decorative' (store title)
- **Body**: 'Cormorant Garamond' (general content)
- **Fallback**: serif

## Usage Instructions

### For Site Owners/Admins
1. **Access Inventory Management**:
   - Ensure your user account has the "Admin" or "Owner" role
   - Click "Add to Inventory" in the navigation bar
   - Create, edit, or delete products

2. **Manage Product Categories**:
   - Set the `CraftType` field when creating/editing products
   - Use "Handicrafts" for handmade items
   - Use "3D Prints" for 3D printed items

3. **Feature Products**:
   - Check the "Featured" checkbox when creating/editing products
   - Featured products display prominently on the home page

### For Customers
1. **Browse Products**:
   - Visit the home page to see all products
   - Use navigation links to filter by category
   - Click on any product tile to view details

2. **Manage Account**:
   - Click "Account" in the header to access account management
   - Update profile information including shipping/billing addresses
   - Manage wishlist items
   - Change password or email through account settings

3. **Wishlist**:
   - Add products to wishlist from product pages (feature to be implemented)
   - View all wishlist items on account page
   - Remove items as needed

## Setup Requirements

### Database Migration
If you haven't already, ensure roles are set up in the database:
```bash
dotnet ef migrations add AddRoles
dotnet ef database update
```

### Create Admin Users
After running the application, you'll need to manually add roles to users in the database or create a seeding script:
```sql
-- Insert roles if they don't exist
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (NEWID(), 'Admin', 'ADMIN', NEWID()),
       (NEWID(), 'Owner', 'OWNER', NEWID());

-- Assign role to a user (replace with actual user ID and role ID)
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES ('user-id-here', 'admin-role-id-here');
```

## Future Enhancements

### Recommended Next Steps
1. **Product Sorting**: Add sorting options (price, date, popularity) on category and home pages
2. **Search Functionality**: Implement product search with filters
3. **Image Upload**: Replace URL input with file upload for product images
4. **Shopping Cart**: Add cart functionality for purchasing products
5. **Order Management**: Track customer orders and shipping
6. **Reviews System**: Complete implementation of product reviews
7. **Wishlist Integration**: Add "Add to Wishlist" buttons on product pages and tiles
8. **Pagination**: Implement pagination for product listings
9. **Product Variants**: Support for size/color variations of products
10. **Admin Dashboard**: Create comprehensive dashboard for site statistics

### Design Improvements
1. Add animations and transitions for smoother user experience
2. Implement loading states for async operations
3. Add toast notifications for better user feedback
4. Create custom 404 and error pages matching the design
5. Optimize images with lazy loading
6. Add breadcrumb navigation for better site navigation

## Testing Checklist

### Visual Testing
- [ ] Store header displays correctly on all screen sizes
- [ ] Navigation links work and highlight correctly
- [ ] Product tiles display properly in grid layout
- [ ] Hover effects work on tiles and navigation
- [ ] Account dropdown appears for non-authenticated users
- [ ] Account link works for authenticated users

### Functionality Testing
- [ ] Home page displays all active products
- [ ] Category filtering works for Handicrafts and 3D Prints
- [ ] Product tiles are clickable and navigate to details
- [ ] Account page displays user information
- [ ] Profile editing saves changes
- [ ] Wishlist displays correctly
- [ ] Admin can access inventory management
- [ ] Non-admins cannot access inventory management
- [ ] Product CRUD operations work for authorized users

### Responsive Testing
- [ ] Layout adapts to mobile screens
- [ ] Navigation collapses on small screens
- [ ] Product grid adjusts column count
- [ ] Store title scales appropriately
- [ ] Forms are usable on mobile devices

## Browser Compatibility
- Chrome: ?
- Firefox: ?
- Safari: ?
- Edge: ?
- Mobile browsers: ?

## Dependencies
- Bootstrap 5.x (included via CDN in layout)
- Bootstrap Icons 1.11.3 (included via CDN)
- Google Fonts: Cinzel Decorative, Cormorant Garamond
- ASP.NET Core Identity
- Entity Framework Core

## Notes
- The "Add to Inventory" navigation link is only visible to users with Admin or Owner roles
- Products with `IsActive = false` will not appear on public pages
- The category filtering expects exact matches for "Handicrafts" and "3D Prints" in the `CraftType` field
- Email confirmation is required for new user registration (can be disabled in Program.cs if needed for development)

## Support
For issues or questions, please refer to the project documentation or contact the development team.
