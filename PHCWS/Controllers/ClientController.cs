using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Newtonsoft.Json;
using PHCWS.objects;
using Swashbuckle.Swagger.Annotations;
using WebApiAuthenticate.Filters;

namespace PHCWS.Controllers
{

    
    [EnableCorsAttribute("*", "*", "*")]
    [BasicAuthentication]
    public class ClientController : ApiController
    {
        //private static string StrConn = @"Data Source=VMAZ;Initial Catalog=GFSC;User ID=PHCAPI;password=7h(8x*V,a3HcDGuF";
        private static string StrConn = @"Data Source=LPTITSSMOREIRA\SQL2019;Initial Catalog=GFSC;User ID=sa;password=12345678";
        // GET: Client
        /// <summary>
        /// Returns costumers data.
        /// </summary>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Cliente>))]
        [System.Web.Http.HttpGet]
        
        public HttpResponseMessage GetClientes()
        {
            List<Cliente> cl = new List<Cliente>();
            cl = GetClients();
            if (cl.Count!=0)
            return Request.CreateResponse(HttpStatusCode.OK, cl, Configuration.Formatters.JsonFormatter);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        private static List<Cliente> GetClients()
        {
            List<Cliente> clients = new List<Cliente>();
            
            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {
                
                DataTable vrmTable = new DataTable();

                conn.Open();
				string query = "Select clstamp,nome, no, estab, morada, codpost, local, ncont " +
                ", isnull((select taxa from taxasiva where codigo = cl.tabiva ),0) as IVA " +
                ",cl.tipodesc " +
                ",cl.codmotiseimp " +
                ",cl.motiseimp " +
                "from cl where inactivo = 0 and cl.dfront = 1";
                

                SqlCommand uquery = new SqlCommand(query, conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                int tempno = 0, tempestab = 0;
                float tempIVA = 0.00F;
                for (int i = 0; i < vrmTable.Rows.Count; i++)
                {
                    Cliente temp = new Cliente();
                    //temp.clstamp = vrmTable.Rows[i]["clstamp"].ToString();
                    temp.codpost = vrmTable.Rows[i]["codpost"].ToString();
                    temp.cod_motivo_isencao = vrmTable.Rows[i]["codmotiseimp"].ToString();
                    temp.desc_motivo_isencao = vrmTable.Rows[i]["motiseimp"].ToString();
                    int.TryParse(vrmTable.Rows[i]["estab"].ToString(), out tempestab);
                    temp.estab = tempestab;
                    float.TryParse(vrmTable.Rows[i]["IVA"].ToString(), out tempIVA);
                    temp.IVA = tempIVA;
                    temp.localidade = vrmTable.Rows[i]["local"].ToString();
                    temp.morada = vrmTable.Rows[i]["morada"].ToString();
                    temp.NIF = vrmTable.Rows[i]["ncont"].ToString();
                    int.TryParse(vrmTable.Rows[i]["no"].ToString(), out tempno);
                    temp.no = tempno;
                    temp.nome = vrmTable.Rows[i]["nome"].ToString();
                    temp.tipodesconto = vrmTable.Rows[i]["tipodesc"].ToString();
                    clients.Add(temp);
                    
                }
                
                return clients;

            }
            catch (Exception ex)
            {
                return clients;
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