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
        const passwordInputs = document.querySelectorAll('input[type="password"][data-val-regex]');
        
        passwordInputs.forEach(input => {
            const strengthIndicator = this.createPasswordStrengthIndicator(input);
            
            input.addEventListener('input', (e) => {
                this.validatePasswordStrength(e.target, strengthIndicator);
            });
        });
    }

    createPasswordStrengthIndicator(passwordInput) {
        const indicator = document.createElement('div');
        indicator.className = 'password-strength-indicator';
        indicator.innerHTML = `
            <div class="strength-bar">
                <div class="strength-fill"></div>
            </div>
            <div class="strength-text">Password strength: <span class="strength-level">Weak</span></div>
            <ul class="strength-requirements">
                <li data-requirement="length">At least 8 characters</li>
                <li data-requirement="lowercase">One lowercase letter</li>
                <li data-requirement="uppercase">One uppercase letter</li>
                <li data-requirement="number">One number</li>
                <li data-requirement="special">One special character</li>
            </ul>
        `;
        
        passwordInput.parentNode.insertBefore(indicator, passwordInput.nextSibling);
        return indicator;
    }

    validatePasswordStrength(passwordInput, indicator) {
        const password = passwordInput.value;
        const requirements = {
            length: password.length >= 8,
            lowercase: /[a-z]/.test(password),
            uppercase: /[A-Z]/.test(password),
            number: /\d/.test(password),
            special: /[@$!%*?&]/.test(password)
        };

        let score = 0;
        const requirementElements = indicator.querySelectorAll('[data-requirement]');
        
        Object.keys(requirements).forEach(req => {
            const element = indicator.querySelector(`[data-requirement="${req}"]`);
            if (requirements[req]) {
                element.classList.add('met');
                score++;
            } else {
                element.classList.remove('met');
            }
        });

        // Update strength indicator
        const strengthFill = indicator.querySelector('.strength-fill');
        const strengthLevel = indicator.querySelector('.strength-level');
        
        let strength, color;
        if (score < 2) {
            strength = 'Very Weak';
            color = '#ff4444';
        } else if (score < 3) {
            strength = 'Weak';
            color = '#ff8800';
        } else if (score < 4) {
            strength = 'Fair';
            color = '#ffaa00';
        } else if (score < 5) {
            strength = 'Good';
            color = '#88cc00';
        } else {
            strength = 'Strong';
            color = '#00cc44';
        }

        strengthFill.style.width = `${(score / 5) * 100}%`;
        strengthFill.style.backgroundColor = color;
        strengthLevel.textContent = strength;
        strengthLevel.style.color = color;
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