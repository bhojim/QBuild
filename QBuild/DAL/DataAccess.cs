using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QBuild.DAL
{
    public static class DataAccess
    {
        /// <summary>
        /// Get data for populating the treeview
        /// </summary>
        /// <returns>dataTable</returns>
        public static DataTable GetTreeData()
        {
            DataTable dt = new DataTable();

            string connString = ConfigurationManager.ConnectionStrings["QBuildConnection"].ConnectionString;
            string query = "select COMPONENT_NAME AS Name, PARENT_NAME AS Parent from bom";

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // Create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dt);
            conn.Close();
            da.Dispose();
            return dt;
        }

        /// <summary>
        /// Get data for populating the grid
        /// </summary>
        /// <param name="parentName"></param>
        /// <returns>dataTable</returns>
        public static DataTable GetGridData(string parentName)
        {
            DataTable dt = new DataTable();
            string connString = ConfigurationManager.ConnectionStrings["QBuildConnection"].ConnectionString;
            string query = "SELECT PARENT_NAME, COMPONENT_NAME, PART_NUMBER, TITLE, QUANTITY, [TYPE], ITEM, MATERIAL " +
                            "FROM bom " +
                            "LEFT JOIN Part ON bom.COMPONENT_NAME = Part.NAME " +
                            "WHERE PARENT_NAME = '" + parentName + "' " +
                            "ORDER BY COMPONENT_NAME";

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // Create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dt);
            conn.Close();
            da.Dispose();
            return dt;
        }

    }
}
