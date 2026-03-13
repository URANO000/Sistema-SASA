//No mostrar el botón de asignar hasta que un tiquete esté seleccionado y alguien sea elegido
function validarAsignacion() {

    const selectedTickets = $(".ticket-checkbox:checked").length;
    const assignee = $("#assigneeSelect").val();

    const isValid = selectedTickets > 0 && assignee;

    $("#assignTicketsBtn").prop("disabled", !isValid);
}

//En caso de que algo sale pre-seleccionado
$(document).ready(function () {

    validarAsignacion();
    updateSelectedCount();

});

//Función para ver cuántos usuarios hay seleccionados
function updateSelectedCount() {

    const count = $(".ticket-checkbox:checked").length;

    if (count === 0)
        $("#selectedCount").text("");
    else
        $("#selectedCount").text(count + " tiquete(s) seleccionado(s)");
}

$(document).on("change", ".ticket-checkbox", function () {

    updateSelectedCount();
    validarAsignacion();

});

//Para seleccionar todos los tiquetes masivamente

$("#selectAllTickets").change(function () {

    const checked = $(this).prop("checked");

    $(".ticket-checkbox:not(:disabled)")
        .prop("checked", checked);

    updateSelectedCount();
    validarAsignacion();
});

//Validar al cambiar de asignado
$("#assigneeSelect").change(function () {
    validarAsignacion();
});

//Para asignar, aquí se llama el controlador

$("#assignTicketsBtn").click(function () {

    let selectedTickets = [];

    $(".ticket-checkbox:checked").each(function () {
        selectedTickets.push(parseInt($(this).val()));
    });

    const assigneeId = $("#assigneeSelect").val();

    if (selectedTickets.length === 0) {
        mostrarAlerta("Seleccione al menos un tiquete.");
        return;
    }

    if (!assigneeId) {
        mostrarAlerta("Debe seleccionar un usuario.");
        return;
    }

    $.ajax({
        url: "/Tiquete/AsignarTiquetes",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            idsTiquetes: selectedTickets,
            idAsignee: assigneeId
        }),
        success: function (response) {

            if (response.success) {

                mostrarSuccess(response.message);

                setTimeout(function () {
                    location.reload();
                }, 1200);
            }
            else {
                mostrarError(response.message);
            }
        },
        error: function () {
            mostrarError("Error inesperado asignando los tiquetes.");
        }
    });

});


//Algunos helpers

function mostrarSuccess(message) {

    $("#successModalMessage").text(message);
    $("#successModal").modal("show");
}

function mostrarError(message) {

    $("#errorModalMessage").text(message);
    $("#errorModal").modal("show");
}

function mostrarAlerta(message) {

    $("#alertModalMessage").text(message);
    $("#alertModal").modal("show");
}