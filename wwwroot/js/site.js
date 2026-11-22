// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
    const themeKey = 'multiservicio-theme';
    const root = document.documentElement;
    const themeToggle = document.querySelector('[data-theme-toggle]');

    document.querySelectorAll('.table-responsive table').forEach((table) => {
        const headers = [...(table.tHead?.rows[0]?.cells ?? [])]
            .map((cell) => cell.textContent.trim().replace(/\s+/g, ' '));

        [...(table.tBodies ?? [])].forEach((tbody) => {
            [...tbody.rows].forEach((row) => {
                [...row.cells].forEach((cell, index) => {
                    if (cell.tagName === 'TD' && !cell.hasAttribute('data-label')) {
                        cell.dataset.label = headers[index] ?? '';
                    }
                });
            });
        });
    });

    const applyTheme = (theme) => {
        root.dataset.theme = theme;
        root.style.colorScheme = theme;
        if (themeToggle) {
            const isDark = theme === 'dark';
            themeToggle.setAttribute('aria-label', isDark ? 'Cambiar a modo claro' : 'Cambiar a modo oscuro');
            themeToggle.querySelector('i')?.classList.toggle('bi-sun-fill', isDark);
            themeToggle.querySelector('i')?.classList.toggle('bi-moon-stars-fill', !isDark);
        }
    };

    let savedTheme;
    try {
        savedTheme = localStorage.getItem(themeKey);
    } catch {
        savedTheme = null;
    }
    applyTheme(savedTheme === 'dark' || savedTheme === 'light'
        ? savedTheme
        : (window.matchMedia?.('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'));

    themeToggle?.addEventListener('click', () => {
        const theme = root.dataset.theme === 'dark' ? 'light' : 'dark';
        applyTheme(theme);
        try {
            localStorage.setItem(themeKey, theme);
        } catch {
            // El cambio de tema sigue activo mientras la página esté abierta.
        }
    });

    document.querySelectorAll('[data-carousel]').forEach((carousel) => {
        const slides = [...carousel.querySelectorAll('.carousel-slide')];
        const indicators = [...carousel.querySelectorAll('[data-carousel-to]')];
        if (slides.length < 2) return;

        let activeIndex = slides.findIndex((slide) => slide.classList.contains('is-active'));
        let timer;
        const showSlide = (index) => {
            activeIndex = (index + slides.length) % slides.length;
            slides.forEach((slide, slideIndex) => {
                const isActive = slideIndex === activeIndex;
                slide.classList.toggle('is-active', isActive);
                slide.setAttribute('aria-hidden', String(!isActive));
            });
            indicators.forEach((indicator, indicatorIndex) => {
                const isActive = indicatorIndex === activeIndex;
                indicator.classList.toggle('is-active', isActive);
                indicator.setAttribute('aria-current', String(isActive));
            });
        };
        const stop = () => window.clearInterval(timer);
        const start = () => {
            stop();
            if (!window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
                timer = window.setInterval(() => showSlide(activeIndex + 1), Number(carousel.dataset.interval) || 5500);
            }
        };

        carousel.querySelector('[data-carousel-prev]')?.addEventListener('click', () => { showSlide(activeIndex - 1); start(); });
        carousel.querySelector('[data-carousel-next]')?.addEventListener('click', () => { showSlide(activeIndex + 1); start(); });
        indicators.forEach((indicator) => indicator.addEventListener('click', () => {
            showSlide(Number(indicator.dataset.carouselTo));
            start();
        }));
        carousel.addEventListener('mouseenter', stop);
        carousel.addEventListener('mouseleave', start);
        carousel.addEventListener('focusin', stop);
        carousel.addEventListener('focusout', (event) => {
            if (!carousel.contains(event.relatedTarget)) start();
        });
        start();
    });
})();
