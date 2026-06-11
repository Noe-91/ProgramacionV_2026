namespace ProgramacionV_2026.Models
{
    public class ActividadViewModel

    {
        public int IdActividad { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan? Hora { get; set; }
        public string Lugar { get; set; }
        public string Responsable { get; set; }
        public string Estado { get; set; }
        public int IdTipoActividad { get; set; }
        public string TipoActividad { get; set; }
    }
}
