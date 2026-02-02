/**
 * CC.Utils.Validation
 * Modular validation utilities for forms and inputs.
 */
(function () {
    class ValidationUtils {
        constructor() {
            this.emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        }

        /**
         * Validates an email address.
         * @param {string} email 
         * @returns {boolean}
         */
        isValidEmail(email) {
            return this.emailRegex.test(email);
        }

        /**
         * Validates password strength.
         * Returns an object with score (0-5) and requirement status.
         * @param {string} password 
         * @returns {object}
         */
        validatePasswordStrength(password) {
            const requirements = {
                length: password.length >= 8,
                lowercase: /[a-z]/.test(password),
                uppercase: /[A-Z]/.test(password),
                number: /\d/.test(password),
                special: /[@$!%*?&]/.test(password)
            };

            const score = Object.values(requirements).filter(Boolean).length;
            return { score, requirements };
        }

        /**
         * Sets up password strength indicator for an input.
         * @param {HTMLInputElement} input 
         * @param {HTMLElement} indicatorContainer 
         */
        setupStrengthIndicator(input, indicatorContainer) {
            if (!input || !indicatorContainer) return;

            input.addEventListener('input', () => {
                const value = input.value;
                if (value.length > 0) {
                    indicatorContainer.classList.remove('hidden');
                    const { score, requirements } = this.validatePasswordStrength(value);
                    this.updateStrengthUI(indicatorContainer, score, requirements);
                } else {
                    indicatorContainer.classList.add('hidden');
                }
            });
        }

        /**
         * Updates the UI of a strength indicator.
         */
        updateStrengthUI(container, score, requirements) {
            // Update requirement icons
            Object.keys(requirements).forEach(req => {
                const item = container.querySelector(`[data-requirement="${req}"]`);
                if (item) {
                    const icon = item.querySelector('i');
                    const isMet = requirements[req];

                    item.classList.toggle('text-primary', isMet);
                    item.classList.toggle('font-medium', isMet);
                    item.classList.toggle('text-muted-foreground/60', !isMet);

                    if (icon) {
                        icon.setAttribute('data-lucide', isMet ? 'check-circle-2' : 'circle');
                        icon.classList.toggle('text-primary', isMet);
                        icon.classList.toggle('text-border', !isMet);
                    }
                }
            });

            if (window.lucide) window.lucide.createIcons({ parent: container });

            // Update fill bar
            const fill = container.querySelector('.strength-fill');
            const label = container.querySelector('.strength-level');
            if (fill && label) {
                const percentage = (score / 5) * 100;
                fill.style.width = `${percentage}%`;

                // Color based on score
                const colors = [
                    'hsl(var(--destructive))',
                    'hsl(38, 92%, 50%)',
                    'hsl(262, 83%, 58%)',
                    'hsl(142, 76%, 36%)',
                    'hsl(var(--primary))'
                ];
                fill.style.backgroundColor = colors[Math.min(score, 4)];
            }
        }

        /**
         * Shows validation error for a field.
         */
        showError(field, message) {
            field.classList.add('input-validation-error');
            let errorContainer = field.parentNode.querySelector('.field-validation-error');
            if (!errorContainer) {
                errorContainer = document.createElement('span');
                errorContainer.className = 'field-validation-error text-xs text-destructive mt-1 block';
                field.parentNode.appendChild(errorContainer);
            }
            errorContainer.textContent = message;
        }

        /**
         * Clears validation error for a field.
         */
        clearError(field) {
            field.classList.remove('input-validation-error');
            const errorContainer = field.parentNode.querySelector('.field-validation-error');
            if (errorContainer) errorContainer.textContent = '';
        }
    }

    CC.Utils.Validation = new ValidationUtils();
})();
