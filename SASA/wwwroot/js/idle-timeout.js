(() => {
    const IDLE_MINUTES = 15;
    const WARNING_SECONDS = 60;

    const idleMs = IDLE_MINUTES * 60 * 1000;
    const warningMs = idleMs - (WARNING_SECONDS * 1000);
    const activityPingThrottleMs = 60 * 1000;

    let warningTimer = null;
    let logoutTimer = null;
    let countdownInterval = null;
    let modalInstance = null;
    let warningVisible = false;
    let lastKeepAliveAt = 0;

    const getCsrfToken = () => {
        const tokenElement = document.querySelector('meta[name="csrf-token"]');
        return tokenElement ? tokenElement.getAttribute('content') : '';
    };

    const getModalInstance = () => {
        const modalElement = document.getElementById('sessionTimeoutModal');
        if (!modalElement) {
            return null;
        }

        if (!modalInstance) {
            modalInstance = new bootstrap.Modal(modalElement, {
                backdrop: 'static',
                keyboard: false
            });
        }

        return modalInstance;
    };

    const stopCountdown = () => {
        if (countdownInterval) {
            clearInterval(countdownInterval);
            countdownInterval = null;
        }
    };

    const hideWarning = () => {
        stopCountdown();
        warningVisible = false;

        const modal = getModalInstance();
        if (modal) {
            modal.hide();
        }
    };

    const showWarning = () => {
        const modal = getModalInstance();
        if (!modal) {
            return;
        }

        warningVisible = true;

        let remainingSeconds = WARNING_SECONDS;
        const countdownElement = document.getElementById('sessionTimeoutCountdown');

        if (countdownElement) {
            countdownElement.textContent = String(remainingSeconds);
        }

        modal.show();

        stopCountdown();
        countdownInterval = setInterval(() => {
            remainingSeconds -= 1;

            if (countdownElement) {
                countdownElement.textContent = String(Math.max(0, remainingSeconds));
            }

            if (remainingSeconds <= 0) {
                stopCountdown();
            }
        }, 1000);
    };

    const postKeepAlive = async () => {
        const now = Date.now();

        if (now - lastKeepAliveAt < activityPingThrottleMs) {
            return;
        }

        try {
            const response = await fetch('/Account/KeepAlive', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': getCsrfToken()
                },
                cache: 'no-store'
            });

            if (response.ok) {
                lastKeepAliveAt = now;
            }
        } catch {
        }
    };

    const logoutNow = async () => {
        try {
            await fetch('/logout', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': getCsrfToken()
                },
                cache: 'no-store'
            });
        } catch {
        }

        window.location.href = '/login?reason=timeout';
    };

    const resetTimers = () => {
        clearTimeout(warningTimer);
        clearTimeout(logoutTimer);

        warningTimer = setTimeout(showWarning, warningMs);
        logoutTimer = setTimeout(logoutNow, idleMs);
    };

    const onUserActivity = () => {
        if (warningVisible) {
            return;
        }

        resetTimers();
        postKeepAlive();
    };

    const stayConnected = async () => {
        lastKeepAliveAt = 0;
        await postKeepAlive();
        hideWarning();
        resetTimers();
    };

    document.addEventListener('click', (event) => {
        const target = event.target;

        if (target && target.id === 'btnStayConnected') {
            event.preventDefault();
            stayConnected();
        }
    });

    ['mousemove', 'mousedown', 'keydown', 'scroll', 'touchstart'].forEach((eventName) => {
        document.addEventListener(eventName, onUserActivity, { passive: true });
    });

    resetTimers();
})();