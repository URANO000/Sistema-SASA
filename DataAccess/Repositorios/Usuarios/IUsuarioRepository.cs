using DataAccess.Modelos.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositorios.Usuarios
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ObtenerUsuariosAsync(); //Listar
        Task<Usuario?> ObtenerUsuarioPorIdAsync(int id); //Detalle
        Task AgregarUsuarioAsync(Usuario usuario); //Agregar
        Task ActualizarUsuarioAsync(Usuario usuario); //Actualizar
        Task DesactivarUsuario(int id); //Desactivar. Nunca eliminar
    }
}
