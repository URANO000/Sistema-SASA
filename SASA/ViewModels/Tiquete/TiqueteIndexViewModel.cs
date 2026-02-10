using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Modelos.DTOs.Prioridad;
using DataAccess.Modelos.DTOs.Usuarios;

namespace SASA.ViewModels.Tiquete
{
    public class TiqueteIndexViewModel
    {
        //Primero el listado
        public IReadOnlyList<TiqueteListaViewModel> Tiquetes { get; init; } = []; //Evita nulos
        //Precargar datos para el formulario de creación y edición
        public IReadOnlyList<ListaCategoriaDto> CategoriasDisponibles { get; init; } = [];
        public IReadOnlyList<ListaPrioridadDto> PrioridadesDisponibles { get; init; } = [];
        public IReadOnlyList<UsuarioTIDropdownDto> UsuariosTIDisponibles { get; init; } = [];

        //Crear tiquete viewmodel
        public CrearTiqueteViewModel CrearTiquete { get; set; } = new()
        {
            Asunto = string.Empty,
            Descripcion = string.Empty,
            Categoria = 0,
            Prioridad = 0,
            IdAsignee = string.Empty
        };
    }
}
