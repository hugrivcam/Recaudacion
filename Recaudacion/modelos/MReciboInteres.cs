namespace Recaudacion.modelos
{
    public class ReciboInteres
    {
        public string num_recibo { get; set; }
        public long num_recibo_n { get; set; }
        public decimal principal { get; set; }
        public decimal recargo { get; set; }
        public decimal costas { get; set; }
        public decimal interes_demora { get; set; }
        public decimal ingresos { get; set; }
        public decimal ingresos_zona { get; set; }
        public decimal ingresos_datados { get; set; }
        public decimal deuda_actual{ get; set; }

    }
}
