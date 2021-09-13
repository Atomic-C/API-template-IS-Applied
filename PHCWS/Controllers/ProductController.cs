using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using PHCWS.objects;
using WebApiAuthenticate.Filters;

namespace PHCWS.Controllers
{

    [BasicAuthentication]
    [EnableCorsAttribute("*", "*", "*")]
    public class ProductController : ApiController
    {
        private static string StrConn = @"Data Source=VMAZ;Initial Catalog=GFSC;User ID=PHCAPI;password=7h(8x*V,a3HcDGuF";
        /// <summary>
        /// Get list of all products avaiable on the API.
        /// </summary>
        // GET: Product
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetProducts()
        {
            List<ArtigoPHC> cl = new List<ArtigoPHC>();
            cl = GetListProducts();
            if (cl.Count != 0)
                return Request.CreateResponse(HttpStatusCode.OK, cl, Configuration.Formatters.JsonFormatter);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        private static List<ArtigoPHC> GetListProducts()
        {
            List<ArtigoPHC> Prods = new List<ArtigoPHC>();

            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                string query = "Select st.ref"+
                ", design"+
                ", convert(varchar(1000), stobs.stobs) as desccompleta"+
                ",st.epv1"+
                ",isnull((select taxa from taxasiva where codigo = st.tabiva),0) as IVA"+
                ",ST.U_FMQTT as lote"+
                ",st.Peso "+
                "from st(nolock) inner join stobs on stobs.ref= st.ref "+
                 "where st.inactivo = 0 and ST.QLOOK = 1";


                SqlCommand uquery = new SqlCommand(query, conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                float tempprice = 0.00F, tempiva = 0.00F, templote = 0.00F, temppeso = 0.00F;
                for (int i = 0; i < vrmTable.Rows.Count; i++)
                {
                    ArtigoPHC temp = new ArtigoPHC();
                    temp.referencia = vrmTable.Rows[i]["ref"].ToString().Trim(); 
                    temp.description = vrmTable.Rows[i]["design"].ToString().Trim();
                    temp.big_description = vrmTable.Rows[i]["desccompleta"].ToString().Trim();
                    float.TryParse(vrmTable.Rows[i]["epv1"].ToString().Trim(), out tempprice);
                    temp.price = tempprice;
                    float.TryParse(vrmTable.Rows[i]["IVA"].ToString().Trim(), out tempiva);
                    temp.iva= tempiva;
                    float.TryParse(vrmTable.Rows[i]["lote"].ToString().Trim(), out templote);
                    temp.minim_qtt = templote;
                    float.TryParse(vrmTable.Rows[i]["peso"].ToString().Trim(), out temppeso);
                    temp.weight = temppeso;
                    Prods.Add(temp);

                }

                return Prods;

            }
            catch (Exception ex)
            {
                return Prods;
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
        }
    }
}