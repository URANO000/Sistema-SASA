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
                    setTimeout(() => location.reload(), 1200);
                    return;
                }
                else {
                    //caso 2, falla
                    $("#addUserModal .modal-content").html(response);

                    $.validator.unobtrusive.parse("#createUserForm");
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
//            url: form.attr("action"),
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