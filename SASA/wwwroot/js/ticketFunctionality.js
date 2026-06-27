const CANCELADO = parseInt($("#estatusDropdown").data("cancelado"));

function toggleResolucion() {
    const estatus = parseInt($("#estatusDropdown").val());

    if (estatus === CANCELADO) {
        $("#resolucionContainer").show();
    } else {
        $("#resolucionContainer").hide();
        $("#Resolucion").val("");
    }
}

toggleResolucion();
$("#estatusDropdown").on("change", toggleResolucion);