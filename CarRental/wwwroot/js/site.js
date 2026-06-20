document.addEventListener('DOMContentLoaded', function () {
    // Navbar scroll effect
    const nav = document.querySelector('.landing-nav');
    if (nav) {
        window.addEventListener('scroll', function () {
            nav.classList.toggle('scrolled', window.scrollY > 50);
        });
    }

    // Scroll reveal
    const observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry, i) {
            if (entry.isIntersecting) {
                const el = entry.target;
                const delay = el.dataset.delay || 0;
                setTimeout(function () {
                    el.classList.add('visible');
                }, delay);
                observer.unobserve(el);
            }
        });
    }, { threshold: 0.15 });

    document.querySelectorAll('.reveal, .car-card, .step-card, .location-card, .brand-card').forEach(function (el, i) {
        el.dataset.delay = i * 100;
        observer.observe(el);
    });

    // Counter animation
    const counterObserver = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                animateCounters(entry.target);
                counterObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.5 });

    const statsSection = document.querySelector('.hero-stats');
    if (statsSection) {
        counterObserver.observe(statsSection);
    }

    function animateCounters(container) {
        container.querySelectorAll('.hero-stat-number').forEach(function (el) {
            var target = parseInt(el.dataset.count);
            var suffix = el.dataset.suffix || '';
            var duration = 2000;
            var start = 0;
            var startTime = null;

            function step(timestamp) {
                if (!startTime) startTime = timestamp;
                var progress = Math.min((timestamp - startTime) / duration, 1);
                var eased = 1 - Math.pow(1 - progress, 4);
                var current = Math.floor(eased * target);
                el.textContent = current + suffix;
                if (progress < 1) {
                    requestAnimationFrame(step);
                }
            }

            requestAnimationFrame(step);
        });
    }

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(function (link) {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            var target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });

    // Parallax on hero glows
    window.addEventListener('scroll', function () {
        var scrolled = window.scrollY;
        document.querySelectorAll('.hero-glow').forEach(function (glow, i) {
            var speed = 0.15 + (i * 0.05);
            glow.style.transform = 'translateY(' + (scrolled * speed) + 'px)';
        });
    });
});
