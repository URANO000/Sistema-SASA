document.addEventListener("DOMContentLoaded", () => {

    const updateBadge = () => {
        const unread = document.querySelectorAll(".notif-item.notif-unread").length;
        const badge = document.querySelector(".notification-badge");
        if (!badge) return;

        if (unread <= 0) {
            badge.style.display = "none";
            return;
        }

        badge.style.display = "inline-block";
        badge.textContent = unread > 99 ? "99+" : unread.toString();
    };

    const setItemReadState = (item, unread) => {
        const dot = item.querySelector(".unread-dot");
        const btnIcon = item.querySelector(".toggle-read i");

        if (unread) {
            item.classList.add("notif-unread");
            if (dot) dot.classList.remove("d-none");
            if (btnIcon) btnIcon.className = "bi bi-envelope-open";
        } else {
            item.classList.remove("notif-unread");
            if (dot) dot.classList.add("d-none");
            if (btnIcon) btnIcon.className = "bi bi-envelope";
        }
        updateBadge();
    };

    document.querySelectorAll(".notif-item .toggle-read").forEach(btn => {
        btn.addEventListener("click", (e) => {
            const item = e.currentTarget.closest(".notif-item");
            const isUnread = item.classList.contains("notif-unread");
            setItemReadState(item, !isUnread);
        });
    });

    const btnMarkAll = document.getElementById("btnMarkAll");
    if (btnMarkAll) {
        btnMarkAll.addEventListener("click", () => {
            document.querySelectorAll(".notif-item").forEach(item => setItemReadState(item, false));
        });
    }

    document.querySelectorAll(".notif-item").forEach(item => {
        const silenced = item.getAttribute("data-silenced") === "true";
        if (silenced) item.classList.add("notif-silenced");
    });

    updateBadge();
});
(() => {
    const getKey = (ticketId) => `sasa_silence_ticket_${ticketId}`;

    const formatUntil = (date) => {
        const d = new Date(date);
        return d.toLocaleString();
    };

    const applySilenceUI = (ticketId) => {
        const card = document.getElementById("silenceCard");
        if (!card) return;

        const status = document.getElementById("silenceStatus");
        const untilEl = document.getElementById("silenceUntil");
        const badge = document.getElementById("silenceBadge");
        const btnUnsilence = document.getElementById("btnUnsilence");

        const raw = localStorage.getItem(getKey(ticketId));
        if (!raw) {
            status.textContent = "Activas";
            untilEl.textContent = "Sin silencio configurado.";
            badge.classList.remove("silenced");
            badge.classList.add("active");
            badge.innerHTML = `<i class="bi bi-bell"></i>`;
            btnUnsilence.classList.add("d-none");
            return;
        }

        const until = new Date(raw);
        const now = new Date();

        if (until <= now) {
            localStorage.removeItem(getKey(ticketId));
            applySilenceUI(ticketId);
            return;
        }

        status.textContent = "Silenciadas";
        untilEl.textContent = `Silenciado hasta: ${formatUntil(until)}`;
        badge.classList.add("silenced");
        badge.innerHTML = `<i class="bi bi-bell-slash"></i>`;
        btnUnsilence.classList.remove("d-none");
    };

    const silenceCard = document.getElementById("silenceCard");
    if (silenceCard) {
        const ticketId = silenceCard.getAttribute("data-ticket-id");
        applySilenceUI(ticketId);

        document.querySelectorAll(".silence-option").forEach(btn => {
            btn.addEventListener("click", () => {
                const hours = parseInt(btn.getAttribute("data-hours"), 10);
                const tid = btn.getAttribute("data-ticket");

                const until = new Date();
                until.setHours(until.getHours() + hours);

                localStorage.setItem(getKey(tid), until.toISOString());
                applySilenceUI(tid);

                const modalEl = document.getElementById("silenceModal");
                const modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) modal.hide();
            });
        });

        const btnUnsilence = document.getElementById("btnUnsilence");
        if (btnUnsilence) {
            btnUnsilence.addEventListener("click", () => {
                localStorage.removeItem(getKey(ticketId));
                applySilenceUI(ticketId);
            });
        }
    }

    const notifItems = document.querySelectorAll(".notif-item[data-related]");
    if (notifItems.length > 0) {
        notifItems.forEach(item => {
            const related = item.getAttribute("data-related") || "";
            const match = related.match(/#(\d+)/);
            if (!match) return;

            const tid = match[1];
            const raw = localStorage.getItem(getKey(tid));
            if (!raw) return;

            const until = new Date(raw);
            if (until > new Date()) {
                item.classList.add("notif-silenced");
            }
        });
    }
})();

