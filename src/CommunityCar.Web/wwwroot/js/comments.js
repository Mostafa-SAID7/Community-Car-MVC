/**
 * comments.js
 * Handles comment list loading, infinite scroll, form submission, and interactions.
 */

(function () {
    const Comments = {
        init() {
            this.initCommentsList();
            this.initCommentForms();
        },

        initCommentsList() {
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }

            // Toggle comment form when clicking comment button
            if (!window.commentFormToggleInitialized) {
                document.addEventListener('click', (e) => {
                    const commentBtn = e.target.closest('[data-button-type="comment"]');
                    if (commentBtn) {
                        const entityId = commentBtn.dataset.entityId;
                        const formWrap = document.querySelector(`.comment-form-wrap[data-entity-id="${entityId}"]`);
                        if (formWrap) {
                            formWrap.classList.toggle('hidden');
                            const textarea = formWrap.querySelector('.comment-textarea');
                            if (textarea && !formWrap.classList.contains('hidden')) {
                                textarea.focus();
                            }
                        }
                    }
                });
                window.commentFormToggleInitialized = true;
            }

            // Load initial comments (beyond first 3)
            const loadInitialButtons = document.querySelectorAll('.load-initial-comments-btn:not([data-initialized])');
            loadInitialButtons.forEach(btn => {
                btn.addEventListener('click', function () {
                    const entityId = this.dataset.entityId;
                    const entityType = this.dataset.entityType;
                    const getPartialUrl = this.dataset.partialUrl;
                    const container = this.closest('.comments-section-container').querySelector('.comments-list-container');

                    this.disabled = true;
                    const originalContent = this.innerHTML;
                    this.innerHTML = '<i class="w-4 h-4 mr-2 animate-spin">⟳</i>Loading...';

                    fetch(`${getPartialUrl}?entityId=${entityId}&entityType=${entityType}&page=1&pageSize=100`)
                        .then(response => {
                            const hasMore = response.headers.get('X-Has-More') === 'true';
                            const totalPages = parseInt(response.headers.get('X-Total-Pages') || '1');

                            // Show load more container if there are more pages
                            if (hasMore && totalPages > 1) {
                                const loadMoreContainer = this.closest('.comments-section-container').querySelector('.load-more-container');
                                if (loadMoreContainer) {
                                    loadMoreContainer.classList.remove('hidden');
                                    const loadMoreBtn = loadMoreContainer.querySelector('.load-more-comments-btn');
                                    if (loadMoreBtn) {
                                        loadMoreBtn.dataset.page = '2';
                                    }
                                }
                            }
                            return response.text();
                        })
                        .then(html => {
                            const tempDiv = document.createElement('div');
                            tempDiv.innerHTML = html;
                            const newList = tempDiv.querySelector('.comments-list');
                            const currentList = container.querySelector('.comments-list');
                            if (newList && currentList) {
                                currentList.replaceWith(newList);
                            }

                            this.parentElement.classList.add('hidden');

                            if (typeof lucide !== 'undefined') {
                                lucide.createIcons();
                            }
                        })
                        .catch(error => {
                            console.error('Error loading comments:', error);
                            this.innerHTML = originalContent;
                            this.disabled = false;
                        });
                });
                btn.setAttribute('data-initialized', 'true');
            });

            // Global delegate for delete and reply buttons
            if (!window.commentsDelegateInitialized) {
                document.addEventListener('click', (e) => {
                    // Delete logic
                    const deleteBtn = e.target.closest('.delete-btn');
                    if (deleteBtn) {
                        const commentId = deleteBtn.dataset.commentId;
                        const deleteUrl = deleteBtn.dataset.deleteUrl;

                        if (window.AlertModal) {
                            window.AlertModal.confirm('Are you sure you want to delete this comment?', 'Delete Comment', (confirmed) => {
                                if (confirmed) {
                                    this.executeDelete(commentId, deleteUrl);
                                }
                            });
                        } else if (confirm('Are you sure you want to delete this comment?')) {
                            this.executeDelete(commentId, deleteUrl);
                        }
                    }

                    // Reply logic
                    const replyBtn = e.target.closest('.reply-btn');
                    if (replyBtn) {
                        const commentId = replyBtn.dataset.commentId;
                        const authorName = replyBtn.dataset.authorName;
                        const container = replyBtn.closest('.comments-section-container');
                        const formWrap = container.querySelector('.comment-form-wrap');
                        const formElement = container.querySelector('.comment-form-element');
                        const formContainer = container.querySelector('.comment-form');

                        if (formWrap && formWrap.classList.contains('hidden')) {
                            formWrap.classList.remove('hidden');
                        }

                        if (formElement && formContainer) {
                            const parentIdInput = formElement.querySelector('.parent-comment-id-input');
                            const replyInfo = formContainer.querySelector('.reply-info');
                            const replyToName = formContainer.querySelector('.reply-to-name');
                            const textarea = formElement.querySelector('.comment-textarea');
                            const cancelBtn = formElement.querySelector('.cancel-btn');

                            if (parentIdInput && replyInfo && replyToName && textarea) {
                                parentIdInput.value = commentId;
                                replyToName.innerText = authorName;
                                replyInfo.classList.remove('hidden');
                                textarea.placeholder = `Reply to ${authorName}...`;
                                if (cancelBtn) cancelBtn.classList.remove('hidden');

                                formContainer.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                textarea.focus();
                            }
                        }
                    }
                });
                window.commentsDelegateInitialized = true;
            }

            // Initialize Load More buttons
            const loadMoreButtons = document.querySelectorAll('.load-more-comments-btn:not([data-initialized])');
            loadMoreButtons.forEach(btn => {
                btn.addEventListener('click', function () {
                    const entityId = this.dataset.entityId;
                    const entityType = this.dataset.entityType;
                    const page = this.dataset.page;
                    const getPartialUrl = this.dataset.partialUrl;
                    const container = this.closest('.comments-section-container').querySelector('.comments-list-container .comments-list');

                    this.disabled = true;
                    const originalContent = this.innerHTML;
                    this.innerHTML = '<i class="w-4 h-4 mr-2 animate-spin">⟳</i>Loading...';

                    fetch(`${getPartialUrl}?entityId=${entityId}&entityType=${entityType}&page=${page}`)
                        .then(response => {
                            const hasMore = response.headers.get('X-Has-More') === 'true';
                            if (!hasMore) {
                                this.parentElement.classList.add('hidden');
                            }
                            return response.text();
                        })
                        .then(html => {
                            const tempDiv = document.createElement('div');
                            tempDiv.innerHTML = html;

                            while (tempDiv.firstChild) {
                                container.appendChild(tempDiv.firstChild);
                            }

                            this.dataset.page = parseInt(page) + 1;

                            if (typeof lucide !== 'undefined') {
                                lucide.createIcons();
                            }
                        })
                        .catch(error => {
                            console.error('Error loading more comments:', error);
                        })
                        .finally(() => {
                            this.disabled = false;
                            this.innerHTML = originalContent;
                        });
                });
                btn.setAttribute('data-initialized', 'true');
            });
        },

        executeDelete(commentId, deleteUrl) {
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = deleteUrl;

            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
            if (tokenElement) {
                const tokenInput = document.createElement('input');
                tokenInput.type = 'hidden';
                tokenInput.name = '__RequestVerificationToken';
                tokenInput.value = tokenElement.value;
                form.appendChild(tokenInput);
            }

            const idInput = document.createElement('input');
            idInput.type = 'hidden';
            idInput.name = 'id';
            idInput.value = commentId;
            form.appendChild(idInput);

            const returnUrlInput = document.createElement('input');
            returnUrlInput.type = 'hidden';
            returnUrlInput.name = 'returnUrl';
            returnUrlInput.value = window.location.href;
            form.appendChild(returnUrlInput);

            document.body.appendChild(form);
            form.submit();
        },

        initCommentForms() {
            const forms = document.querySelectorAll('.comment-form-element:not([data-initialized])');

            forms.forEach(form => {
                const container = form.closest('.comment-form');
                const textarea = form.querySelector('.comment-textarea');
                const cancelBtn = form.querySelector('.cancel-btn');
                const submitBtn = form.querySelector('.submit-comment-btn');
                const parentIdInput = form.querySelector('.parent-comment-id-input');
                const replyInfo = container.querySelector('.reply-info');
                const replyToName = container.querySelector('.reply-to-name');
                const cancelReplyBtn = container.querySelector('.cancel-reply-btn');
                const mentionList = container.querySelector('#mention-suggestions');

                let mentionQuery = '';
                let isMentioning = false;
                let mentionStartIndex = -1;

                const resetForm = () => {
                    textarea.value = '';
                    cancelBtn.classList.add('hidden');
                    parentIdInput.value = '';
                    if (replyInfo) replyInfo.classList.add('hidden');
                    textarea.placeholder = "Write a comment...";
                    hideMentions();
                    textarea.blur();
                };

                const hideMentions = () => {
                    isMentioning = false;
                    if (mentionList) mentionList.classList.add('hidden');
                };

                const showMentions = (users) => {
                    if (!mentionList || users.length === 0) {
                        hideMentions();
                        return;
                    }

                    mentionList.innerHTML = users.map(u => `
                        <div class="mention-item" data-user-name="${u.name}">
                            <img src="${u.avatar}" class="w-6 h-6 rounded-full" />
                            <span class="text-sm">${u.name}</span>
                        </div>
                    `).join('');

                    mentionList.classList.remove('hidden');

                    mentionList.querySelectorAll('.mention-item').forEach(item => {
                        item.addEventListener('click', function () {
                            const name = this.dataset.userName;
                            const before = textarea.value.substring(0, mentionStartIndex);
                            const after = textarea.value.substring(textarea.selectionStart);
                            textarea.value = before + '@' + name + ' ' + after;
                            hideMentions();
                            textarea.focus();
                        });
                    });
                };

                if (textarea) {
                    textarea.addEventListener('input', function (e) {
                        const value = this.value;
                        const cursor = this.selectionStart;
                        const charAtCursor = value[cursor - 1];

                        if (charAtCursor === '@') {
                            isMentioning = true;
                            mentionStartIndex = cursor - 1;
                            mentionQuery = '';
                        } else if (isMentioning) {
                            if (charAtCursor === ' ' || cursor < mentionStartIndex) {
                                hideMentions();
                            } else {
                                mentionQuery = value.substring(mentionStartIndex + 1, cursor);
                                if (mentionQuery.length >= 2) {
                                    fetch(`/Comments/SearchUsers?query=${encodeURIComponent(mentionQuery)}`)
                                        .then(r => r.json())
                                        .then(users => showMentions(users));
                                }
                            }
                        }

                        if (this.value.trim() || (parentIdInput && parentIdInput.value)) {
                            if (cancelBtn) cancelBtn.classList.remove('hidden');
                        } else {
                            if (cancelBtn) cancelBtn.classList.add('hidden');
                        }
                    });
                }

                if (cancelBtn) {
                    cancelBtn.addEventListener('click', resetForm);
                }

                if (cancelReplyBtn) {
                    cancelReplyBtn.addEventListener('click', function () {
                        parentIdInput.value = '';
                        replyInfo.classList.add('hidden');
                        textarea.placeholder = "Write a comment...";
                        if (!textarea.value.trim()) {
                            cancelBtn.classList.add('hidden');
                        }
                    });
                }

                form.addEventListener('submit', function (e) {
                    e.preventDefault();

                    const formData = new FormData(this);
                    const originalText = submitBtn.innerHTML;

                    submitBtn.disabled = true;
                    submitBtn.innerHTML = '<i class="w-4 h-4 mr-2 animate-spin">⟳</i>Posting...';

                    fetch(this.action, {
                        method: 'POST',
                        body: formData,
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    })
                        .then(async response => {
                            if (!response.ok) {
                                const text = await response.text();
                                throw new Error(text || `Server returned ${response.status}`);
                            }
                            return response.json();
                        })
                        .then(data => {
                            if (data.success) {
                                resetForm();

                                const entityId = form.querySelector('input[name="entityId"]').value;
                                const statsBox = document.querySelector(`.comments-section-container[data-entity-id="${entityId}"]`)?.parentElement?.querySelector(`[data-stats-type="comments"][data-entity-id="${entityId}"] span`);
                                if (statsBox) {
                                    const currentCount = parseInt(statsBox.innerText.replace(/,/g, '')) || 0;
                                    statsBox.innerText = (currentCount + 1).toLocaleString();
                                }

                                const commentsList = form.closest('.comments-section-container')?.querySelector('.comments-list');
                                if (commentsList) {
                                    const emptyState = commentsList.closest('.comments-list-container')?.querySelector('.text-center.py-8');
                                    if (emptyState) emptyState.remove();

                                    const comment = data.comment;
                                    const parentId = parentIdInput.value;

                                    if (document.querySelector(`[data-comment-id="${comment.id}"]`)) {
                                        return;
                                    }

                                    let content = comment.content;
                                    content = content.replace(/@([\w\s]+)/g, '<span class="text-primary font-bold cursor-pointer hover:underline">@$1</span>');

                                    if (parentId) {
                                        const parentContainer = document.querySelector(`.comment-item[data-comment-id="${parentId}"]`)?.closest('.comment-item-container');
                                        if (parentContainer) {
                                            let repliesList = parentContainer.querySelector('.replies-container');
                                            if (!repliesList) {
                                                repliesList = document.createElement('div');
                                                repliesList.className = 'replies-container ml-8 space-y-3 border-l-2 border-muted pl-4 mt-3';
                                                parentContainer.appendChild(repliesList);
                                            }

                                            const replyHtml = `
                                                <div class="reply-item flex gap-3 p-3 bg-muted/10 rounded-lg animate-in fade-in slide-in-from-top-2 duration-300" data-comment-id="${comment.id}">
                                                    <div class="flex-shrink-0">
                                                        <img src="${comment.authorAvatar}" 
                                                             alt="${comment.authorName}" 
                                                             class="w-8 h-8 rounded-full object-cover" />
                                                    </div>
                                                    <div class="flex-1 text-sm">
                                                        <div class="flex items-center gap-2 mb-1">
                                                            <span class="font-medium text-foreground">${comment.authorName}</span>
                                                            <span class="text-xs text-muted-foreground">${comment.timeAgo}</span>
                                                        </div>
                                                        <div class="text-foreground mb-1">
                                                            ${content}
                                                        </div>
                                                        <div class="flex items-center gap-3">
                                                            <button class="text-muted-foreground hover:text-primary transition-colors reply-btn text-[10px] uppercase font-bold" 
                                                                    data-comment-id="${parentId}"
                                                                    data-author-name="${comment.authorName}">
                                                                Reply
                                                            </button>
                                                            <button class="text-muted-foreground hover:text-red-600 transition-colors delete-btn text-[10px] uppercase font-bold" 
                                                                    data-comment-id="${comment.id}"
                                                                    data-delete-url="${form.action.replace('/Add', '/Delete')}">
                                                                Delete
                                                            </button>
                                                        </div>
                                                    </div>
                                                </div>
                                            `;
                                            repliesList.insertAdjacentHTML('beforeend', replyHtml);
                                        } else {
                                            location.reload();
                                        }
                                    } else {
                                        const commentHtml = `
                                            <div class="comment-item-container space-y-3 animate-in fade-in slide-in-from-top-2 duration-300">
                                                <div class="comment-item flex gap-3 p-4 bg-muted/30 rounded-lg" data-comment-id="${comment.id}">
                                                    <div class="flex-shrink-0">
                                                        <img src="${comment.authorAvatar}" 
                                                             alt="${comment.authorName}" 
                                                             class="w-10 h-10 rounded-full object-cover" />
                                                    </div>
                                                    <div class="flex-1">
                                                        <div class="flex items-center gap-2 mb-2">
                                                            <span class="font-medium text-foreground">${comment.authorName}</span>
                                                            <span class="text-sm text-muted-foreground">${comment.timeAgo}</span>
                                                        </div>
                                                        <div class="text-foreground mb-2">
                                                            ${content}
                                                        </div>
                                                        <div class="flex items-center gap-4 text-sm">
                                                            <button class="text-muted-foreground hover:text-primary transition-colors reply-btn" data-comment-id="${comment.id}" data-author-name="${comment.authorName}">
                                                                <i data-lucide="reply" class="w-4 h-4 inline mr-1"></i>
                                                                Reply
                                                            </button>
                                                            <button class="text-muted-foreground hover:text-red-600 transition-colors delete-btn" 
                                                                    data-comment-id="${comment.id}"
                                                                    data-delete-url="${form.action.replace('/Add', '/Delete')}">
                                                                <i data-lucide="trash-2" class="w-4 h-4 inline mr-1"></i>
                                                                Delete
                                                            </button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        `;
                                        commentsList.insertAdjacentHTML('afterbegin', commentHtml);
                                    }

                                    if (typeof lucide !== 'undefined') {
                                        lucide.createIcons();
                                    }
                                } else {
                                    location.reload();
                                }
                            } else {
                                if (window.AlertModal) {
                                    window.AlertModal.error(data.message || 'Failed to post comment', 'Error');
                                } else {
                                    alert(data.message || 'Failed to post comment');
                                }
                            }
                        })
                        .catch(error => {
                            console.error('Error:', error);
                            if (window.AlertModal) {
                                window.AlertModal.error('An error occurred while posting your comment: ' + error.message, 'Error');
                            } else {
                                alert('An error occurred while posting your comment: ' + error.message);
                            }
                        })
                        .finally(() => {
                            submitBtn.disabled = false;
                            submitBtn.innerHTML = originalText;
                        });
                });

                form.setAttribute('data-initialized', 'true');
            });
        }
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => Comments.init());
    } else {
        Comments.init();
    }
})();
