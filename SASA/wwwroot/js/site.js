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
    .addEventListener("click", function () {
        document.getElementById("sidebar")
            .classList.toggle("open"); //Toggle adds open if not present, removes it if present
    });
//I am not smart enough to do this better so for now, I'll do it the brainless way
document.getElementById("sidebarClose")
    .addEventListener("click", function () {
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


//---------------------------TICKETS----------------------------------------------
//PARA FILTROS
document.addEventListener("DOMContentLoaded", function () {

    const switchInput = document.getElementById("dateRangeSwitch");

    const singleContainer = document.getElementById("singleDateContainer");
    const rangeContainer = document.getElementById("rangeDateContainer");

    const singleDate = document.getElementById("filter-date-single");

    const dateFrom = document.getElementById("filter-date-from");
    const dateTo = document.getElementById("filter-date-to");

    function updateMode() {

        if (switchInput.checked) {

            singleDate.classList.add("d-none");
            singleDate.disabled = true;

            dateFrom.classList.remove("d-none");
            dateTo.classList.remove("d-none");

            dateFrom.disabled = false;
            dateTo.disabled = false;

        } else {

            singleDate.classList.remove("d-none");
            singleDate.disabled = false;

            dateFrom.classList.add("d-none");
            dateTo.classList.add("d-none");

            dateFrom.disabled = true;
            dateTo.disabled = true;
        }
    }

    switchInput.addEventListener("change", updateMode);

    updateMode();
});

