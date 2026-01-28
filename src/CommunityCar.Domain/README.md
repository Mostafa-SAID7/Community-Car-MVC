# CommunityCar Domain Layer

This document outlines the Domain-Driven Design (DDD) structure of the CommunityCar Domain layer.

## 📁 Folder Structure

### `/Base`
Contains base classes and interfaces used throughout the domain:
- `AggregateRoot.cs` - Base class for aggregate roots
- `BaseEntity.cs` - Base class for all entities
- `IBaseEntity.cs` - Interface for base entity functionality
- `ValueObject.cs` - Base class for value objects

### `/Constants`
Contains domain constants:
- `Progression.cs` - User progression constants
- `Roles.cs` - User role constants

### `/Entities`
Contains all domain entities organized by bounded context:
- `/Account` - User account related entities
  - `/Core` - Core user entities (User)
  - `/Profile` - Profile related entities (UserFollowing, UserProfileView)
  - `/Authentication` - Authentication entities (UserToken, UserSession)
  - `/Gamification` - Gamification entities (UserBadge, UserAchievement)
  - `/Media` - Media entities (UserGallery)
  - `/Management` - Management entities (UserManagementAction)
  - `/Analytics` - Analytics entities (UserContentAnalytics)
- `/Community` - Community features
- `/Dashboard` - Dashboard entities
- `/Shared` - Shared entities across contexts

### `/Enums`
Contains domain enumerations:
- `/Account` - Account related enums (MediaType, etc.)
- `/Users` - User related enums
- Various other domain enums

### `/Events` ⭐
Domain events following the Domain Events pattern:
- `IDomainEvent.cs` - Base interface for domain events
- `/Account` - Account related events
  - `UserRegisteredEvent.cs` - Raised when user registers
  - `ProfileViewedEvent.cs` - Raised when profile is viewed
  - `UserFollowedEvent.cs` - Raised when user follows another
- `/Community` - Community related events
  - `PostCreatedEvent.cs` - Raised when post is created
  - `PostLikedEvent.cs` - Raised when post is liked

### `/Exceptions` ⭐
Domain-specific exceptions:
- `DomainException.cs` - Base domain exception
- `BusinessRuleValidationException.cs` - Business rule violations
- `/Account` - Account specific exceptions
  - `InvalidUserOperationException.cs` - Invalid user operations

### `/Policies` ⭐
Access control and authorization policies:
- `IAccessPolicy.cs` - Base interface for access policies
- `/Account` - Account related policies
  - `ProfileAccessPolicy.cs` - Profile access permissions
- `/Community` - Community related policies
  - `ContentModerationPolicy.cs` - Content moderation rules

### `/Rules` ⭐
Business rules following the Business Rules pattern:
- `IBusinessRule.cs` - Base interface for business rules
- `/Account` - Account related rules
  - `UserRegistrationRules.cs` - User registration validation
  - `ProfileViewRules.cs` - Profile viewing rules
- `/Community` - Community related rules
  - `PostRules.cs` - Post creation and editing rules

### `/Services` ⭐
Domain services for complex business logic:
- `IDomainService.cs` - Marker interface for domain services
- `/Account` - Account related domain services
  - `UserDomainService.cs` - User interaction and reputation logic

### `/Specifications` ⭐
Query specifications following the Specification pattern:
- `ISpecification.cs` - Base specification interface
- `BaseSpecification.cs` - Base specification implementation
- `/Account` - Account related specifications
  - `UserSpecifications.cs` - User query specifications
  - `ProfileViewSpecifications.cs` - Profile view query specifications
- `/Community` - Community related specifications
  - `PostSpecifications.cs` - Post query specifications

### `/ValueObjects` ⭐
Value objects representing domain concepts:
- `Address.cs` - Address value object
- `/Account` - Account related value objects
  - `NotificationSettings.cs` - User notification preferences
  - `OAuthInfo.cs` - OAuth authentication info
  - `PrivacySettings.cs` - User privacy settings
  - `TwoFactorSettings.cs` - Two-factor authentication settings
  - `UserProfile.cs` - User profile information
  - `ProfileViewMetrics.cs` - Profile view analytics
- `/Common` - Common value objects
  - `Email.cs` - Email address validation
  - `PhoneNumber.cs` - Phone number validation
- `/Community` - Community related value objects
  - `PostMetrics.cs` - Post engagement metrics

## 🏗️ DDD Patterns Implemented

### 1. **Entities & Aggregate Roots**
- Rich domain models with behavior
- Proper encapsulation with private setters
- Domain events integration

### 2. **Value Objects**
- Immutable objects representing domain concepts
- Validation logic encapsulated within
- Equality based on value, not identity

### 3. **Domain Events**
- Decouple domain logic from side effects
- Enable event-driven architecture
- Support for eventual consistency

### 4. **Business Rules**
- Explicit business rule validation
- Async support for complex validations
- Clear error messages

### 5. **Specifications**
- Encapsulate query logic
- Reusable and composable
- Support for complex filtering and sorting

### 6. **Domain Services**
- Business logic that doesn't fit in entities
- Stateless operations
- Cross-aggregate operations

### 7. **Policies**
- Authorization and access control
- Business policy enforcement
- Separation of concerns

## 🚀 Usage Examples

### Business Rules
```csharp
var rule = new EmailMustBeUniqueRule(email, emailExistsCheck);
if (await rule.IsBrokenAsync())
    throw new BusinessRuleValidationException(rule);
```

### Specifications
```csharp
var spec = new UserSpecifications.ActiveUsersSpec();
var activeUsers = await repository.ListAsync(spec);
```

### Domain Events
```csharp
user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.Profile.FullName, DateTime.UtcNow));
```

### Value Objects
```csharp
var email = Email.Create("user@example.com");
var phone = PhoneNumber.Create("+1234567890");
```

## 📋 Best Practices

1. **Keep entities focused** - Each entity should have a single responsibility
2. **Use value objects** - For concepts that are defined by their value, not identity
3. **Validate at boundaries** - Use business rules to validate operations
4. **Raise domain events** - For important business events that other parts of the system care about
5. **Encapsulate business logic** - Keep business rules within the domain layer
6. **Use specifications** - For complex query logic that needs to be reusable

## 🔄 Integration with Other Layers

- **Application Layer**: Uses domain services, specifications, and handles domain events
- **Infrastructure Layer**: Implements repositories using specifications
- **Web Layer**: Catches domain exceptions and converts to appropriate responses

This structure provides a solid foundation for implementing Domain-Driven Design principles while maintaining clean separation of concerns and high testability.