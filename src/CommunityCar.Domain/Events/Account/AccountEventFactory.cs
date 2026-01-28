using CommunityCar.Domain.Events.Account.Factories.Authentication;
using CommunityCar.Domain.Events.Account.Factories.Lifecycle;
using CommunityCar.Domain.Events.Account.Factories.Profile;
using CommunityCar.Domain.Events.Account.Factories.Registration;
using CommunityCar.Domain.Events.Account.Factories.Security;
using CommunityCar.Domain.Events.Account.Authentication;
using CommunityCar.Domain.Events.Account.Lifecycle;
using CommunityCar.Domain.Events.Account.Profile;
using CommunityCar.Domain.Events.Account.Registration;
using CommunityCar.Domain.Events.Account.Security;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Events.Account;

public static class AccountEventFactory
{
    public static UserRegisteredEvent CreateUserRegistered(
        Guid userId,
        string email,
        string username,
        string firstName,
        string lastName,
        string registrationMethod = "Email",
        string ipAddress = null,
        string userAgent = null,
        bool requiresEmailVerification = true)
        => UserRegisteredEventFactory.CreateUserRegistered(
            userId, email, username, firstName, lastName, 
            registrationMethod, ipAddress, userAgent, requiresEmailVerification);

    public static UserEmailVerifiedEvent CreateEmailVerified(
        Guid userId,
        string email,
        string verificationToken = null)
        => UserEmailVerifiedEventFactory.CreateEmailVerified(userId, email, verificationToken);

    public static UserRegistrationFailedEvent CreateRegistrationFailed(
        string email,
        string username,
        string failureReason,
        string registrationMethod = "Email",
        string ipAddress = null,
        Dictionary<string, object> additionalData = null)
        => UserRegistrationFailedEventFactory.CreateRegistrationFailed(
            email, username, failureReason, registrationMethod, ipAddress, additionalData);

    public static UserCreatedEvent CreateUserCreated(
        Guid userId,
        string email,
        string username,
        string firstName,
        string lastName,
        Guid? createdBy = null,
        string creationMethod = "Registration",
        bool isActive = true,
        IEnumerable<string> initialRoles = null,
        Dictionary<string, object> metadata = null)
        => UserCreatedEventFactory.CreateUserCreated(
            userId, email, username, firstName, lastName, 
            createdBy, creationMethod, isActive, initialRoles, metadata);

    public static UserActivatedEvent CreateUserActivated(
        Guid userId,
        Guid? activatedBy = null,
        string activationReason = null)
        => UserActivatedEventFactory.CreateUserActivated(userId, activatedBy, activationReason);

    public static UserDeactivatedEvent CreateUserDeactivated(
        Guid userId,
        Guid? deactivatedBy = null,
        string deactivationReason = null)
        => UserDeactivatedEventFactory.CreateUserDeactivated(userId, deactivatedBy, deactivationReason);

    public static UserDeletedEvent CreateUserDeleted(
        Guid userId,
        string email,
        string username,
        Guid? deletedBy = null,
        string deletionReason = null,
        bool isSoftDelete = true)
        => UserDeletedEventFactory.CreateUserDeleted(
            userId, email, username, deletedBy, deletionReason, isSoftDelete);

    public static UserLockedEvent CreateUserLocked(
        Guid userId,
        string email,
        string username,
        LockoutType lockoutType,
        string lockoutReason,
        DateTime? lockoutEndTime = null,
        Guid? lockedBy = null,
        string ipAddress = null,
        int failedAttempts = 0,
        Dictionary<string, object> additionalData = null)
        => UserLockedEventFactory.CreateUserLocked(
            userId, email, username, lockoutType, lockoutReason, 
            lockoutEndTime, lockedBy, ipAddress, failedAttempts, additionalData);

    public static UserUnlockedEvent CreateUserUnlocked(
        Guid userId,
        string email,
        string username,
        Guid? unlockedBy = null,
        string unlockReason = null,
        LockoutType previousLockoutType = LockoutType.Temporary,
        TimeSpan? lockoutDuration = null)
        => UserUnlockedEventFactory.CreateUserUnlocked(
            userId, email, username, unlockedBy, unlockReason, previousLockoutType, lockoutDuration);

    public static UserLoginFailedEvent CreateLoginFailed(
        Guid? userId,
        string email,
        string username,
        string failureReason,
        string ipAddress = null,
        string userAgent = null,
        int totalFailedAttempts = 1,
        bool isAccountLocked = false)
        => UserLoginFailedEventFactory.CreateLoginFailed(
            userId, email, username, failureReason, ipAddress, userAgent, totalFailedAttempts, isAccountLocked);

    public static SuspiciousLoginAttemptEvent CreateSuspiciousLoginAttempt(
        Guid userId,
        string email,
        string suspiciousActivity,
        string ipAddress = null,
        string userAgent = null,
        string location = null,
        LoginRiskLevel riskLevel = LoginRiskLevel.Medium,
        Dictionary<string, object> suspiciousIndicators = null)
        => SuspiciousLoginAttemptEventFactory.CreateSuspiciousLoginAttempt(
            userId, email, suspiciousActivity, ipAddress, userAgent, location, riskLevel, suspiciousIndicators);

    public static UserLoggedInEvent CreateUserLoggedIn(
        Guid userId,
        string email,
        string username,
        string ipAddress = null,
        string userAgent = null,
        string location = null,
        string loginMethod = "Password",
        bool isMfaVerified = false,
        Dictionary<string, object> metadata = null)
        => UserLoggedInEventFactory.CreateUserLoggedIn(
            userId, email, username, ipAddress, userAgent, location, loginMethod, isMfaVerified, metadata);

    public static UserLoggedOutEvent CreateUserLoggedOut(
        Guid userId,
        string email,
        string logoutReason = "UserInitiated",
        TimeSpan? sessionDuration = null)
        => UserLoggedOutEventFactory.CreateUserLoggedOut(userId, email, logoutReason, sessionDuration);

    public static UserPasswordChangedEvent CreatePasswordChanged(
        Guid userId,
        string email,
        Guid? changedBy = null,
        string changeReason = "UserRequested",
        bool wasExpired = false,
        bool wasReset = false)
        => UserPasswordChangedEventFactory.CreatePasswordChanged(
            userId, email, changedBy, changeReason, wasExpired, wasReset);

    public static PasswordResetRequestedEvent CreatePasswordResetRequested(
        Guid userId,
        string email,
        string resetToken,
        string ipAddress = null,
        DateTime? expirationDate = null)
        => PasswordResetRequestedEventFactory.CreatePasswordResetRequested(
            userId, email, resetToken, ipAddress, expirationDate);

    public static UserMfaEnabledEvent CreateMfaEnabled(
        Guid userId,
        string email,
        string mfaMethod,
        Guid? enabledBy = null)
        => UserMfaEnabledEventFactory.CreateMfaEnabled(userId, email, mfaMethod, enabledBy);

    public static UserProfileUpdatedEvent CreateProfileUpdated(
        Guid userId,
        string email,
        Dictionary<string, object> changedFields,
        Guid? updatedBy = null,
        string updateReason = null)
        => UserProfileUpdatedEventFactory.CreateProfileUpdated(
            userId, email, changedFields, updatedBy, updateReason);

    public static UserEmailChangedEvent CreateEmailChanged(
        Guid userId,
        string oldEmail,
        string newEmail,
        bool requiresVerification = true,
        Guid? changedBy = null)
        => UserEmailChangedEventFactory.CreateEmailChanged(
            userId, oldEmail, newEmail, requiresVerification, changedBy);

    public static UserRolesChangedEvent CreateRolesChanged(
        Guid userId,
        string email,
        IEnumerable<string> oldRoles,
        IEnumerable<string> newRoles,
        Guid changedBy,
        string changeReason = null)
        => UserRolesChangedEventFactory.CreateRolesChanged(
            userId, email, oldRoles, newRoles, changedBy, changeReason);
}