using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Recaudacion.modelos;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
//using System.Runtime.CompilerServices;

namespace Recaudacion.Data
{
    public class DCalcInteres
    {
        //private string num_recibo;
        private string where;
        private DateTime fecha;
        public string lastError;
        //private ReciboInteres[] listaRecibos;
        public DCalcInteres(string where) //, DateTime f) 
        {
            this.where = where;
            //this.fecha = f;
            //listaRecibos = null;
        }

        public DataTable ListarRecibos()
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
                    sql = "select num_recibo,principal,recargo,costas,interes_demora,ingresos,igresos_zona,ingresos_datados from recibos where " + where + " order by num_recibo";
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
                lastError = "Error en ListaRecibos: " + ex.Message ;
                Console.WriteLine(lastError);
            }
            return Tabla;               
        }
        public DataTable ListarRecibosPagina(int nPagina ,int tamPagina )
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
                    sql = "select * from (select top " + tamPagina + " * from (select TOP " + rf + " num_recibo,principal,recargo,costas,interes_demora,ingresos,igresos_zona,ingresos_datados from recibos where " + where + " order by num_recibo asc) order by num_recibo desc) order by num_recibo";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.CommandType = CommandType.Text;
                    sqlCon.Open();
                    Resultado = cmd.ExecuteReader();
                    Tabla.Load(Resultado);
                    sqlCon.Close();
                    Console.WriteLine("listar recibos existoso.");
                }
            }
            catch (Exception ex)
            {
                lastError = "Error en ListaRecibos: " + ex.Message;
                Console.WriteLine(lastError);
            }
            return Tabla;
        }
    }
}
