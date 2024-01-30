using System.Data.SqlClient;

namespace Recaudacion.Data
{
    //Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Z:\baseejec.mdb

    public class Conexion
    {
        private string Provider;
        private string dataSource;
        private string cadenaConexion;
        private static Conexion con = null;

        //constructor
        private Conexion() { 
            this.Provider = "Provider = Microsoft.ACE.OLEDB.12.0";
            this.dataSource = "Data Source=Z:\\baseejec.mdb";
            this.cadenaConexion = this.Provider + ";" + this.dataSource;
        }
        public static Conexion getInstancia() 
        {
            if (con == null) 
                con = new Conexion();
            return con;
        }
        public SqlConnection crearConexion() 
        {
            SqlConnection MiConexion = new SqlConnection();
            try
            {
                MiConexion.ConnectionString = this.cadenaConexion;
                //miConexion.Open();
            }
            catch (Exception ex)
            {
                MiConexion = null;
            }
            return MiConexion;
        }
    }
}
