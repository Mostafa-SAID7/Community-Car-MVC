// Client-side validation for authentication forms
class AuthValidation {
    constructor() {
        this.initializeValidation();
    }

    initializeValidation() {
        // Password strength validation
        this.setupPasswordValidation();

        // Email validation
        this.setupEmailValidation();

        // Real-time form validation
        this.setupRealTimeValidation();

        // 2FA code validation
        this.setupTwoFactorValidation();
    }

    setupPasswordValidation() {
        // Updated selector to look for our new data attribute
        const passwordInputs = document.querySelectorAll('input[type="password"][data-password-strength="true"]');

        passwordInputs.forEach(input => {
            const strengthIndicator = this.createPasswordStrengthIndicator(input);

            input.addEventListener('input', (e) => {
                const value = e.target.value;
                if (value.length > 0) {
                    strengthIndicator.classList.remove('hidden');
                    strengthIndicator.classList.add('flex'); // Indicator uses flex layout internally
                    strengthIndicator.classList.add('fade-in');
                    this.validatePasswordStrength(e.target, strengthIndicator);
                } else {
                    strengthIndicator.classList.add('hidden');
                    strengthIndicator.classList.remove('flex');
                }
            });

            // Password confirmation matching
            const confirmInput = document.querySelector(`input[name="ConfirmPassword"]`);
            if (confirmInput) {
                confirmInput.addEventListener('input', () => {
                    this.validatePasswordMatch(input, confirmInput);
                });
                input.addEventListener('input', () => {
                    if (confirmInput.value.length > 0) {
                        this.validatePasswordMatch(input, confirmInput);
                    }
                });
            }
        });
    }

    createPasswordStrengthIndicator(passwordInput) {
        const data = passwordInput.dataset;
        const indicator = document.createElement('div');
        indicator.className = 'password-strength-indicator mt-3 p-4 bg-background/40 border border-border/50 rounded-2xl hidden';
        indicator.innerHTML = `
            <div class="flex items-center justify-between mb-2">
                <span class="text-[10px] font-bold text-muted-foreground uppercase tracking-widest">${data.strengthLabel || 'Password strength'}:</span>
                <span class="strength-level text-[10px] font-bold uppercase tracking-widest"></span>
            </div>
            <div class="h-1.5 w-full bg-border/20 rounded-full overflow-hidden mb-4">
                <div class="strength-fill h-full transition-all duration-500 ease-out w-0"></div>
            </div>
            <ul class="flex flex-col sm:grid sm:grid-cols-2 gap-x-4 gap-y-1.5 sm:gap-y-2">
                <li data-requirement="length" class="flex items-center gap-2 text-[10px] text-muted-foreground/60 transition-colors py-0.5">
                    <i data-lucide="circle" class="w-3 h-3 text-border flex-shrink-0"></i> <span>${data.reqLength}</span>
                </li>
                <li data-requirement="lowercase" class="flex items-center gap-2 text-[10px] text-muted-foreground/60 transition-colors py-0.5">
                    <i data-lucide="circle" class="w-3 h-3 text-border flex-shrink-0"></i> <span>${data.reqLower}</span>
                </li>
                <li data-requirement="uppercase" class="flex items-center gap-2 text-[10px] text-muted-foreground/60 transition-colors py-0.5">
                    <i data-lucide="circle" class="w-3 h-3 text-border flex-shrink-0"></i> <span>${data.reqUpper}</span>
                </li>
                <li data-requirement="number" class="flex items-center gap-2 text-[10px] text-muted-foreground/60 transition-colors py-0.5">
                    <i data-lucide="circle" class="w-3 h-3 text-border flex-shrink-0"></i> <span>${data.reqNumber}</span>
                </li>
                <li data-requirement="special" class="flex items-center gap-2 text-[10px] text-muted-foreground/60 transition-colors py-0.5">
                    <i data-lucide="circle" class="w-3 h-3 text-border flex-shrink-0"></i> <span>${data.reqSpecial}</span>
                </li>
            </ul>
        `;

        passwordInput.parentNode.parentNode.appendChild(indicator);
        if (window.lucide) window.lucide.createIcons({ parent: indicator });
        return indicator;
    }

    validatePasswordStrength(passwordInput, indicator) {
        const data = passwordInput.dataset;
        const password = passwordInput.value;
        const requirements = {
            length: password.length >= 8,
            lowercase: /[a-z]/.test(password),
            uppercase: /[A-Z]/.test(password),
            number: /\d/.test(password),
            special: /[@$!%*?&]/.test(password)
        };

        let score = 0;

        Object.keys(requirements).forEach(req => {
            const element = indicator.querySelector(`[data-requirement="${req}"]`);
            const icon = element.querySelector('i');
            if (requirements[req]) {
                element.classList.remove('text-muted-foreground/60');
                element.classList.add('text-primary', 'font-medium');
                icon.setAttribute('data-lucide', 'check-circle-2');
                icon.classList.remove('text-border');
                icon.classList.add('text-primary');
                score++;
            } else {
                element.classList.add('text-muted-foreground/60');
                element.classList.remove('text-primary', 'font-medium');
                icon.setAttribute('data-lucide', 'circle');
                icon.classList.add('text-border');
                icon.classList.remove('text-primary');
            }
        });

        if (window.lucide) window.lucide.createIcons({ parent: indicator });

        const strengthFill = indicator.querySelector('.strength-fill');
        const strengthLevel = indicator.querySelector('.strength-level');

        const levels = [
            { text: data.vweak, color: 'hsl(var(--destructive))' }, // Very Weak
            { text: data.weak, color: 'hsl(var(--chart-warning, 38 92% 50%))' },    // Weak
            { text: data.fair, color: 'hsl(var(--chart-info, 262 83% 58%))' },      // Fair
            { text: data.good, color: 'hsl(var(--chart-success, 142 76% 36%))' },   // Good
            { text: data.strong, color: 'hsl(var(--primary))' }     // Strong
        ];

        const currentLevel = levels[Math.min(score, 4)];

        strengthFill.style.width = `${(score / 5) * 100}%`;
        strengthFill.style.backgroundColor = currentLevel.color;
        strengthLevel.textContent = currentLevel.text;
        strengthLevel.style.color = currentLevel.color;
    }

    validatePasswordMatch(passwordInput, confirmInput) {
        if (confirmInput.value === passwordInput.value && confirmInput.value !== '') {
            confirmInput.classList.remove('border-destructive');
            confirmInput.classList.add('border-primary');
        } else if (confirmInput.value !== '') {
            confirmInput.classList.remove('border-primary');
            confirmInput.classList.add('border-destructive');
        }
    }

    setupEmailValidation() {
        const emailInputs = document.querySelectorAll('input[type="email"]');

        emailInputs.forEach(input => {
            input.addEventListener('blur', (e) => {
                this.validateEmail(e.target);
            });
        });
    }

    validateEmail(emailInput) {
        const email = emailInput.value;
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        const isValid = emailRegex.test(email);
        const errorElement = emailInput.parentNode.querySelector('.field-validation-error');

        if (!isValid && email.length > 0) {
            this.showFieldError(emailInput, 'Please enter a valid email address');
        } else if (errorElement) {
            errorElement.textContent = '';
            emailInput.classList.remove('input-validation-error');
        }
    }

    setupRealTimeValidation() {
        const forms = document.querySelectorAll('form[data-ajax="false"], form:not([data-ajax])');

        forms.forEach(form => {
            const inputs = form.querySelectorAll('input[data-val="true"]');

            inputs.forEach(input => {
                input.addEventListener('blur', (e) => {
                    this.validateField(e.target);
                });

                input.addEventListener('input', (e) => {
                    // Clear errors on input
                    this.clearFieldError(e.target);
                });
            });
        });
    }

    setupTwoFactorValidation() {
        const twoFactorInputs = document.querySelectorAll('input[data-two-factor="true"]');

        twoFactorInputs.forEach(input => {
            input.addEventListener('input', (e) => {
                // Auto-format and validate 2FA codes
                let value = e.target.value.replace(/\D/g, ''); // Remove non-digits

                if (value.length > 6) {
                    value = value.substring(0, 6);
                }

                e.target.value = value;

                // Auto-submit when 6 digits entered
                if (value.length === 6) {
                    const form = e.target.closest('form');
                    if (form && form.dataset.autoSubmit === 'true') {
                        form.submit();
                    }
                }
            });

            input.addEventListener('paste', (e) => {
                e.preventDefault();
                const paste = (e.clipboardData || window.clipboardData).getData('text');
                const digits = paste.replace(/\D/g, '').substring(0, 6);
                e.target.value = digits;

                if (digits.length === 6) {
                    const form = e.target.closest('form');
                    if (form && form.dataset.autoSubmit === 'true') {
                        form.submit();
                    }
                }
            });
        });
    }

    validateField(field) {
        const validators = field.dataset;
        let isValid = true;
        let errorMessage = '';

        // Required validation
        if (validators.valRequired !== undefined && !field.value.trim()) {
            isValid = false;
            errorMessage = validators.valRequired || 'This field is required';
        }

        // Email validation
        if (isValid && validators.valEmail !== undefined && field.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(field.value)) {
                isValid = false;
                errorMessage = validators.valEmail || 'Please enter a valid email address';
            }
        }

        // Length validation
        if (isValid && validators.valLength !== undefined && field.value) {
            const minLength = parseInt(validators.valLengthMin) || 0;
            const maxLength = parseInt(validators.valLengthMax) || Infinity;

            if (field.value.length < minLength || field.value.length > maxLength) {
                isValid = false;
                errorMessage = validators.valLength || `Length must be between ${minLength} and ${maxLength} characters`;
            }
        }

        // Regex validation
        if (isValid && validators.valRegex !== undefined && field.value) {
            const pattern = new RegExp(validators.valRegexPattern);
            if (!pattern.test(field.value)) {
                isValid = false;
                errorMessage = validators.valRegex || 'Invalid format';
            }
        }

        if (!isValid) {
            this.showFieldError(field, errorMessage);
        } else {
            this.clearFieldError(field);
        }

        return isValid;
    }

    showFieldError(field, message) {
        field.classList.add('input-validation-error');

        let errorElement = field.parentNode.querySelector('.field-validation-error');
        if (!errorElement) {
            errorElement = document.createElement('span');
            errorElement.className = 'field-validation-error';
            field.parentNode.appendChild(errorElement);
        }

        errorElement.textContent = message;
    }

    clearFieldError(field) {
        field.classList.remove('input-validation-error');
        const errorElement = field.parentNode.querySelector('.field-validation-error');
        if (errorElement) {
            errorElement.textContent = '';
        }
    }
}

// Initialize validation when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new AuthValidation();
});

// Export for use in other scripts
window.AuthValidation = AuthValidation;