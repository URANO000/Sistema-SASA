import {useState} from "react";
function Formulario() {
  const [form, setForm] = useState({ nombre: "", correo: "" });
  const [errors, setErrors] = useState({});
  const [submitted, setSubmitted] = useState(false);

  const validate = () => {
    const newErrors = {};

    if (!form.nombre.trim()) newErrors.nombre = "Este campo es obligatorio.";
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.correo))
      newErrors.correo = "Formato de correo inválido.";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (validate()) {
      setSubmitted(true);
      setTimeout(() => setSubmitted(false), 2500);
    }
  };

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setErrors({ ...errors, [e.target.name]: undefined });
  };

  return (
    <div className="bg-slate-900/60 p-6 rounded-xl border border-slate-800 shadow-md shadow-black/40">
      <h2 className="text-xl font-semibold text-white mb-4">
        Formulario con Validación
      </h2>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm mb-1">Nombre</label>
          <input
            name="nombre"
            type="text"
            className={`w-full px-3 py-2 rounded-lg bg-slate-950 border ${
              errors.nombre ? "border-[#E00319]" : "border-slate-700"
            } text-sm`}
            value={form.nombre}
            onChange={handleChange}
          />
          {errors.nombre && (
            <p className="text-[#FF1A3B] text-xs mt-1">{errors.nombre}</p>
          )}
        </div>

        <div>
          <label className="block text-sm mb-1">Correo</label>
          <input
            name="correo"
            type="email"
            className={`w-full px-3 py-2 rounded-lg bg-slate-950 border ${
              errors.correo ? "border-[#E00319]" : "border-slate-700"
            } text-sm`}
            value={form.correo}
            onChange={handleChange}
          />
          {errors.correo && (
            <p className="text-[#FF1A3B] text-xs mt-1">{errors.correo}</p>
          )}
        </div>

        <button
          type="submit"
          className="bg-[#B7041A] text-white px-4 py-2 rounded-lg text-sm font-semibold hover:bg-[#9a0416] transition-colors"
        >
          Guardar
        </button>
        {submitted && (
          <p className="text-emerald-400 text-sm mt-2">
            Guardado exitosamente.
          </p>
        )}
      </form>
    </div>
  );
}

export default Formulario;
