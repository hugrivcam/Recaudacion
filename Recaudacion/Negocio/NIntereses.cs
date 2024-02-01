using Recaudacion.Data;
using Recaudacion.modelos;
using System.Data;
using System.Runtime.CompilerServices;

namespace Recaudacion.Negocio
{
    public class NInteresesServicio
    {
        public static decimal CalcularInteres(MReciboInteres obj, DateTime fecha_interes)
        {
            const decimal MINIMO = 30.05M;
            decimal demoratotal = 0;
            decimal idem = 0;
            decimal demora = 0;
            decimal intereses = 0;
            //decimal interes_costas; //i_cost
            int num_costas = 0;
            int cont_costas = 0;
            int num_ingresos = 0;
            int cont_ingresos = 0;
            decimal importe_parcial,importe_pendiente;
            decimal importeIngreso,importeCosta;
            DateTime fechaIngreso,fechaCosta;
            DataTable TIngresos;
            DataTable TCostas;//string Num_recibo = obj.Num_Recibo;
            DateTime FechaInicial,FechaFinal;
            try
            {
                DDataCalculoInteres DMiCalculo = new DDataCalculoInteres();
                if (obj.Principal > MINIMO && obj.Tipo_Interes > 0 && fecha_interes > obj.Fecha_venc_108 && obj.Fecha_venc_vol != null && obj.Recargo > Math.Round(obj.Principal / 10, 2))
                {
                    TCostas = DMiCalculo.ListarCostas(obj.Num_Recibo);
                    TIngresos = DMiCalculo.ListarIngresos(obj.Num_Recibo, fecha_interes);
                    num_costas = TCostas.Rows.Count;
                    cont_costas = 0;
                    num_ingresos = TIngresos.Rows.Count;
                    cont_ingresos = 0;
                    if (obj.Fecha_inicio_ejecutiva != null)
                        FechaInicial = (DateTime)(obj.Fecha_inicio_ejecutiva.Value.AddDays(-1)); //decido cual es mi fecha incial para contabilizar intereses
                    else
                        FechaInicial = (DateTime)obj.Fecha_venc_vol;
                    //FechaFinal = fecha_interes; //vamos a cacular intereses desde la fecha inicial hasta la fecha final, si no hay ingresos esta será la fecha final
                    //importe_parcial = obj.Principal;//si no hay ingresos todo el el importe será el importe parcial
                    importe_pendiente = obj.Principal;//en VBA era contador_pendiente//al inicio tenemos que calcular intereses sobre el importe total pendiente, luego iremos restando segun vayamos calculando tramos hasta llegar a cero.
                    //primero calculamos los intereses en los tramos que hay ingresos
                    //int i = 0;
                    foreach (DataRow fila in TIngresos.Rows)
                    {
                        cont_ingresos++;
                        importeIngreso = (decimal)fila["importe"];
                        importe_parcial = importeIngreso;
                        fechaIngreso = (DateTime)fila["fecha"];//no pueden ser nulos por la naturaleza de la consulta
                        FechaFinal = fechaIngreso;
                        if (num_costas > 0 && cont_costas < num_costas) //las costas no generan intereses, pero como los ingresos se aplican primero a costas, las costas anteriores restan a los ingresos posteriores
                        {
                            fechaCosta = (DateTime)TCostas.Rows[cont_costas]["fecha"];//las costas sólo las recorro una vez hacia adelante, no puedo por cada ingreso recorrer todas las costas, siempre recorro a partir de donde estoy posicionado
                            while (cont_costas < num_costas && fechaCosta < fechaIngreso && importe_parcial > 0)
                            {
                                DataRow filaCosta = TCostas.Rows[cont_costas];
                                fechaCosta = (DateTime)filaCosta["fecha"];
                                importeCosta = (decimal)filaCosta["importe"];
                                importe_parcial -= importeCosta;
                                if (importe_parcial < 0) importe_parcial = 0;
                                cont_costas++;
                            }
                        }
                        if (importe_pendiente >= importe_parcial)
                            importe_pendiente -= importe_parcial;//mientras hay importe pendiente hay que contabilizar intereses
                        else
                        {
                            importe_parcial = importe_pendiente;
                            importe_pendiente = 0;
                        }
                        if (importe_parcial > 0)
                        {
                            intereses += calcular_interes_demora(FechaInicial, FechaFinal, importe_parcial, obj.Tipo_Interes);

                        }

                        if (importe_pendiente <= 0) break;
                    }
                    if (importe_pendiente > 0)
                    {
                        FechaFinal = fecha_interes;
                        if (importe_pendiente > 0)
                        {
                            intereses += calcular_interes_demora(FechaInicial, FechaFinal, importe_pendiente, obj.Tipo_Interes);
                        }

                    }

                }
            }
            catch (Exception ex) 
            {
                intereses = -1;
                Console.Error.WriteLine("Error calculando los intereses: " + ex.Message);
            }
            return intereses;
        }

        private static decimal calcular_interes_demora(DateTime fi, DateTime ff, decimal importe, int clase_interes) {
            
            int estado = 0;
            decimal interes = 0;
            int num_reg = 0;
            int dif_dias = 0;
            decimal tipo_ant = 0;
            DateTime fecha_aplic;
            decimal tipo_Interes;
            try 
            {
                DateTime f_ant = new DateTime(1900, 1, 1);
                DDataCalculoInteres origen = new DDataCalculoInteres();
                DataTable listaIntereses = origen.ListarInteres_Demora(clase_interes);
                fi = fi.AddDays(1); //PRIN2
                ff = ff.AddDays(-1);//FIN
                num_reg = listaIntereses.Rows.Count;
                foreach(DataRow fila in listaIntereses.Rows)
                {
                    fecha_aplic = (DateTime)fila["fecha_aplic"];
                    tipo_Interes = (decimal)fila["tipo_interes"];
                    if (estado == 0)
                    {
                        if (fi>f_ant && fi < fecha_aplic) //si la fecha incial cabe entre una fecha primigenia y la fecha del interes actual la contabilizamos
                        {
                            if (ff > fecha_aplic) //si mi fecha final es mayor que la fecha de mi interes actual tendré que contar dias entre mi fecha inicial y la fecha de mi interes actual
                            {
                                dif_dias = (fecha_aplic - fi).Days;
                                estado = 1;
                                interes += (importe * dif_dias * tipo_ant / 36500);
                            }
                            else
                            {
                                dif_dias = (ff - fi).Days + 1;//he lleado al final
                                estado = 2;
                                interes += (importe * dif_dias * tipo_ant / 36500);
                                break;
                            }

                        }
                    }
                    else 
                    {
                        if (estado == 1) //tramos intermedios
                        {
                            if (ff > fecha_aplic) //si mi fecha final es mayor que la fecha de mi interes actual tendré que contar dias entre mi fecha inicial y la fecha de mi interes actual
                            {
                                dif_dias = (fecha_aplic - f_ant).Days;
                                //estado = 1;
                                interes += (importe * dif_dias * tipo_ant / 36500);
                            }
                            else
                            {
                                dif_dias = (ff - f_ant).Days + 1;//he lleado al final
                                estado = 2;
                                interes += (importe * dif_dias * tipo_ant / 36500);
                                break;
                            }
                        }
                    }
                    f_ant = fecha_aplic;
                    tipo_ant = tipo_Interes;
                }
                //aun queda contabilizar un tramo final
                if (estado == 0)
                {
                    dif_dias = (ff - fi).Days + 1;//he lleado al final
                    interes += (importe * dif_dias * tipo_ant / 36500);
                }
                if (estado == 1) 
                {
                    dif_dias = (ff - f_ant).Days + 1;//he lleado al final
                    interes += (importe * dif_dias * tipo_ant / 36500);
                }
                return interes;

            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Console.Error.WriteLine("Error calcunlando intervalo de intereses: " + ex.Message);
            }

            return 0;
        }
        public static DataTable ListarSinInteres(int nPagina, int tamPagina, string where)
        {
            DDataCalculoInteres dObj = new DDataCalculoInteres();
            return dObj.ListarRecibos(where);
            
        }

        private static MReciboInteres MapearDataRowAMReciboInteres(DataRow fila)
        {
            decimal interes;
            MReciboInteres providencia = new MReciboInteres();
            providencia.Num_Recibo = (string)fila["num_recibo"];
            providencia.Fecha_venc_108 = (DateTime)fila["Fecha_venc_108"];
            providencia.Fecha_inicio_ejecutiva = (DateTime)fila["Fecha_inicio_ejecutiva"];
            providencia.Fecha_venc_vol = (DateTime)fila["Fecha_venc_vol"];
            providencia.Tipo_Interes = (int)fila["tipo_interes"];
            providencia.Costas = (decimal)fila["Costas"];
            providencia.Deuda_Actual = (decimal)fila["Deuda_actual"];
            providencia.Ingresos = (decimal)fila["Ingresos"];
            providencia.Ingresos_Datados = (decimal)fila["Ingresos_datados"];
            providencia.Ingresos_Zona = (decimal)fila["Ingresos_zona"];
            providencia.InteresAplz = (decimal)fila["InteresAplz"];
            providencia.Interes_Demora = (decimal)fila["Interes_Demora"];
            interes = CalcularInteres(providencia, DateTime.Now);
            fila["Interes_Demora"] = interes;//modifico el interes del dataTable por si acaso lo necesito en algun momento actualizado
            providencia.Interes_Demora = interes;
            return providencia;
        }
        //este metodo es para hacer pruebas, no creo que lo vaya a usar
        public static DataTable ListarConInteres(string where)
        {
            //MReciboInteres providencia = new MReciboInteres();
            decimal interes;
            //string num_recibo="";
            DDataCalculoInteres DObj = new DDataCalculoInteres();
            DataTable t = DObj.ListarRecibos(where);
            foreach (DataRow fila in t.Rows) 
            {
                MReciboInteres providencia = MapearDataRowAMReciboInteres(fila);
            }
            return t;
        }
        
        public static MReciboInteres[] ListarConInteresPaginado(int nPag,int tamPag,string where)
        {
            List<MReciboInteres> listaProvidencias = new List<MReciboInteres>();
            //decimal interes;
            //string num_recibo = "";
            DDataCalculoInteres DObj = new DDataCalculoInteres();
            DataTable t = DObj.ListarRecibosPagina(nPag,tamPag, where);
            foreach (DataRow fila in t.Rows)
            {
                MReciboInteres providencia = MapearDataRowAMReciboInteres(fila);
                listaProvidencias.Add(providencia);
            }
            return listaProvidencias.ToArray();
        }

        public Task<MReciboInteres[]> GetListaInteresPaginadoAsync(int nPag, int tamPag, string where) 
        {
            
            MReciboInteres[] lista = ListarConInteresPaginado(nPag, tamPag, where);
            int tam = lista.Length;
            return Task.FromResult(lista);
        }

    }
}
