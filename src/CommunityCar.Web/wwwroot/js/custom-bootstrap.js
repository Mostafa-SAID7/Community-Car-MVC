// DEPRECATED: This file is no longer used since Bootstrap has been restored
// Keeping for reference only - Bootstrap 5.3.0 is now loaded via CDN
// All functionality has been replaced by native Bootstrap components

// Custom Bootstrap replacement functionality

// Modal functionality
class CustomModal {
    constructor(element) {
        this.element = typeof element === 'string' ? document.getElementById(element) : element;
        this.isOpen = false;
        this.init();
    }

    init() {
        // Add close button functionality
        const closeButtons = this.element.querySelectorAll('.modal-close, [data-dismiss="modal"]');
        closeButtons.forEach(btn => {
            btn.addEventListener('click', () => this.hide());
        });

        // Close on backdrop click
        this.element.addEventListener('click', (e) => {
            if (e.target === this.element) {
                this.hide();
            }
        });

        // Close on escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.isOpen) {
                this.hide();
            }
        });
    }

    show() {
        this.element.classList.add('show');
        this.isOpen = true;
        document.body.style.overflow = 'hidden';
        
        // Trigger shown event
        this.element.dispatchEvent(new CustomEvent('shown.modal'));
    }

    hide() {
        this.element.classList.remove('show');
        this.isOpen = false;
        document.body.style.overflow = '';
        
        // Trigger hidden event
        this.element.dispatchEvent(new CustomEvent('hidden.modal'));
    }

    static getInstance(element) {
        const el = typeof element === 'string' ? document.getElementById(element) : element;
        if (!el._customModal) {
            el._customModal = new CustomModal(el);
        }
        return el._customModal;
    }
}

// Toast functionality
class CustomToast {
    constructor(element, options = {}) {
        this.element = typeof element === 'string' ? document.getElementById(element) : element;
        this.options = {
            delay: 5000,
            autohide: true,
            ...options
        };
        this.init();
    }

    init() {
        // Add close button functionality
        const closeButton = this.element.querySelector('.toast-close, [data-dismiss="toast"]');
        if (closeButton) {
            closeButton.addEventListener('click', () => this.hide());
        }
    }

    show() {
        // Ensure toast container exists
        let container = document.getElementById('toastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toastContainer';
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        // Add to container if not already there
        if (!this.element.parentElement || this.element.parentElement !== container) {
            container.appendChild(this.element);
        }

        // Show toast
        this.element.style.display = 'flex';
        
        // Auto hide if enabled
        if (this.options.autohide) {
            setTimeout(() => this.hide(), this.options.delay);
        }

        // Trigger shown event
        this.element.dispatchEvent(new CustomEvent('shown.toast'));
    }

    hide() {
        this.element.style.display = 'none';
        
        // Remove from DOM after animation
        setTimeout(() => {
            if (this.element.parentElement) {
                this.element.parentElement.removeChild(this.element);
            }
        }, 300);

        // Trigger hidden event
        this.element.dispatchEvent(new CustomEvent('hidden.toast'));
    }

    static getInstance(element) {
        const el = typeof element === 'string' ? document.getElementById(element) : element;
        if (!el._customToast) {
            el._customToast = new CustomToast(el);
        }
        return el._customToast;
    }
}

// Dropdown functionality
class CustomDropdown {
    constructor(element) {
        this.element = typeof element === 'string' ? document.querySelector(element) : element;
        this.toggle = this.element.querySelector('.dropdown-toggle, [data-toggle="dropdown"]');
        this.menu = this.element.querySelector('.dropdown-menu');
        this.isOpen = false;
        this.init();
    }

    init() {
        if (this.toggle) {
            this.toggle.addEventListener('click', (e) => {
                e.preventDefault();
                e.stopPropagation();
                this.isOpen ? this.hide() : this.show();
            });
        }

        // Close on outside click
        document.addEventListener('click', (e) => {
            if (this.isOpen && !this.element.contains(e.target)) {
                this.hide();
            }
        });

        // Close on escape key
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.isOpen) {
                this.hide();
            }
        });
    }

    show() {
        this.menu.classList.add('show');
        this.isOpen = true;
        
        // Trigger shown event
        this.element.dispatchEvent(new CustomEvent('shown.dropdown'));
    }

    hide() {
        this.menu.classList.remove('show');
        this.isOpen = false;
        
        // Trigger hidden event
        this.element.dispatchEvent(new CustomEvent('hidden.dropdown'));
    }

    static getInstance(element) {
        const el = typeof element === 'string' ? document.querySelector(element) : element;
        if (!el._customDropdown) {
            el._customDropdown = new CustomDropdown(el);
        }
        return el._customDropdown;
    }
}

// Utility function to create and show toast
function showToast(message, type = 'info', options = {}) {
    const toastId = 'toast-' + Date.now();
    const iconMap = {
        success: 'check-circle',
        error: 'alert-circle',
        warning: 'alert-triangle',
        info: 'info'
    };

    const toast = document.createElement('div');
    toast.id = toastId;
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
        <div class="toast-icon">
            <i data-lucide="${iconMap[type] || 'info'}"></i>
        </div>
        <div class="toast-body">${message}</div>
        <button type="button" class="toast-close">
            <i data-lucide="x"></i>
        </button>
    `;

    const customToast = new CustomToast(toast, options);
    customToast.show();

    // Initialize Lucide icons for the new toast
    if (window.lucide) {
        lucide.createIcons();
    }

    return customToast;
}

// Initialize dropdowns on page load
document.addEventListener('DOMContentLoaded', function() {
    // Initialize all dropdowns
    document.querySelectorAll('.dropdown').forEach(dropdown => {
        new CustomDropdown(dropdown);
    });

    // Initialize all modals
    document.querySelectorAll('.modal').forEach(modal => {
        new CustomModal(modal);
    });
});

// Global object to replace Bootstrap
window.bootstrap = {
    Modal: CustomModal,
    Toast: CustomToast,
    Dropdown: CustomDropdown
};

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { CustomModal, CustomToast, CustomDropdown, showToast };
}