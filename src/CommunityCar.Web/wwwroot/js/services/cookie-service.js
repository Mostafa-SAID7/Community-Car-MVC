/**
 * Cookie Service
 * Handles getting and setting cookies securely
 */
(function (CC) {
    class CookieService extends CC.Utils.BaseService {
        constructor() {
            super('Cookie');
        }

        get(name) {
            const value = `; ${document.cookie}`;
            const parts = value.split(`; ${name}=`);
            if (parts.length === 2) return parts.pop().split(';').shift();
            return null;
        }

        set(name, value, days) {
            const date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            const expires = "; expires=" + date.toUTCString();
            document.cookie = name + "=" + (value || "") + expires + "; path=/; samesite=lax";
        }
    }

    // Initialize Singleton
    CC.Services.Cookie = new CookieService();

})(window.CommunityCar);
