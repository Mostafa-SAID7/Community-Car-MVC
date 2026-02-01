using CommunityCar.Domain.Enums.Shared;

namespace CommunityCar.Application.Features.Shared.Interactions.ViewModels;

/// <summary>
/// Interaction ViewModels for Reactions, Comments, and Shares
/// 
/// This file has been reorganized for better maintainability. Each ViewModel class
/// is now in its own separate file within this namespace. This improves code organization,
/// reduces merge conflicts, and makes the codebase easier to navigate.
/// 
/// Individual files:
/// - ReactionVM.cs - Individual reaction with user details
/// - ReactionSummaryVM.cs - Summary of all reactions on an entity
/// - ReactionTypeInfoVM.cs - Information about a specific reaction type
/// - ReactionResultVM.cs - Result of a reaction operation
/// - CommentVM.cs - Comment with replies and reactions
/// - ShareVM.cs - Individual share with platform details
/// - ShareSummaryVM.cs - Summary of all shares on an entity
/// - ShareResultVM.cs - Result of a share operation
/// - ShareMetadataVM.cs - Metadata for social media sharing
/// - InteractionSummaryVM.cs - Complete interaction summary (reactions, comments, shares)
/// - CreateCommentVM.cs - View model for creating comments
/// - CreateReplyVM.cs - View model for creating comment replies
/// - ShareEntityVM.cs - View model for sharing entities
/// 
/// All classes maintain their original functionality and public API.
/// </summary>

// The individual classes are now in separate files within this namespace.
// This file is kept for documentation and backward compatibility.