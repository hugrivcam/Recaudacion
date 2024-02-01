using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Recaudacion.modelos;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
//using System.Runtime.CompilerServices;

namespace Recaudacion.Data
{
    public class DDataCalculoInteres
    {
        //private string num_recibo;
        //private string where;
        //private DateTime fecha;
        public string LastError;
        //private ReciboInteres[] listaRecibos;
        /*public DDataCalculoInteres(string where) //, DateTime f) 
        {
            this.where = where;
            //this.fecha = f;
            //listaRecibos = null;
        }*/

        public DataTable ListarRecibos(string where)
        { 
            string sql;
            SqlConnection sqlCon = null;
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            try 
            {
                sqlCon = Conexion.getInstancia().crearConexion();
                if(sqlCon != null )
                {
                    sql = "select num_recibo,principal,recargo,costas,interes_demora,ingresos,igresos_zona,ingresos_datados,deuda_actual,tipo_interes, Fecha_venc_vol,Fecha_venc_108,Fecha_inicio_ejecutiva,InteresAplz from recibos where " + where + " order by num_recibo";
                    SqlCommand cmd = new SqlCommand(sql,sqlCon);
                    cmd.CommandType = CommandType.Text;
                    sqlCon.Open();
                    Resultado = cmd.ExecuteReader();
                    Tabla.Load(Resultado); 
                    sqlCon.Close();
                    Console.WriteLine("listar recibos existoso.");
                }
            }
            catch (Exception ex) {
                LastError = "Error en ListaRecibos: " + ex.Message ;
                Console.WriteLine(LastError);
            }
            return Tabla;               
        }
        public DataTable ListarCostas(string num_recibo)
        {
            string sql;
            SqlConnection sqlCon = null;
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            try
            {
                sqlCon = Conexion.getInstancia().crearConexion();
                if (sqlCon != null)
                {
                    sql = "select num_recibo,importe,fecha from relacion_costas where num_recibo = '" + num_recibo + "'fecha_anulacion is null order by fecha";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.CommandType = CommandType.Text;
                    sqlCon.Open();
                    Resultado = cmd.ExecuteReader();
                    Tabla.Load(Resultado);
                    sqlCon.Close();
                    Console.WriteLine("listar costas existoso.");
                }
            }
            catch (Exception ex)
            {
                LastError = "Error en ListarCostas: " + ex.Message;
                Console.WriteLine(LastError);
            }
            return Tabla;
        }

        public DataTable ListarIngresos(string num_recibo,DateTime Fecha_interes)
        {
            string sql;
            SqlConnection sqlCon = null;
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            try
            {
                sqlCon = Conexion.getInstancia().crearConexion();
                if (sqlCon != null)
                {
                    sql = "select importe,fecha from relacion_ingresos_cuenta where num_recibo = '" + num_recibo + "' AND fecha < #" + Fecha_interes.ToString("MM/dd/yyyy") + "# order by fecha" ;//ojo comprobar esta linea para ver si la fecha se compara correctamente
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.CommandType = CommandType.Text;
                    sqlCon.Open();
                    Resultado = cmd.ExecuteReader();
                    Tabla.Load(Resultado);
                    sqlCon.Close();
                    Console.WriteLine("listar ingresos existoso.");
                }
            }
            catch (Exception ex)
            {
                LastError = "Error en ListarIngresos: " + ex.Message;
                Console.WriteLine(LastError);
            }
            return Tabla;
        }

        public DataTable ListarInteres_Demora(int clase_interes)
        {
            string sql;
            SqlConnection sqlCon = null;
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            try
            {
                sqlCon = Conexion.getInstancia().crearConexion();
                if (sqlCon != null)
                {
                    sql = "select fecha_aplic,clase_interes,tipo_interes where clase_interes = " + clase_interes + " order by fecha_aplic asc";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.CommandType = CommandType.Text;
                    sqlCon.Open();
                    Resultado = cmd.ExecuteReader();
                    Tabla.Load(Resultado);
                    sqlCon.Close();
                    Console.WriteLine("listar interes existoso.");
                }
            }
            catch (Exception ex)
            {
                LastError = "Error en ListaInteres_Demora: " + ex.Message;
                Console.WriteLine(LastError);
            }
            return Tabla;
        }


        public DataTable ListarRecibosPagina(int nPagina ,int tamPagina, string where)
        {
            string sql;
            SqlConnection sqlCon = null;
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            int rf,ri;//registro final
            int rc;// recordCount
            ri = tamPagina * nPagina;
            rf = ri + tamPagina; 
            try
            {
                sqlCon = Conexion.getInstancia().crearConexion();
                if (sqlCon != null)
                {
                    sql = "select count(*) from recibos where " + where;
                    //rc = (int?)new SqlCommand(sql, sqlCon).ExecuteScalar();
                    rc = Convert.ToInt32(new SqlCommand(sql, sqlCon).ExecuteScalar());
                    if (rf > rc)  
                        rf = rc; 
                    sql = "select * from (select top " + tamPagina + " * from (select TOP " + rf + " num_recibo,principal,recargo,costas,interes_demora,ingresos,igresos_zona,ingresos_datados,deuda_actual,tipo_interes,Fecha_venc_vol,Fecha_venc_108,Fecha_inicio_ejecutiva,InteresAplz from recibos where " + where + " order by num_recibo asc) order by num_recibo desc) order by num_recibo";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.CommandType = CommandType.Text;
                    sqlCon.Open();
                    Resultado = cmd.ExecuteReader();
                    Tabla.Load(Resultado);
                    sqlCon.Close();
                    Console.WriteLine("listar recibosPagina existoso.");
                }
            }
            catch (Exception ex)
            {
                LastError = "Error en ListaRecibosPagina: " + ex.Message;
                Console.WriteLine(LastError);
            }
            return Tabla;
        }
    }
}
