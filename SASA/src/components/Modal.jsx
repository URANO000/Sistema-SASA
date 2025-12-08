function Modal({ open, ticketId, onCancel, onConfirm }) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50">
      <div className="bg-slate-900 w-full max-w-md rounded-xl border border-slate-700 p-6 shadow-lg shadow-black/40">
        <h2 className="text-xl font-semibold text-white mb-3">
          Confirmar eliminación
        </h2>

        <p className="text-sm text-slate-300 mb-4">
          Estás a punto de eliminar un tiquete.{" "}
          <span className="text-[#FF1A3B] font-semibold">
            Esta acción es irreversible.
          </span>
        </p>

        {ticketId && (
          <p className="text-sm text-slate-200 mb-4">
            Tiquete seleccionado:{" "}
            <span className="font-semibold">{ticketId}</span>
          </p>
        )}

        <div className="flex justify-end gap-3">
          <button
            onClick={onCancel}
            className="px-4 py-2 bg-slate-800 rounded-lg text-sm hover:bg-slate-700"
          >
            Cancelar
          </button>

          <button
            onClick={onConfirm}
            className="px-4 py-2 bg-[#B7041A] text-white rounded-lg text-sm font-semibold hover:bg-[#9a0416]"
          >
            Confirmar
          </button>
        </div>
      </div>
    </div>
  );
}

export default Modal;
