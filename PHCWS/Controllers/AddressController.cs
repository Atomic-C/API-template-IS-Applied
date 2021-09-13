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
using Swashbuckle.Swagger.Annotations;
using WebApiAuthenticate.Filters;

namespace PHCWS.Controllers
{
    
    [BasicAuthentication]
    [EnableCorsAttribute("*", "*", "*")]
    public class AddressController : ApiController
    {
        private static string StrConn = @"Data Source=VMAZ;Initial Catalog=GFSC;User ID=PHCAPI;password=7h(8x*V,a3HcDGuF";
        /// <summary>
        /// Returns costumers delivery addresses.
        /// </summary>
        /// <param name="estab">Estab of the client</param>
        /// <param name="no">Number of the client</param>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Morada>))]
        [System.Web.Http.HttpGet]
        // GET: Address
        public HttpResponseMessage GetAdresses(int no, int estab)
        {
            List<Morada> cl = new List<Morada>();
            cl = GetAddreses(no,estab);
            if (cl.Count != 0)
                return Request.CreateResponse(HttpStatusCode.OK, cl, Configuration.Formatters.JsonFormatter);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        /// <summary>
        /// creates a new costumer delivery address.
        /// </summary>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Morada))]
        // POST: Orders/postEncomenda
        [System.Web.Http.HttpPost]
        public HttpResponseMessage postNewAddress([FromBody] Morada morada)
        {
            if (ModelState.IsValid)
            {
                if (morada.no == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, "Número de cliente tem de ser sempre maior que 0");
                }
                if (CheckAddress(morada.descricao) == true)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Desculpe, mas já existe uma morada com essa descrição.");
                }
                else
                {
                    string resposta = "";
                    resposta = NewAdress(morada);
                    if (resposta == "true")
                        return Request.CreateResponse(HttpStatusCode.Created, "Morada criada com sucesso!");
                    else
                        return Request.CreateResponse(HttpStatusCode.BadRequest,"Erro ao criar morada.\n"+resposta);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "O Json da morada não é o modelo usado");
            }
        }
        private static string NewAdress(Morada morada)
        {
            DateTime localDate = DateTime.Now;
            String hora = localDate.ToString("hh:mm:ss", CultureInfo.InvariantCulture);
            String datasql = localDate.ToString("yyyyMMdd");
            //String dataano = localDate.ToString("yyyy");
            String szadrstamp = Guid.NewGuid().ToString().Substring(0, 24);
            SqlConnection conn = new SqlConnection(StrConn);

            
            string saveStaff = "INSERT into szadrs (szadrsstamp,szadrsdesc,morada,local, codpost,codpais, pais, no, estab, ousrinis, ousrdata,ousrhora,usrinis, usrdata,usrhora,nome, origem)  " +
                            " VALUES ('" + szadrstamp + "','" + morada.descricao.Replace("'","") + "','" + morada.morada.Replace("'", "") + "','" + morada.localidade.Replace("'", "") + "','" + morada.codpostal.Replace("'", "") + "','" + morada.codpais.Replace("'", "") + "','" + morada.descpais.Replace("'", "") + "'," + morada.no + "," + morada.estab + ", 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "', 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "','" + morada.nome.Trim().Replace("'", "") + "','CL')";


            conn.Open();
            try
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = saveStaff;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
                return "true";
            }
            catch (Exception ex)
            {
                //return false;
                return ex.Message;
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
        private static bool CheckAddress(string descricao)
        {
            

            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                string query = "Select szadrsdesc as descricao, " +
                "szadrs.morada, szadrs.local, " +
                "cl.nome,cl.no,cl.estab, " +
                "szadrs.codpost, " +
                "szadrs.codpais " +
                ",szadrs.pais " +
                 "from SZADRS " +
                 "inner join cl (nolock) on cl.no=szadrs.no and cl.estab=szadrs.estab and szadrs.origem='CL' and cl.inactivo=0 " +
                 "where szadrs.szadrsdesc = '" + descricao + "'";


                SqlCommand uquery = new SqlCommand(query, conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                if (vrmTable.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            
                

            }
            catch (Exception ex)
            {
                return true;
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
        private static List<Morada> GetAddreses(int no, int estab)
        {
            List<Morada> Adresses = new List<Morada>();

            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                string query = "Select szadrsdesc as descricao, " +
                "szadrs.morada, szadrs.local, " +
                "cl.nome,cl.no,cl.estab, "+
                "szadrs.codpost, " +
                "szadrs.codpais " +
                ",szadrs.pais " +
                 "from SZADRS "+
                 "inner join cl (nolock) on cl.no=szadrs.no and cl.estab=szadrs.estab and szadrs.origem='CL' and cl.inactivo=0 " +
                 "where szadrs.origem = 'CL' and szadrs.no = " + no.ToString().Replace(",",".") + " and szadrs.estab = " + estab.ToString().Replace(",", ".") + " and szadrs.inactivo = 0 order by szadrsdesc";


                SqlCommand uquery = new SqlCommand(query, conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();

                for (int i = 0; i < vrmTable.Rows.Count; i++)
                {
                    Morada temp = new Morada();
                    temp.no = no;
                    temp.estab = estab;
                    temp.nome= vrmTable.Rows[i]["nome"].ToString();
                    temp.codpais= vrmTable.Rows[i]["codpais"].ToString();
                    temp.codpostal = vrmTable.Rows[i]["codpost"].ToString();
                    temp.localidade = vrmTable.Rows[i]["local"].ToString();
                    temp.morada = vrmTable.Rows[i]["morada"].ToString();
                    temp.descpais = vrmTable.Rows[i]["pais"].ToString();
                    temp.descricao = vrmTable.Rows[i]["descricao"].ToString();

                    Adresses.Add(temp);

                }

                return Adresses;

            }
            catch (Exception ex)
            {
                return Adresses;
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