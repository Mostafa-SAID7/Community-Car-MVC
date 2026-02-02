/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: ["class"],
    content: [
        './Views/**/*.cshtml',
        './Pages/**/*.cshtml',
        './wwwroot/js/**/*.js',
        './Areas/**/*.cshtml',
        './Components/**/*.cshtml',
        './TagHelpers/**/*.cs',
        './Models/**/*.cs'
    ],
    theme: {
        screens: {
            'xs': '475px',
            'sm': '640px',
            'md': '768px',
            'lg': '1024px',
            'xl': '1280px',
            '2xl': '1400px',
            // Custom breakpoints matching app.css
            'mobile': '640px',      // Mobile breakpoint from app.css
            'tablet': '768px',      // Tablet breakpoint from app.css  
            'desktop': '1280px',    // Desktop breakpoint from app.css
        },
        container: {
            center: true,
            padding: "2rem",
            screens: {
                "2xl": "1400px",
            },
        },
        extend: {
            colors: {
                border: "hsl(var(--border))",
                input: "hsl(var(--input))",
                ring: "hsl(var(--ring))",
                background: "hsl(var(--background))",
                foreground: "hsl(var(--foreground))",
                primary: {
                    DEFAULT: "hsl(var(--primary))",
                    foreground: "hsl(var(--primary-foreground))",
                },
                secondary: {
                    DEFAULT: "hsl(var(--secondary))",
                    foreground: "hsl(var(--secondary-foreground))",
                },
                destructive: {
                    DEFAULT: "hsl(var(--destructive))",
                    foreground: "hsl(var(--destructive-foreground))",
                },
                muted: {
                    DEFAULT: "hsl(var(--muted))",
                    foreground: "hsl(var(--muted-foreground))",
                },
                accent: {
                    DEFAULT: "hsl(var(--accent))",
                    foreground: "hsl(var(--accent-foreground))",
                },
                popover: {
                    DEFAULT: "hsl(var(--popover))",
                    foreground: "hsl(var(--popover-foreground))",
                },
                card: {
                    DEFAULT: "hsl(var(--card))",
                    foreground: "hsl(var(--card-foreground))",
                },
                // Enhanced Red Color Palette
                red: {
                    50: "#fef2f2",
                    100: "#fee2e2",
                    200: "#fecaca",
                    300: "#fca5a5",
                    400: "#f87171",
                    500: "#ef4444",
                    600: "#dc2626",
                    700: "#b91c1c",
                    800: "#991b1b",
                    900: "#7f1d1d",
                    950: "#450a0a",
                },
                // Semantic Colors
                success: {
                    50: "#f0fdf4",
                    100: "#dcfce7",
                    200: "#bbf7d0",
                    300: "#86efac",
                    400: "#4ade80",
                    500: "#22c55e",
                    600: "#16a34a",
                    700: "#15803d",
                    800: "#166534",
                    900: "#14532d",
                    950: "#052e16",
                },
                warning: {
                    50: "#fffbeb",
                    100: "#fef3c7",
                    200: "#fde68a",
                    300: "#fcd34d",
                    400: "#fbbf24",
                    500: "#f59e0b",
                    600: "#d97706",
                    700: "#b45309",
                    800: "#92400e",
                    900: "#78350f",
                    950: "#451a03",
                },
                info: {
                    50: "#eff6ff",
                    100: "#dbeafe",
                    200: "#bfdbfe",
                    300: "#93c5fd",
                    400: "#60a5fa",
                    500: "#3b82f6",
                    600: "#2563eb",
                    700: "#1d4ed8",
                    800: "#1e40af",
                    900: "#1e3a8a",
                    950: "#172554",
                },
                error: {
                    50: "#fef2f2",
                    100: "#fee2e2",
                    200: "#fecaca",
                    300: "#fca5a5",
                    400: "#f87171",
                    500: "#ef4444",
                    600: "#dc2626",
                    700: "#b91c1c",
                    800: "#991b1b",
                    900: "#7f1d1d",
                    950: "#450a0a",
                },
                // Glass Effect Colors
                glass: {
                    white: "rgba(255, 255, 255, 0.1)",
                    black: "rgba(0, 0, 0, 0.1)",
                    red: "rgba(239, 68, 68, 0.1)",
                },
            },
            borderRadius: {
                lg: "var(--radius)",
                md: "calc(var(--radius) - 2px)",
                sm: "calc(var(--radius) - 4px)",
                xl: "1rem",
                "2xl": "1.5rem",
                "3xl": "2rem",
            },
            fontFamily: {
                sans: ["Alexandria", "ui-sans-serif", "system-ui"],
                'arabic-heading': ["Lemonada", "Alexandria", "cursive"],
                'arabic-body': ["Noto Sans Arabic", "Alexandria", "sans-serif"],
                'inter': ["Inter", "Alexandria", "system-ui", "-apple-system", "sans-serif"],
                mono: ["JetBrains Mono", "Fira Code", "monospace"],
            },
            fontSize: {
                '2xs': ['0.625rem', { lineHeight: '0.75rem' }],
                '3xl': ['1.875rem', { lineHeight: '2.25rem' }],
                '4xl': ['2.25rem', { lineHeight: '2.5rem' }],
                '5xl': ['3rem', { lineHeight: '1' }],
                '6xl': ['3.75rem', { lineHeight: '1' }],
                '7xl': ['4.5rem', { lineHeight: '1' }],
                '8xl': ['6rem', { lineHeight: '1' }],
                '9xl': ['8rem', { lineHeight: '1' }],
            },
            spacing: {
                '18': '4.5rem',
                '88': '22rem',
                '128': '32rem',
                '144': '36rem',
                // Layout spacing utilities from app.css
                'sidebar-left': '16rem',        // Left sidebar width (w-64) - 256px
                'sidebar-right': '22rem',       // Right sidebar width when both sidebars present - 352px
                'sidebar-mobile': '14rem',      // Mobile sidebar width (w-56) - 224px
                'sidebar-tablet': '6rem',       // Tablet sidebar width (w-24) - 96px
                // Main content spacing utilities
                'main-mobile': '1rem',          // Mobile main content padding - 16px
                'main-tablet': '6rem',          // Tablet main content left padding - 96px
                'main-desktop': '16rem',        // Desktop main content left padding - 256px
                'main-desktop-both': '22rem',   // Desktop main content right padding when both sidebars - 352px
                // Responsive breakpoint spacing
                'content-mobile': '1rem',       // Mobile content padding
                'content-tablet': '1.5rem',     // Tablet content padding
                'content-desktop': '2rem',      // Desktop content padding
                // Sidebar specific measurements
                'sidebar-collapsed': '4rem',    // Collapsed sidebar width - 64px
                'sidebar-expanded': '16rem',    // Expanded sidebar width - 256px
                'sidebar-right-expanded': '22rem', // Right sidebar expanded width - 352px
                // Container spacing
                'container-mobile': '1rem',     // Mobile container padding
                'container-tablet': '2rem',     // Tablet container padding
                'container-desktop': '3rem',    // Desktop container padding
            },
            backdropBlur: {
                xs: '2px',
                '3xl': '64px',
            },
            boxShadow: {
                'glass': '0 8px 32px 0 rgba(31, 38, 135, 0.37)',
                'glass-inset': 'inset 0 1px 0 0 rgba(255, 255, 255, 0.05)',
                'red-glow': '0 0 20px rgba(239, 68, 68, 0.3)',
                'red-glow-lg': '0 0 40px rgba(239, 68, 68, 0.4)',
                'inner-border': 'inset 0 1px 0 0 rgba(255, 255, 255, 0.1)',
                'soft': '0 2px 15px -3px rgba(0, 0, 0, 0.07), 0 10px 20px -2px rgba(0, 0, 0, 0.04)',
                'medium': '0 4px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
                'hard': '0 10px 40px -10px rgba(0, 0, 0, 0.2), 0 2px 10px -2px rgba(0, 0, 0, 0.04)',
            },
            keyframes: {
                "accordion-down": {
                    from: { height: "0" },
                    to: { height: "var(--radix-accordion-content-height)" },
                },
                "accordion-up": {
                    from: { height: "var(--radix-accordion-content-height)" },
                    to: { height: "0" },
                },
                "fade-in": {
                    "0%": { opacity: "0" },
                    "100%": { opacity: "1" },
                },
                "fade-out": {
                    "0%": { opacity: "1" },
                    "100%": { opacity: "0" },
                },
                "slide-in-from-top": {
                    "0%": { transform: "translateY(-100%)" },
                    "100%": { transform: "translateY(0)" },
                },
                "slide-in-from-bottom": {
                    "0%": { transform: "translateY(100%)" },
                    "100%": { transform: "translateY(0)" },
                },
                "slide-in-from-left": {
                    "0%": { transform: "translateX(-100%)" },
                    "100%": { transform: "translateX(0)" },
                },
                "slide-in-from-right": {
                    "0%": { transform: "translateX(100%)" },
                    "100%": { transform: "translateX(0)" },
                },
                "scale-in": {
                    "0%": { transform: "scale(0.95)", opacity: "0" },
                    "100%": { transform: "scale(1)", opacity: "1" },
                },
                "scale-out": {
                    "0%": { transform: "scale(1)", opacity: "1" },
                    "100%": { transform: "scale(0.95)", opacity: "0" },
                },
                "bounce-in": {
                    "0%": { transform: "scale(0.3)", opacity: "0" },
                    "50%": { transform: "scale(1.05)" },
                    "70%": { transform: "scale(0.9)" },
                    "100%": { transform: "scale(1)", opacity: "1" },
                },
                "pulse-red": {
                    "0%, 100%": { boxShadow: "0 0 0 0 rgba(239, 68, 68, 0.7)" },
                    "70%": { boxShadow: "0 0 0 10px rgba(239, 68, 68, 0)" },
                },
                "shimmer": {
                    "0%": { backgroundPosition: "-200% 0" },
                    "100%": { backgroundPosition: "200% 0" },
                },
                "float": {
                    "0%, 100%": { transform: "translateY(0px)" },
                    "50%": { transform: "translateY(-10px)" },
                },
                "wiggle": {
                    "0%, 100%": { transform: "rotate(-3deg)" },
                    "50%": { transform: "rotate(3deg)" },
                },
                "spin-slow": {
                    "0%": { transform: "rotate(0deg)" },
                    "100%": { transform: "rotate(360deg)" },
                },
            },
            animation: {
                "accordion-down": "accordion-down 0.2s ease-out",
                "accordion-up": "accordion-up 0.2s ease-out",
                "fade-in": "fade-in 0.5s ease-out",
                "fade-out": "fade-out 0.5s ease-out",
                "slide-in-from-top": "slide-in-from-top 0.3s ease-out",
                "slide-in-from-bottom": "slide-in-from-bottom 0.3s ease-out",
                "slide-in-from-left": "slide-in-from-left 0.3s ease-out",
                "slide-in-from-right": "slide-in-from-right 0.3s ease-out",
                "scale-in": "scale-in 0.2s ease-out",
                "scale-out": "scale-out 0.2s ease-out",
                "bounce-in": "bounce-in 0.6s ease-out",
                "pulse-red": "pulse-red 2s infinite",
                "shimmer": "shimmer 2s linear infinite",
                "float": "float 3s ease-in-out infinite",
                "wiggle": "wiggle 1s ease-in-out infinite",
                "spin-slow": "spin-slow 3s linear infinite",
            },
            backgroundImage: {
                'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
                'gradient-conic': 'conic-gradient(from 180deg at 50% 50%, var(--tw-gradient-stops))',
                'shimmer': 'linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent)',
            },
            transitionTimingFunction: {
                'bounce-in': 'cubic-bezier(0.68, -0.55, 0.265, 1.55)',
                'smooth': 'cubic-bezier(0.4, 0, 0.2, 1)',
            },
            zIndex: {
                '60': '60',
                '70': '70',
                '80': '80',
                '90': '90',
                '100': '100',
            },
        },
    },
    plugins: [
        require("tailwindcss-animate"),
        function({ addComponents, addUtilities, theme }) {
            // Custom Button Components
            addComponents({
                '.btn': {
                    '@apply inline-flex items-center justify-center rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:opacity-50 disabled:pointer-events-none ring-offset-background': {},
                },
                '.btn-primary': {
                    '@apply bg-red-500 text-white hover:bg-red-600 active:bg-red-700': {},
                },
                '.btn-secondary': {
                    '@apply bg-secondary text-secondary-foreground hover:bg-secondary/80': {},
                },
                '.btn-outline': {
                    '@apply border border-input hover:bg-accent hover:text-accent-foreground': {},
                },
                '.btn-ghost': {
                    '@apply hover:bg-accent hover:text-accent-foreground': {},
                },
                '.btn-destructive': {
                    '@apply bg-destructive text-destructive-foreground hover:bg-destructive/90': {},
                },
                '.btn-sm': {
                    '@apply h-9 px-3 rounded-md': {},
                },
                '.btn-md': {
                    '@apply h-10 py-2 px-4': {},
                },
                '.btn-lg': {
                    '@apply h-11 px-8 rounded-md': {},
                },
                '.btn-icon': {
                    '@apply h-10 w-10': {},
                },
            });

            // Custom Card Components
            addComponents({
                '.card': {
                    '@apply rounded-lg border bg-card text-card-foreground shadow-sm': {},
                },
                '.card-header': {
                    '@apply flex flex-col space-y-1.5 p-6': {},
                },
                '.card-title': {
                    '@apply text-2xl font-semibold leading-none tracking-tight': {},
                },
                '.card-description': {
                    '@apply text-sm text-muted-foreground': {},
                },
                '.card-content': {
                    '@apply p-6 pt-0': {},
                },
                '.card-footer': {
                    '@apply flex items-center p-6 pt-0': {},
                },
            });

            // Layout Components (matching app.css patterns)
            addComponents({
                '.layout-container': {
                    '@apply min-h-screen bg-background': {},
                },
                '.sidebar-left': {
                    '@apply fixed left-0 top-0 h-full bg-card border-r border-border z-40': {},
                    '@apply hidden md:block': {},
                    '@apply w-sidebar-tablet md:w-sidebar-tablet xl:w-sidebar-left': {},
                },
                '.sidebar-right': {
                    '@apply fixed right-0 top-0 h-full bg-card border-l border-border z-40': {},
                    '@apply hidden xl:block': {},
                    '@apply w-sidebar-right': {},
                },
                '.main-content': {
                    '@apply min-h-screen transition-all duration-300': {},
                    '@apply p-main-mobile': {},
                    '@apply md:pl-main-tablet': {},
                    '@apply xl:pl-main-desktop xl:pr-main-desktop-both': {},
                },
                '.content-wrapper': {
                    '@apply max-w-7xl mx-auto': {},
                    '@apply px-container-mobile': {},
                    '@apply md:px-container-tablet': {},
                    '@apply xl:px-container-desktop': {},
                },
                '.sidebar-mobile-overlay': {
                    '@apply fixed inset-0 bg-black/50 z-30 md:hidden': {},
                },
                '.sidebar-mobile': {
                    '@apply fixed left-0 top-0 h-full w-sidebar-mobile bg-card border-r border-border z-40': {},
                    '@apply transform transition-transform duration-300': {},
                    '@apply -translate-x-full': {},
                    '&.open': {
                        '@apply translate-x-0': {},
                    },
                },
                // RTL Layout Components
                '.rtl-sidebar-left': {
                    '[dir="rtl"] &': {
                        '@apply right-0 left-auto border-l border-r-0': {},
                    },
                },
                '.rtl-sidebar-right': {
                    '[dir="rtl"] &': {
                        '@apply left-0 right-auto border-r border-l-0': {},
                    },
                },
                '.rtl-main-content': {
                    '[dir="rtl"] &': {
                        '@apply md:pr-main-tablet md:pl-0': {},
                        '@apply xl:pr-main-desktop xl:pl-main-desktop-both': {},
                    },
                },
                '.rtl-sidebar-mobile': {
                    '[dir="rtl"] &': {
                        '@apply right-0 left-auto border-l border-r-0': {},
                        '@apply translate-x-full': {},
                        '&.open': {
                            '@apply translate-x-0': {},
                        },
                    },
                },
            });

            // Glass Effect Components
            addComponents({
                '.glass': {
                    '@apply backdrop-blur-md bg-white/10 border border-white/20': {},
                },
                '.glass-dark': {
                    '@apply backdrop-blur-md bg-black/10 border border-white/10': {},
                },
                '.glass-red': {
                    '@apply backdrop-blur-md bg-red-500/10 border border-red-500/20': {},
                },
                '.glass-card': {
                    '@apply glass rounded-xl shadow-glass': {},
                },
                '.glass-button': {
                    '@apply glass rounded-lg px-4 py-2 hover:bg-white/20 transition-all duration-200': {},
                },
            });

            // Form Components
            addComponents({
                '.input': {
                    '@apply flex h-10 w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50': {},
                },
                '.textarea': {
                    '@apply flex min-h-[80px] w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50': {},
                },
                '.select': {
                    '@apply flex h-10 w-full items-center justify-between rounded-md border border-input bg-transparent px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50': {},
                },
                '.label': {
                    '@apply text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70': {},
                },
            });

            // Custom Utilities
            addUtilities({
                '.scrollbar-hide': {
                    '-ms-overflow-style': 'none',
                    'scrollbar-width': 'none',
                    '&::-webkit-scrollbar': {
                        display: 'none',
                    },
                },
                '.scrollbar-thin': {
                    'scrollbar-width': 'thin',
                    '&::-webkit-scrollbar': {
                        width: '6px',
                        height: '6px',
                    },
                    '&::-webkit-scrollbar-track': {
                        background: 'transparent',
                    },
                    '&::-webkit-scrollbar-thumb': {
                        background: theme('colors.gray.300'),
                        borderRadius: '3px',
                    },
                    '&::-webkit-scrollbar-thumb:hover': {
                        background: theme('colors.gray.400'),
                    },
                },
                '.text-balance': {
                    'text-wrap': 'balance',
                },
                '.text-pretty': {
                    'text-wrap': 'pretty',
                },
                '.bg-grid': {
                    'background-image': `url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 32 32' width='32' height='32' fill='none' stroke='rgb(15 23 42 / 0.04)'%3e%3cpath d='m0 .5h32m-32 32v-32'/%3e%3c/svg%3e")`,
                },
                '.bg-grid-small': {
                    'background-image': `url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' width='16' height='16' fill='none' stroke='rgb(15 23 42 / 0.04)'%3e%3cpath d='m0 .5h16m-16 16v-16'/%3e%3c/svg%3e")`,
                },
                '.bg-dot': {
                    'background-image': `radial-gradient(rgb(15 23 42 / 0.04) 1px, transparent 1px)`,
                    'background-size': '16px 16px',
                },
                // Sidebar and Main Content Layout Utilities (matching app.css patterns)
                '.sidebar-spacing': {
                    // Mobile: no sidebar spacing
                    'padding-left': '0',
                    'padding-right': '0',
                    // Tablet: left sidebar spacing
                    '@media (min-width: 768px)': {
                        'padding-left': theme('spacing.main-tablet'),
                    },
                    // Desktop: both sidebars spacing
                    '@media (min-width: 1280px)': {
                        'padding-left': theme('spacing.main-desktop'),
                        'padding-right': theme('spacing.main-desktop-both'),
                    },
                },
                '.main-content-spacing': {
                    // Mobile: basic padding
                    'padding': theme('spacing.main-mobile'),
                    // Tablet: left sidebar compensation
                    '@media (min-width: 768px)': {
                        'padding-left': theme('spacing.main-tablet'),
                        'padding-right': theme('spacing.main-mobile'),
                    },
                    // Desktop: both sidebars compensation
                    '@media (min-width: 1280px)': {
                        'padding-left': theme('spacing.main-desktop'),
                        'padding-right': theme('spacing.main-desktop-both'),
                    },
                },
                '.sidebar-left-spacing': {
                    // Mobile: hidden
                    'margin-left': '0',
                    // Tablet: show with tablet width
                    '@media (min-width: 768px)': {
                        'width': theme('spacing.sidebar-tablet'),
                    },
                    // Desktop: show with full width
                    '@media (min-width: 1280px)': {
                        'width': theme('spacing.sidebar-left'),
                    },
                },
                '.sidebar-right-spacing': {
                    // Mobile & Tablet: hidden
                    'margin-right': '0',
                    'width': '0',
                    // Desktop: show with right sidebar width
                    '@media (min-width: 1280px)': {
                        'width': theme('spacing.sidebar-right'),
                    },
                },
                '.content-container-spacing': {
                    // Mobile: basic container padding
                    'padding-left': theme('spacing.container-mobile'),
                    'padding-right': theme('spacing.container-mobile'),
                    // Tablet: increased padding
                    '@media (min-width: 768px)': {
                        'padding-left': theme('spacing.container-tablet'),
                        'padding-right': theme('spacing.container-tablet'),
                    },
                    // Desktop: maximum padding
                    '@media (min-width: 1280px)': {
                        'padding-left': theme('spacing.container-desktop'),
                        'padding-right': theme('spacing.container-desktop'),
                    },
                },
                // RTL Support utilities
                '.rtl-sidebar-spacing': {
                    '[dir="rtl"] &': {
                        // Mobile: no sidebar spacing
                        'padding-right': '0',
                        'padding-left': '0',
                        // Tablet: right sidebar spacing for RTL
                        '@media (min-width: 768px)': {
                            'padding-right': theme('spacing.main-tablet'),
                            'padding-left': '0',
                        },
                        // Desktop: both sidebars spacing for RTL
                        '@media (min-width: 1280px)': {
                            'padding-right': theme('spacing.main-desktop'),
                            'padding-left': theme('spacing.main-desktop-both'),
                        },
                    },
                },
                '.rtl-main-content-spacing': {
                    '[dir="rtl"] &': {
                        // Mobile: basic padding
                        'padding': theme('spacing.main-mobile'),
                        // Tablet: right sidebar compensation for RTL
                        '@media (min-width: 768px)': {
                            'padding-right': theme('spacing.main-tablet'),
                            'padding-left': theme('spacing.main-mobile'),
                        },
                        // Desktop: both sidebars compensation for RTL
                        '@media (min-width: 1280px)': {
                            'padding-right': theme('spacing.main-desktop'),
                            'padding-left': theme('spacing.main-desktop-both'),
                        },
                    },
                },
            });
        },
    ],
}