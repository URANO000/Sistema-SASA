//--------------------------------------------------------------------
//Esto es para USUARIO ADD CONTROLLER, utilizando JQUERY
$(function () {
    $(document).on("submit", "#createUserForm", function (e) {
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

                //Caso 1, success de JSON
                if (response.success) {
                    $("#addUserModal").modal("hide");

                    //Abrir vista parcial/modal de éxito
                    $("#successModal").modal("show");

                    //Reload de tabla
                    setTimeout(() => location.reload(), 900);
                    return;
                }
                else {
                    //caso 2, falla
                    $("#addUserModal .modal-content").html(response);

                    $.validator.unobtrusive.parse("#createUserForm");

                    $("#addUserModal").modal("hide");

                    $("#errorModal").modal("show")
                    setTimeout(() => {
                        window.location.href = "/Usuario";
                    }, 900);
                }
            },
            error: function () {
                $("#addUserModal").modal("hide");
                $("#errorModal").modal("show")
                setTimeout(() => {
                    window.location.href = "/Usuario";
                }, 900);
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
                    }, 900);
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
                $("#errorModal").modal("show")
                setTimeout(() => {
                    window.location.href = "/Usuario";
                }, 900);
            }
        });
    });
});

//--TIQUETE SUBMIT------------
$(function () {

    $(document).on("submit", "#crearTiqueteForm", function (e) {

        e.preventDefault();

        const form = document.getElementById("crearTiqueteForm");
        const files = form.querySelector('input[type="file"]').files;

        // Client-side size validation
        for (let i = 0; i < files.length; i++) {
            if (files[i].size > 5 * 1024 * 1024) {
                alert("Un archivo supera el límite de 5MB.");
                return;
            }
        }

        // jQuery validation
        if (!$(form).valid()) {
            return;
        }

        const formData = new FormData(form);

        $.ajax({
            url: form.action,
            type: "POST",
            data: formData,
            processData: false,  
            contentType: false,  

            success: function (response) {

                if (response.success) {
                    $("#addTicket").modal("hide");
                    $("#successModal").modal("show");

                    setTimeout(() => {
                        window.location.href = "/Tiquete";
                    }, 900);
                }
                else {
                    $("#addTicket .modal-content").html(response);
                    $.validator.unobtrusive.parse("#crearTiqueteForm");
                }
            },

            error: function () {
                $("#addTicket").modal("hide");
                $("#errorModal").modal("show");

                setTimeout(() => {
                    window.location.href = "/Tiquete";
                }, 900);
            }
        });
    });
});

//-------CARGAR SUB-CATEGORÍAS DINÁMICAMENTE -----------------
$("#categoriaDropdown").on('change', function () {

    var categoriaId = $(this).val();
    var subDropdown = $("#subcategoriaDropdown");

    subDropdown.empty(); // remove previous options

    subDropdown.append('<option value="">Seleccione una subcategoría</option>');

    if (!categoriaId)
        return;

    $.get("/Tiquete/ObtenerSubCategorias", { idCategoria: categoriaId }, function (data) {

        $.each(data, function (index, sub) {

            subDropdown.append(
                $('<option>', {
                    value: sub.idSubCategoria,
                    text: sub.nombreSubCategoria,
                    "data-prioridad": sub.nombrePrioridad
                })
            );

        });

    });
});

$("#subcategoriaDropdown").on('change', function () {

    var selectedOption = $(this).find(":selected");
    var prioridad = selectedOption.data("prioridad");

    $("#prioridadInput").val(prioridad);

});


//--TIQUETE EDITAR SUBMIT------------               
$(function () {
    $(document).on("submit", "#editTicketForm", function (e) {
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
                        window.location.href = "/Tiquete";
                    }, 900);
                }
                else { 
                    //Limpiar mensajes previos
                    form.find("[data-valmsg-for]").text("");

                    //Mostrar errores
                    $.each(response.errors, function (key, messages) {
                        if (key === "_form") {
                            $("#mensaje").text(messages[0])
                            $("#alertModal").modal("show")
                            setTimeout(() => {
                                $("#alertModal").modal("hide");
                            }, 900);
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
                $("#errorModal").modal("show")
                setTimeout(() => {
                    window.location.href = "/Tiquete";
                }, 900);
            }
        });

    });

});

//------------PARA AVANCE SUBMIT---------------------------
$(function () {
    $(document).on("submit", "#addAdvanceForm", function (e) {

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
                    $("#avanceModal").modal("hide");
                    $("#successModal").modal("show");

                    //No se vá a ningún lugar, sólo se queda en detalle
                    setTimeout(() => {
                        $("#successModal").modal("hide");
                        location.reload();
                    }, 900);
                }
                else {
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
                $("#avanceModal").modal("hide");
                $("#errorModal").modal("show")
                setTimeout(() => {
                    $("#errorModal").modal("hide")
                }, 1400);
            }
        });

    });

});
