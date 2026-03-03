(() => {
    const IDLE_MINUTES = 2;       // ✅ antes 2, ahora 15
    const WARNING_SECONDS = 60;    // modal 60s antes

    const idleMs = IDLE_MINUTES * 60 * 1000;
    const warningMs = idleMs - (WARNING_SECONDS * 1000);

    let warnTimer = null;
    let logoutTimer = null;
    let countdownInterval = null;

    let modalInstance = null;
    let lastPingAt = 0; // para no spamear keepalive
    let warningVisible = false; // ✅ NUEVO: si el modal está visible

    const getCsrf = () => {
        const el = document.querySelector('meta[name="csrf-token"]');
        return el ? el.getAttribute('content') : '';
    };

    const ensureModal = () => {
        const el = document.getElementById('sessionTimeoutModal');
        if (!el) return null;
        if (!modalInstance) modalInstance = new bootstrap.Modal(el, { backdrop: 'static', keyboard: false });
        return modalInstance;
    };

    const stopCountdown = () => {
        if (countdownInterval) clearInterval(countdownInterval);
        countdownInterval = null;
    };

    const hideWarning = () => {
        stopCountdown();
        warningVisible = false;
        const m = ensureModal();
        if (m) m.hide();
    };

    const showWarning = () => {
        const m = ensureModal();
        if (!m) return;

        warningVisible = true;

        let remaining = WARNING_SECONDS;
        const label = document.getElementById('sessionTimeoutCountdown');
        if (label) label.textContent = String(remaining);

        m.show();

        stopCountdown();
        countdownInterval = setInterval(() => {
            remaining -= 1;
            if (label) label.textContent = String(Math.max(0, remaining));
            if (remaining <= 0) stopCountdown();
        }, 1000);
    };

    const logoutNow = async () => {
        try {
            await fetch('/logout', {
                method: 'POST',
                headers: { 'RequestVerificationToken': getCsrf() }
            });
        } catch { }

        window.location.href = '/login?reason=timeout';
    };

    const pingActivity = async () => {
        // solo si hubo interacción; throttle 60s
        const now = Date.now();
        if (now - lastPingAt < 60_000) return;
        lastPingAt = now;

        try {
            await fetch('/Account/KeepAlive', {
                method: 'GET',
                headers: { 'X-User-Activity': '1' },
                cache: 'no-store'
            });
        } catch { }
    };

    // ✅ Arranca/reinicia timers SIN tocar el modal
    const startTimers = () => {
        clearTimeout(warnTimer);
        clearTimeout(logoutTimer);

        warnTimer = setTimeout(showWarning, warningMs);
        logoutTimer = setTimeout(logoutNow, idleMs);
    };

    // ✅ Solo reinicia por actividad si NO está el modal visible
    const onUserActivity = () => {
        if (warningVisible) return; // 👈 aquí está la clave
        startTimers();
        pingActivity();
    };

    const stayConnected = async () => {
        // ✅ SOLO el botón revive sesión
        await pingActivity();
        hideWarning();
        startTimers();
    };

    document.addEventListener('click', (e) => {
        const target = e.target;
        if (target && target.id === 'btnStayConnected') {
            console.log("✅ Click en Mantener sesión");
            e.preventDefault();
            stayConnected();
        }
    });

    ['mousemove', 'mousedown', 'keydown', 'scroll', 'touchstart'].forEach(ev => {
        document.addEventListener(ev, onUserActivity, { passive: true });
    });

    // inicio
    startTimers();
})();