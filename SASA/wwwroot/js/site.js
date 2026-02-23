// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
console.log(":)");

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

            singleContainer.classList.add("d-none");
            rangeContainer.classList.remove("d-none");

            singleDate.disabled = true;

            dateFrom.disabled = false;
            dateTo.disabled = false;

        } else {

            singleContainer.classList.remove("d-none");
            rangeContainer.classList.add("d-none");

            singleDate.disabled = false;

            dateFrom.disabled = true;
            dateTo.disabled = true;
        }
    }

    switchInput.addEventListener("change", updateMode);

    updateMode();
});
//---------------------------USERS----------------------------------------------

//-------------------QUEUE-------------------
document.getElementById('fakeCreateQueue')?.addEventListener('click', function () {

    //It immediately closes add ticket modal window
    const addModal = bootstrap.Modal.getInstance(
        document.getElementById('addQueueModal')
    );
    addModal?.hide();

    //Shows success modal
    const successModal = new bootstrap.Modal(
        document.getElementById('successModal')
    );
    successModal.show();

    //User can close if they want
});

document.getElementById('fakeEditQueue')?.addEventListener('click', function () {
    //Shows success modal

    const successModal = new bootstrap.Modal(
        document.getElementById('successModal')
    );
    successModal.show();

    //I need to add nav back to list
});


//------------------FORMS----------------------------
document.getElementById('fakeEditForm')?.addEventListener('click', function () {
    //Shows success modal

    const successModal = new bootstrap.Modal(
        document.getElementById('successModal')
    );
    successModal.show();

    //I need to add nav back to list
});


