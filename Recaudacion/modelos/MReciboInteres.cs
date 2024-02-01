namespace Recaudacion.modelos
{
    public class MReciboInteres
    {
        public string Num_Recibo { get; set; }
        //public long num_recibo_n { get; set; }
        public decimal Principal { get; set; }
        public decimal Recargo { get; set; }
        public decimal Costas { get; set; }
        public decimal Interes_Demora { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Ingresos_Zona { get; set; }
        public decimal Ingresos_Datados { get; set; }
        public decimal Deuda_Actual { get; set; }
        public int Tipo_Interes { get; set; }
        public DateTime? Fecha_venc_vol { get; set; }
        public DateTime? Fecha_venc_108 { get; set; }
        public DateTime? Fecha_inicio_ejecutiva { get; set; } //? pueden ser nulos
        public decimal InteresAplz {  get; set; }

        public MReciboInteres()
        {
            Num_Recibo = "";
        }

    }
}
