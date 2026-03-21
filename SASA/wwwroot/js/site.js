//Contador de notificacione
async function actualizarIndicadorNotificaciones() {
    try {
        const res = await fetch('/Notificaciones/Contador', { credentials: 'same-origin' });

        if (res.redirected) return;

        const total = await res.json();
        const badge = document.getElementById('notifBadge');
        if (!badge) return;

        if (total > 0) {
            badge.classList.remove('d-none');
            badge.textContent = total > 99 ? '99+' : total;
        } else {
            badge.classList.add('d-none');
        }
    } catch (e) {
        console.error('Error cargando contador', e);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    actualizarIndicadorNotificaciones();
    setInterval(actualizarIndicadorNotificaciones, 15000);
});



//This is for my sidebar toggle

document.getElementById("sidebarToggle")
    .addEventListener("click", function (e) {
        e.stopPropagation();
        document.getElementById("sidebar")
            .classList.toggle("open"); //Toggle adds open if not present, removes it if present
    });

document.getElementById("sidebarClose")
    .addEventListener("click", function (e) {
        e.stopPropagation();
        document.getElementById("sidebar")
            .classList.toggle("open");
    });

document.querySelectorAll(".dropdown-toggle-btn")
    .forEach(button => {
        button.addEventListener("click", () => {
            button.closest(".nav-section")
                .classList.toggle("open");
        });
    });

document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const body = document.body;

    const closeSidebar = (e) => {
        if (!sidebar.contains(e.target) && sidebar.classList.contains("open")) {
            sidebar.classList.remove("open");
        }
    };

    body.addEventListener('touchend', closeSidebar, { passive: true });
    body.addEventListener('click', closeSidebar);

});

//---------------------------TICKETS----------------------------------------------
//PARA FILTROS
document.addEventListener("DOMContentLoaded", function () {

    const switchInput = document.getElementById("dateRangeSwitch");

    const singleWrapper = document.getElementById("single-wrapper");
    const rangeWrapper = document.getElementById("range-wrapper");

    const singleDate = document.getElementById("filter-date-single");
    const dateFrom = document.getElementById("filter-date-from");
    const dateTo = document.getElementById("filter-date-to");

    const mainLabel = document.getElementById("main-label");

    function updateMode() {

        if (switchInput.checked) {

            // UI
            singleWrapper.classList.add("d-none");
            rangeWrapper.classList.remove("d-none");

            // Enable/disable
            singleDate.disabled = true;
            dateFrom.disabled = false;
            dateTo.disabled = false;

            mainLabel.textContent = "Rango de Fechas";

        } else {

            singleWrapper.classList.remove("d-none");
            rangeWrapper.classList.add("d-none");

            singleDate.disabled = false;
            dateFrom.disabled = true;
            dateTo.disabled = true;

            mainLabel.textContent = "Fecha";
        }
    }

    switchInput.addEventListener("change", updateMode);

    updateMode();
});


//TABS--------------------------------------------------------------
document.addEventListener("DOMContentLoaded", function () {
    const activeTab = "@Model.TabActiva";

    if (activeTab === "subcategorias") {
        const trigger = document.getElementById("subcategorias-tab");
        if (trigger) {
            const tab = new bootstrap.Tab(trigger);
            tab.show();
        }
    }
});


document.addEventListener("DOMContentLoaded", function () {
    const activeTab = "@Model.TabActiva";

    if (activeTab === "Cola Global") {
        const trigger = document.getElementById("global-tab");
        if (trigger) {
            const tab = new bootstrap.Tab(trigger);
            tab.show();
        }
    }
});