# CommunityCar Layout System

This document describes the three specialized layouts created for the CommunityCar application, each optimized for different sections and use cases.

## Overview

The CommunityCar application now includes three distinct layouts:

1. **Main App Layout** (`_MainAppLayout.cshtml`) - For community features and general app pages
2. **Dashboard Layout** (`_OptimizedDashboardLayout.cshtml`) - For administrative and management interfaces
3. **Profile Layout** (`_ProfileLayout.cshtml`) - For user profile pages and personal content

## Layout Specifications

### 1. Main App Layout (`_MainAppLayout.cshtml`)

**Purpose**: Community-focused pages including feeds, posts, groups, events, Q&A, and general app content.

**Key Features**:
- Community-optimized header with social actions
- Create post quick action button
- Feed-focused sidebar layout
- Real-time notifications for community interactions
- Mobile-first responsive design
- Integrated AI assistant
- Create post modal component

**Usage**:
```razor
@{
    Layout = "_MainAppLayout";
    ViewData["Title"] = "Community Feed";
    ViewData["PageType"] = "Community";
}
```

**Sidebar Configuration**:
```razor
@{
    ViewData["HideLeftSidebar"] = false;  // Show community navigation
    ViewData["HideRightSidebar"] = false; // Show trending/suggestions
}
```

**Best For**:
- Home/Feed pages
- Community posts and discussions
- Groups and events
- Q&A sections
- Stories and media sharing

### 2. Dashboard Layout (`_OptimizedDashboardLayout.cshtml`)

**Purpose**: Administrative interfaces, analytics, and management tools.

**Key Features**:
- Fixed header with dashboard-specific navigation
- Collapsible sidebar with admin menu
- Analytics-focused right sidebar
- Real-time data refresh capabilities
- Dashboard-specific theme styling
- Performance monitoring integration
- Admin-only components and actions

**Usage**:
```razor
@{
    Layout = "_OptimizedDashboardLayout";
    ViewData["Title"] = "Analytics Overview";
    ViewData["DashboardSection"] = "Analytics";
    ViewData["Description"] = "View platform analytics and performance metrics";
}
```

**Page Actions Section**:
```razor
@section PageActions {
    <button class="btn-primary">
        <i data-lucide="download" class="w-4 h-4"></i>
        Export Report
    </button>
}
```

**Best For**:
- Admin dashboard pages
- Analytics and reporting
- System settings and configuration
- User management
- Content moderation tools

### 3. Profile Layout (`_ProfileLayout.cshtml`)

**Purpose**: User profile pages, personal settings, and profile-related content.

**Key Features**:
- Profile-specific header with user actions
- Integrated profile navigation tabs
- Profile sidebar with user stats and info
- Mobile-optimized profile display
- Profile-specific theme styling
- Social interaction buttons (follow/message)
- Personal content management tools

**Usage**:
```razor
@{
    Layout = "_ProfileLayout";
    ViewData["Title"] = "John Doe";
    ViewData["ProfileSection"] = "Gallery";
    ViewData["ProfileUserId"] = "user-id-123";
    ViewData["IsOwnProfile"] = true;
}
```

**Profile Data Setup**:
```razor
@{
    ViewBag.UserId = "user-id";
    ViewBag.FullName = "John Doe";
    ViewBag.UserName = "johndoe";
    ViewBag.ProfilePictureUrl = "/images/profiles/john.jpg";
    ViewBag.Bio = "Software developer and car enthusiast";
    ViewBag.City = "New York";
    ViewBag.Country = "USA";
    ViewBag.CreatedAt = DateTime.Now.AddYears(-2);
    ViewBag.Rank = 42;
    ViewBag.Level = 15;
    ViewBag.TotalPoints = 2500;
    ViewBag.PostsCount = 156;
    ViewBag.CommentsCount = 423;
    ViewBag.LikesReceived = 1250;
    ViewBag.BadgesCount = 8;
}
```

**Right Sidebar Content**:
```razor
@section SidebarRight {
    <div class="space-y-6">
        <!-- Profile achievements -->
        <!-- Recent activity -->
        <!-- Mutual friends -->
    </div>
}
```

**Best For**:
- User profile pages
- Profile settings and preferences
- User galleries and media
- Personal activity feeds
- Profile analytics and insights

## Common Features Across All Layouts

### Theme System
All layouts support the unified theme system:
- Light/Dark mode toggle
- Automatic theme detection
- Theme persistence in localStorage
- Smooth theme transitions

### Localization
Full RTL/LTR support with:
- Arabic/English language toggle
- Culture-aware routing
- Localized content and UI elements
- Font optimization for each language

### Responsive Design
Mobile-first approach with:
- Collapsible sidebars on mobile
- Touch-optimized interactions
- Adaptive navigation patterns
- Optimized content layout for all screen sizes

### Performance Optimizations
- Lazy loading for non-critical resources
- Optimized CSS and JS bundling
- Efficient font loading strategies
- SEO meta tag generation

## Layout Selection Guidelines

### Use Main App Layout When:
- Building community-focused features
- Creating social interaction pages
- Developing content sharing interfaces
- Building public-facing app pages

### Use Dashboard Layout When:
- Creating admin interfaces
- Building analytics pages
- Developing system management tools
- Creating reporting interfaces

### Use Profile Layout When:
- Building user profile pages
- Creating personal settings interfaces
- Developing user-specific content pages
- Building profile management tools

## Migration from Existing Layouts

### From `_Layout.cshtml`:
1. Change layout reference to `_MainAppLayout`
2. Update ViewData properties as needed
3. Test responsive behavior
4. Verify sidebar content compatibility

### From `_DashboardLayout.cshtml`:
1. Change layout reference to `_OptimizedDashboardLayout`
2. Add dashboard-specific ViewData
3. Update page actions if needed
4. Test admin-specific features

### Creating New Profile Pages:
1. Use `_ProfileLayout` as base
2. Set up ViewBag profile data
3. Configure profile navigation
4. Add profile-specific content

## Best Practices

1. **Layout Selection**: Choose the most appropriate layout for your page's primary purpose
2. **ViewData Configuration**: Always set required ViewData properties for proper layout behavior
3. **Sidebar Content**: Use appropriate sidebar partials for each layout type
4. **Mobile Testing**: Test all layouts on mobile devices for optimal user experience
5. **Performance**: Monitor layout performance and optimize as needed
6. **Accessibility**: Ensure all layouts meet accessibility standards
7. **SEO**: Configure proper meta tags and page types for search optimization

## Troubleshooting

### Common Issues:
- **Sidebar not showing**: Check `HideLeftSidebar`/`HideRightSidebar` ViewData settings
- **Theme not working**: Verify theme toggle JavaScript is loaded
- **Mobile layout issues**: Test responsive breakpoints and sidebar behavior
- **Profile data missing**: Ensure ViewBag properties are set correctly
- **Navigation not working**: Check culture routing and localization setup

### Debug Tips:
- Use browser developer tools to inspect layout structure
- Check console for JavaScript errors
- Verify CSS classes are applied correctly
- Test with different screen sizes and orientations
- Validate HTML structure and accessibility

## Future Enhancements

Planned improvements for the layout system:
- Layout-specific performance optimizations
- Enhanced mobile gestures and interactions
- Advanced theming options
- Layout-specific analytics tracking
- Improved accessibility features
- Progressive Web App (PWA) optimizations