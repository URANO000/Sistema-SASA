function Dashboard() {
  const cards = [
    { title: "Tiquetes Abiertos", value: 14, color: "bg-[#4EBBD6]" },
    { title: "En Progreso", value: 6, color: "bg-[#E00319]" },
    { title: "Pendientes", value: 4, color: "bg-amber-500" },
    { title: "Cerrados", value: 10, color: "bg-emerald-600" },
  ];

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-5 mt-2">
      {cards.map((card) => (
        <div
          key={card.title}
          className={`${card.color} p-5 rounded-xl shadow-md shadow-black/40 text-white`}
        >
          <p className="text-sm opacity-80">{card.title}</p>
          <p className="text-3xl font-bold mt-1">{card.value}</p>
        </div>
      ))}
    </div>
  );
}

export default Dashboard;
