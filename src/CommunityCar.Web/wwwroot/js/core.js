/**
 * Community Car Core
 * Defines the global namespace and core utilities.
 * 
 * Architecture:
 * - CommunityCar.Services: Singleton services for business logic and API interaction.
 * - CommunityCar.Components: UI components and event controllers.
 * - CommunityCar.Utils: specific utility functions.
 * - CommunityCar.Modules: Feature-specific modules.
 */

window.CommunityCar = window.CommunityCar || {
    Services: {},
    Components: {},
    Modules: {},
    Utils: {},
    Events: {}, // Placeholder, will be replaced by EventBus instance
    Config: {
        debug: true
    }
};

// Alias for convenience within the codebase (optional usage)
// Alias for convenience within the codebase
window.CC = window.CommunityCar;

/**
 * Base Service Class
 * Enforces singleton pattern and common initialization logic.
 */
CC.Utils.BaseService = class BaseService {
    constructor(name) {
        if (CC.Services[name]) {
            console.warn(`Service ${name} already initialized. Returning existing instance.`);
            return CC.Services[name];
        }
        this.name = name;
        this.initialized = false;
        CC.Services[name] = this;
    }

    init() {
        if (this.initialized) return;
        this.initialized = true;
        if (CC.Config.debug) console.log(`[Service] ${this.name} initialized.`);
    }
};

/**
 * Base Component Class
 * For UI components that might have multiple instances or need lifecycle management.
 */
CC.Utils.BaseComponent = class BaseComponent {
    constructor(name, element, options = {}) {
        this.name = name;
        this.element = element;
        this.options = options;
    }

    // Helper to safe-query selectors within the component
    find(selector) {
        return this.element ? this.element.querySelector(selector) : null;
    }

    findAll(selector) {
        return this.element ? this.element.querySelectorAll(selector) : [];
    }

    init() {
        if (this.initialized) return true;
        this.initialized = true;
        return false;
    }
};

// Compatibility Alias
CC.Services.BaseService = CC.Utils.BaseService;

// Event Bus for decoupled communication
CC.Utils.EventBus = new class EventBus {
    constructor() {
        this.listeners = {};
    }

    on(event, callback) {
        if (!this.listeners[event]) this.listeners[event] = [];
        this.listeners[event].push(callback);
    }

    off(event, callback) {
        if (!this.listeners[event]) return;
        this.listeners[event] = this.listeners[event].filter(cb => cb !== callback);
    }

    emit(event, data) {
        if (!this.listeners[event]) return;
        this.listeners[event].forEach(callback => {
            try {
                callback(data);
            } catch (e) {
                console.error(`Error in event listener for ${event}:`, e);
            }
        });
    }
}();

// Alias for easier access
CC.Events = CC.Utils.EventBus;

console.log('Community Car Core initialized');
