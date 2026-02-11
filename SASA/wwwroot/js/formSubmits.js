//--------------------------------------------------------------------
//Esto es para USUARIO ADD CONTROLLER, utilizando JQUERY
$(function () {
    $(document).on("submit", "#createUserForm", function (e) {
        e.preventDefault();

        const form = $(this);

        // Guard por si falta jquery.validate
        if (!$.validator || !$.validator.unobtrusive) {
            console.error("Faltan scripts de jquery.validate / unobtrusive");
            return;
        }

        if (!form.valid()) {
            return;
        }

        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (response) {

                //Caso 1, success de JSON
                if (response.success) {
                    const addModalEl = document.getElementById("addUserModal");
                    bootstrap.Modal.getInstance(addModalEl)?.hide();

                    //Abrir vista parcial/modal de éxito
                    const successModalEl = document.getElementById("successModal");
                    new bootstrap.Modal(successModalEl).show();

                    //Reload de tabla
                    setTimeout(() => location.reload(), 1200);
                    return;
                }
                else {
                    //caso 2, falla
                    const newForm = $("#addUserModal").find("form#createUserForm");

                    newForm.removeData("validator");
                    newForm.removeData("unobtrusiveValidation");
                    $.validator.unobtrusive.parse(newForm);

                }
            },
            error: function () {
                alert("Ocurrió un error inesperado.");
            }
        });
    });
});

//--------------------------------------------------------------------
//DISABLE CONFIRM PARA USUARIOS
//$(function () {
//    $(document).on("submit", "#disableConfirm", function (e) {
//        e.preventDefault();

//        const form = $(this);

//        if (!form.valid()) {
//            return;
//        }

//        $.ajax({
//            url: form.attr("action"),//
//            type: "POST",
//            data: form.serialize(),
//            success: function (response) {

//                //Caso 1, success de JSON
//                if (response.success) {
//                    $("#disableConfirm").modal("hide");

//                    //Abrir vista parcial/modal de éxito
//                    $("#successModal").modal("show");

//                    //Reload de tabla
//                    setTimeout(() => location.reload(), 1200);
//                    return;
//                }
//                else {
//                    //caso 2, falla
//                    $("#addUserModal .modal-content").html(response);

//                    $.validator.unobtrusive.parse("#disableConfirm");
//                }
//            },
//            error: function () {
//                alert("Ocurrió un error inesperado.");
//            }
//        });
//    })
//})


//--------------------------------------------------------------------
//EDITAR USUARIO SUBMIT
$(function () {
    $(document).on("submit", "#editUserForm", function (e) {
        e.preventDefault();

        const form = $(this);
        if (!form.valid()) {
            return;
        }

        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (response) {

                //Caso 1, si todo sale bien
                if (response.success) {
                    $("#successModal").modal("show");

                    //Ir a la tabla después de un tiempo
                    setTimeout(() => {
                        window.location.href = "/Usuario";
                    }, 1200);
                }

                if (response.error) {
                    //Limpiar mensajes previos
                    form.find("[data-valmsg-for]").text("");

                    //Mostrar errores
                    $.each(response.errors, function (key, messages) {
                        if (key === "_form") {
                            alert(messages[0]);
                            return;
                        }

                        const span = form.find(`[data-valmsg-for="${key}"]`);
                        if (span.length) {
                            span.text(messages.join(", "));
                        }
                    });
                }
            },
            error: function () {
                alert("Ocurrió un error inesperado.");
            }
        });
    });
});