let assignAll = false;
$("#assignAllBanner").addClass("d-none");

function validarAsignacion() {

    const assignee = $("#assigneeSelect").val();
    const selectedCount = $(".ticket-checkbox:checked").length;
    const isValid = assignee && (assignAll || selectedCount > 0);
    $("#assignTicketsBtn").prop("disabled", !isValid);
}

function updateSelectedCount() {

    if (assignAll) {

        $("#selectedCount").text("Todos los tiquetes serán asignados.");
        return;
    }

    const count = $(".ticket-checkbox:checked").length;

    if (count === 0)
        $("#selectedCount").text("");
    else
        $("#selectedCount").text(count + " tiquete(s) seleccionado(s)");
}


$(document).on("change", ".ticket-checkbox", function () {

    assignAll = false;
    $("#selectAllTickets").prop("checked", false);
    updateSelectedCount();
    validarAsignacion();

});


$("#selectAllTickets").change(function () {

    const checked = $(this).is(":checked");

    $(".ticket-checkbox:not(:disabled)")
        .prop("checked", checked);

    assignAll = false;

    if (checked)
        $("#assignAllBanner").removeClass("d-none");
    else
        $("#assignAllBanner").addClass("d-none");

    updateSelectedCount();
    validarAsignacion();

});



$("#assigneeSelect").change(function () {
    validarAsignacion();
});

$("#confirmAssignAll").click(function () {

    assignAll = true;

    $("#assignAllBanner")
        .removeClass("alert-info")
        .addClass("alert-success");

    $("#assignAllBanner span").text(
        "Se asignarán todos los tiquetes que coinciden con los filtros actuales."
    );

    $("#confirmAssignAll").remove();

    updateSelectedCount();

    validarAsignacion();
});



$("#assignTicketsBtn").click(function () {

    const assigneeId = $("#assigneeSelect").val();

    const ids = $(".ticket-checkbox:checked")
        .map(function () {
            return Number(this.value);
        })
        .get();

    if (!assignAll && ids.length === 0) {
        mostrarAlerta("Seleccione al menos un tiquete.")
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
            asignacion: {
                idsTiquetes: ids,
                idAssignee: assigneeId,
                assignAll: assignAll
            },

            filtro: obtenerFiltrosActuales()
        }),

        success: function (response) {
            if (response.success) {
                mostrarSuccess(response.message);
                setTimeout(function () {
                    location.reload();
                }, 900);
            }
            else {
                mostrarAlerta(response.message);
            }
        },

        error: function () {
            mostrarError("Error inesperado asignando los tiquetes.");
            setTimeout(function () {
                location.reload();
            }, 900);
        }
    });

});

//Obtener filtros de la URL
function obtenerFiltrosActuales() {

    const params = new URLSearchParams(window.location.search);

    return {

        search: params.get("search"),
        estatus: params.get("estatus"),
        fechaInicio: params.get("fechaInicio"),
        fechaFinal: params.get("fechaFinal"),
        fecha: params.get("fecha"),
        vista: 0
    };
}


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