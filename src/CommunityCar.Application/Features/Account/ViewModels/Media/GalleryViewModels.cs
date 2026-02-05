using Microsoft.AspNetCore.Http;

namespace CommunityCar.Application.Features.Account.ViewModels.Media;

/// <summary>
/// Gallery and Media ViewModels
/// 
/// This file has been reorganized for better maintainability. Each ViewModel and Request model
/// is now in its own separate file within this namespace. This improves code organization,
/// reduces merge conflicts, and makes the codebase easier to navigate.
/// 
/// Individual files:
/// - UserGalleryItemVM.cs - Gallery item with media details and view compatibility
/// - UploadImageRequest.cs - Request model for uploading images
/// - AddGalleryItemRequest.cs - Request model for adding gallery items (inherits from UploadImageRequest)
/// - UpdateGalleryItemRequest.cs - Request model for updating gallery items
/// - GalleryTagsVM.cs - Gallery tags with usage statistics
/// - TagUsageVM.cs - Individual tag usage information
/// - GalleryCollectionVM.cs - Collection of gallery items with pagination
/// 
/// All classes maintain their original functionality and public API.
/// </summary>

// The individual classes are now in separate files within this namespace.
// This file is kept for documentation and backward compatibility.