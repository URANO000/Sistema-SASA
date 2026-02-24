using System.ComponentModel.DataAnnotations;

namespace DataAccess.Modelos.Enums
{
    public enum TiqueteEstatus
    {
        [Display(Name = "Creado")]
        Creado = 1,

        [Display(Name = "En Proceso")]
        EnProceso = 2,

        [Display(Name = "En Espera Del Usuario")]
        EnEsperaDelUsuario = 3,

        [Display(Name = "Cancelado")]
        Cancelado = 4,

        [Display(Name = "Resuelto")]

        Resuelto = 5
    }
}
