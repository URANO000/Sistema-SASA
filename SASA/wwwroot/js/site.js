// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
console.log(":)");

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


