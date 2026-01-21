// Interactions JavaScript - Like, Comment, Share functionality
class InteractionManager {
    constructor() {
        this.initializeEventListeners();
    }

    initializeEventListeners() {
        // Reaction buttons
        document.addEventListener('click', (e) => {
            if (e.target.matches('.reaction-btn')) {
                this.handleReaction(e);
            }
            if (e.target.matches('.comment-btn')) {
                this.toggleCommentSection(e);
            }
            if (e.target.matches('.share-btn')) {
                this.handleShare(e);
            }
            if (e.target.matches('.submit-comment-b