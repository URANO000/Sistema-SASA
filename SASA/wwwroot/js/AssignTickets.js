function updateSelectedCount() {

    const count = $(".ticket-checkbox:checked").length;

    if (count === 0)
        $("#selectedCount").text("");
    else
        $("#selectedCount").text(count + " tiquete(s) seleccionado(s)");
}

$(document).on("change", ".ticket-checkbox", updateSelectedCount);

$("#selectAllTickets").change(function () {

    const checked = $(this).prop("checked");

    $(".ticket-checkbox:not(:disabled)")
        .prop("checked", checked);

    updateSelectedCount();
});


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