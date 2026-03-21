const queue = document.getElementById("ticketQueue");

new Sortable(queue, {
    animation: 200,
    ghostClass: "sortable-ghost",

    onEnd: function (evt) {
        handleReorder(evt);
    }
});

function handleReorder(evt) {
    const item = evt.item;

    const prev = item.previousElementSibling;
    const next = item.nextElementSibling;

    const id = item.dataset.id;

    const ordenAnterior = prev ? parseFloat(prev.dataset.orden) : null;
    const ordenSiguiente = next ? parseFloat(next.dataset.orden) : null;

    fetch("/Cola/Reordenar", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            idTiquete: id,
            ordenAnterior: ordenAnterior,
            ordenSiguiente: ordenSiguiente
        })
    })
        .then(r => r.json())
        .then(data => {
            //Update del DOM con nuevo orden, visual
            item.dataset.orden = data.nuevoOrden;

            updatePositionsUI();
        });
}

function updatePositionsUI() {
    const cards = document.querySelectorAll("#ticketQueue .notif-item");

    cards.forEach((card, index) => {
        const badge = card.querySelector(".notif-icon");
        if (badge) {
            badge.textContent = "#" + (index + 1);
        }
    });
}