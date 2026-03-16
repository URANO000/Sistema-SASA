const queue = document.getElementById("ticketQueue");

new Sortable(queue, {
    animation: 200,
    ghostClass: "sortable-ghost",

    onEnd: function () {
        updatePositions();
    }
});

function updatePositions() {
    const cards = document.querySelectorAll(".ticket-card");

    cards.forEach((card, index) => {
        card.querySelector(".queue-position").textContent = "#" + (index + 1);
    });
}