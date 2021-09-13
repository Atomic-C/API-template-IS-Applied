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
using Newtonsoft.Json;
using PHCWS.objects;
using Swashbuckle.Swagger.Annotations;
using WebApiAuthenticate.Filters;

namespace PHCWS.Controllers
{
   
    [BasicAuthentication]
    [EnableCorsAttribute("*", "*", "*")]
    public class OrdersController : ApiController
    {

        private static string StrConn = @"Data Source=VMAZ;Initial Catalog=GFSC;User ID=PHCAPI;password=7h(8x*V,a3HcDGuF";

        public OrdersController()
        {
        }
        /// <summary>
        /// Get order data from a specific order.
        /// </summary>
        /// <param name="orderid">OrderID</param>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EncomendaPHC))]
        [HttpGet]
        // GET: Order
        public HttpResponseMessage GetEncomenda(string orderid)
        {
            EncomendaPHC bo = new EncomendaPHC();
            bo = GetOrder(orderid);
            if ((string.IsNullOrEmpty(bo.orderId))==false)
                return Request.CreateResponse(HttpStatusCode.OK, bo, Configuration.Formatters.JsonFormatter);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        // PUT: Orders/putEncomenda
        /// <summary>
        /// Update order status from a specific order.
        /// </summary>
        /// /// <param name="orderid">OrderID</param>
        [HttpPut]
        public HttpResponseMessage putEncomenda(string orderid,string status)
        {
            string bostamp = "";

            bostamp = GetOrderStamp(orderid);
            if ((string.IsNullOrEmpty(bostamp)) == false)
            {
                string teste = "";
                teste = UpdtOrder(bostamp, status);
                if (teste == "Ok")
                    return Request.CreateResponse(HttpStatusCode.OK, "Order " + orderid + " atualizada com sucesso");
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, teste);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Order " + orderid + " não encontrada");
            }
        }
        /// <summary>
        /// Creates new order in PHC
        /// </summary>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Encomenda>))]
        // POST: Orders/postEncomenda
        [HttpPost]
        public HttpResponseMessage postEncomenda([FromBody] List<Encomenda> LstEnc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (LstEnc.Count == 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NoContent, "Lista de encomendas vazia");
                    }
                    if (LstEnc is null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Json Encomendas mal formatado");
                    }
                    if (!(LstEnc.GetType() == typeof(List<Encomenda>)))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "O Json da encomenda não é o modelo predefinido");
                    }
                    string bostamp = "";
                    foreach (Encomenda encomenda in LstEnc)
                    {
                       
                        bostamp = "";
                        if (OrderExiste(encomenda) == false)
                        {
                            bostamp = criardossierencomenda(encomenda);
                            if (!(bostamp == ""))
                            {
                                string respostalinha = "";
                                foreach (lines linha in encomenda.lines)
                                {

                                    respostalinha = criarLinha(bostamp, linha);
                                    if (!(respostalinha == "ok"))
                                    {
                                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, respostalinha);
                                    }
                                }
                                if (encomenda.shippingFees != 0)
                                {
                                    lines portes = new lines();
                                    portes.name = "PORTES DE ENVIO";
                                    portes.reference = "PORTES";
                                    portes.quantity = 1;
                                    portes.subTotal = encomenda.shippingFees;
                                    portes.percenttax = 23;
                                    portes.subTotalTax = encomenda.shippingFeesWithTax;
                                    respostalinha = criarLinha(bostamp, portes);
                                    if (!(respostalinha == "ok"))
                                    {
                                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, respostalinha);
                                    }
                                }
                            }
                        }
                    }
                    //return new HttpResponseMessage(HttpStatusCode.OK);
                    
                    return Request.CreateResponse(HttpStatusCode.Created, "Order criada"); 


                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }
        private static string GetOrderStamp(string orderid)
        {
            EncomendaPHC order = new EncomendaPHC();

            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                string query = "Select bo.bostamp,bo.ncont as NIF,bo.maquina as status,  " +
                "cast(round(bo.ETOTALDEB, 2) as decimal(10, 2)) as subTotal , cast(round(bo.ETOTAL, 2) as decimal(10, 2)) as Total,  " +
                "bo.obranome as orderid, bo.codpost, bo.local, bo.morada " +
                ",bo.no, bo.estab, bo.obrano " +
                ",bo3.codpais, bo2.email, bo.nome, convert(date, bo.dataobra, 104) as dataobra " +
                ",bo2.CARGA " +
                ",isnull(szadrs.codpais, '') as shippingCountry,isnull(szadrs.codpost, '') as shippingZipcode,isnull(szadrs.local, '') as shippingCity,isnull(szadrs.morada, '') as shippingAddress " +
                 ",isnull((select cast(round(sum(bi.ettdeb), 2) as decimal(10, 2)) from bi (nolock) where bi.bostamp = bo.bostamp and bi.ref= 'PORTES'),0) as Portes " +
                 ",isnull((select cast(round(sum(bi.ettdeb * (1 + (bi.iva / 100))), 2) as decimal(10, 2)) from bi (nolock) where bi.bostamp = bo.bostamp and bi.ref= 'PORTES'),0) as PortesIVA " +
                 " from bo (nolock) inner join bo2 (nolock) on bo2.bo2stamp = bo.bostamp inner join bo3 (nolock) on bo3.bo3stamp = bo.bostamp " +
                 "left outer join szadrs(nolock) on szadrs.szadrsdesc = bo2.carga " +
                 " where bo.ndos=33 and bo.obranome='" + orderid.ToString() + "'";


                SqlCommand uquery = new SqlCommand(query, conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                int tempno = 0, tempestab = 0;
                DateTime tempdata;
                float tempportes = 0.00F, tempportesiva = 0.00F, temptotal = 0.00F, tempsubtotal = 0.00F;
                string bostamp = "";
                List<lines> BI = new List<lines>();
                if (vrmTable.Rows.Count > 0)
                {
                    for (int i = 0; i < vrmTable.Rows.Count; i++)
                    {
                        order.customerAddress = vrmTable.Rows[i]["morada"].ToString();
                        order.customerCity = vrmTable.Rows[i]["local"].ToString();
                        order.customerCountry = vrmTable.Rows[i]["codpais"].ToString();
                        order.customerEmail = vrmTable.Rows[i]["email"].ToString();
                        order.customerName = vrmTable.Rows[i]["nome"].ToString();
                        order.customerZipcode = vrmTable.Rows[i]["codpost"].ToString();
                        int.TryParse(vrmTable.Rows[i]["estab"].ToString(), out tempestab);
                        order.customer_estab_phc = tempestab;
                        int.TryParse(vrmTable.Rows[i]["no"].ToString(), out tempno);
                        order.customer_number_phc = tempno;
                        DateTime.TryParse(vrmTable.Rows[i]["dataobra"].ToString(), out tempdata);
                        order.dateCreated = tempdata;
                        order.status = vrmTable.Rows[i]["status"].ToString();
                        order.nif = vrmTable.Rows[i]["NIF"].ToString();
                        order.orderId = vrmTable.Rows[i]["obrano"].ToString();
                        float.TryParse(vrmTable.Rows[i]["portes"].ToString(), out tempportes);
                        order.shippingFees = tempportes;
                        float.TryParse(vrmTable.Rows[i]["PortesIVA"].ToString(), out tempportesiva);
                        order.shippingFeesWithTax = tempportesiva;
                        order.shipping_address_desc = vrmTable.Rows[i]["carga"].ToString();
                        float.TryParse(vrmTable.Rows[i]["subTotal"].ToString(), out tempsubtotal);
                        order.subTotal = tempsubtotal;
                        float.TryParse(vrmTable.Rows[i]["Total"].ToString(), out temptotal);
                        order.total = temptotal;
                        order.shippingAddress = vrmTable.Rows[i]["shippingAddress"].ToString();
                        order.shippingCity = vrmTable.Rows[i]["shippingCity"].ToString();
                        order.shippingCountry = vrmTable.Rows[i]["shippingCountry"].ToString();
                        order.shippingZipcode = vrmTable.Rows[i]["shippingZipcode"].ToString();
                        bostamp = vrmTable.Rows[i]["bostamp"].ToString();

                        BI = GetOrderLines(bostamp);
                        order.lines = BI;
                        return bostamp;
                    }
                }

                return "";

            }
            catch (Exception ex)
            {
                order.customerName = ex.Message;
                return "";
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
        private static EncomendaPHC GetOrder(string orderid)
        {
            EncomendaPHC order = new EncomendaPHC();

            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                string query = "Select bo.bostamp,bo.ncont as NIF,bo.maquina as status,  "+
                "cast(round(bo.ETOTALDEB, 2) as decimal(10, 2)) as subTotal , cast(round(bo.ETOTAL, 2) as decimal(10, 2)) as Total,  "+
                "bo.obranome as orderid, bo.codpost, bo.local, bo.morada "+
                ",bo.no, bo.estab, bo.obrano "+
                ",bo3.codpais, bo2.email, bo.nome, convert(date, bo.dataobra, 104) as dataobra "+
                ",bo2.CARGA "+
                ",isnull(szadrs.codpais, '') as shippingCountry,isnull(szadrs.codpost, '') as shippingZipcode,isnull(szadrs.local, '') as shippingCity,isnull(szadrs.morada, '') as shippingAddress " +
                 ",isnull((select cast(round(sum(bi.ettdeb), 2) as decimal(10, 2)) from bi (nolock) where bi.bostamp = bo.bostamp and bi.ref= 'PORTES'),0) as Portes " +
				 ",isnull((select cast(round(sum(bi.ettdeb * (1 + (bi.iva / 100))), 2) as decimal(10, 2)) from bi (nolock) where bi.bostamp = bo.bostamp and bi.ref= 'PORTES'),0) as PortesIVA " +
                 " from bo (nolock) inner join bo2 (nolock) on bo2.bo2stamp = bo.bostamp inner join bo3 (nolock) on bo3.bo3stamp = bo.bostamp " +
                 "left outer join szadrs(nolock) on szadrs.szadrsdesc = bo2.carga "+
                 " where bo.ndos=33 and bo.obranome='" +orderid.ToString()+"'";


                SqlCommand uquery = new SqlCommand(query, conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                int tempno=0, tempestab = 0;
                DateTime tempdata;
                float tempportes=0.00F, tempportesiva = 0.00F,temptotal=0.00F,tempsubtotal=0.00F;
                string bostamp = "";
                List<lines> BI = new List<lines>();
                if (vrmTable.Rows.Count > 0)
                {
                    for (int i = 0; i < vrmTable.Rows.Count; i++)
                    {
                        order.customerAddress = vrmTable.Rows[i]["morada"].ToString();
                        order.customerCity = vrmTable.Rows[i]["local"].ToString();
                        order.customerCountry = vrmTable.Rows[i]["codpais"].ToString();
                        order.customerEmail = vrmTable.Rows[i]["email"].ToString();
                        order.customerName = vrmTable.Rows[i]["nome"].ToString();
                        order.customerZipcode = vrmTable.Rows[i]["codpost"].ToString();
                        int.TryParse(vrmTable.Rows[i]["estab"].ToString(), out tempestab);
                        order.customer_estab_phc = tempestab;
                        int.TryParse(vrmTable.Rows[i]["no"].ToString(), out tempno);
                        order.customer_number_phc = tempno;
                        DateTime.TryParse(vrmTable.Rows[i]["dataobra"].ToString(), out tempdata);
                        order.dateCreated = tempdata;
                        order.status = vrmTable.Rows[i]["status"].ToString();
                        order.nif = vrmTable.Rows[i]["NIF"].ToString();
                        order.orderId = vrmTable.Rows[i]["obrano"].ToString(); 
                        float.TryParse(vrmTable.Rows[i]["portes"].ToString(), out tempportes);
                        order.shippingFees = tempportes;
                        float.TryParse(vrmTable.Rows[i]["PortesIVA"].ToString(), out tempportesiva);
                        order.shippingFeesWithTax = tempportesiva;
                        order.shipping_address_desc = vrmTable.Rows[i]["carga"].ToString();
                        float.TryParse(vrmTable.Rows[i]["subTotal"].ToString(), out tempsubtotal);
                        order.subTotal = tempsubtotal;
                        float.TryParse(vrmTable.Rows[i]["Total"].ToString(), out temptotal);
                        order.total = temptotal;
                        order.shippingAddress = vrmTable.Rows[i]["shippingAddress"].ToString();
                        order.shippingCity = vrmTable.Rows[i]["shippingCity"].ToString();
                        order.shippingCountry = vrmTable.Rows[i]["shippingCountry"].ToString();
                        order.shippingZipcode = vrmTable.Rows[i]["shippingZipcode"].ToString();
                        bostamp = vrmTable.Rows[i]["bostamp"].ToString();

                        BI = GetOrderLines(bostamp);
                        order.lines = BI;
                        return order;
                    }
                }

                return order;

            }
            catch (Exception ex)
            {
                order.customerName = ex.Message;
                return order;
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
        private static List<lines> GetOrderLines(string bostamp)
        {
            List<lines> BI = new List<lines>();

            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                string query = "Select bi.ref "+
                                ", bi.design "+
                                ",cast(bi.iva as decimal(10, 2)) as IVA "+
                                ",cast(bi.qtt as decimal(10, 0)) as qtt "+
                                ",cast(round(bi.ettdeb, 2) as decimal(10, 2)) as Total "+
                                ",cast(round(bi.ettdeb * (1 + (bi.iva / 100)), 2) as decimal(10, 2)) as SubTotal "+
                                ",bi.peso "+
                                "from bi(nolock) "+
                                "where bi.bostamp = '"+bostamp+"'"+
                                "order by lordem asc";


                SqlCommand uquery = new SqlCommand(query, conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
               
                float temptotal = 0.00F, tempsubtotal = 0.00F,temppeso=0.00F, tempIVA=0.00F, tempqtt=0.00F;
                for (int i = 0; i < vrmTable.Rows.Count; i++)
                {
                    lines templinha = new lines();
                    templinha.name = vrmTable.Rows[i]["ref"].ToString().Trim();
                    templinha.reference = vrmTable.Rows[i]["design"].ToString().Trim();
                    float.TryParse(vrmTable.Rows[i]["IVA"].ToString(), out tempIVA);
                    templinha.percenttax = tempIVA;
                    float.TryParse(vrmTable.Rows[i]["qtt"].ToString(), out tempqtt);
                    templinha.quantity = tempqtt;
                    float.TryParse(vrmTable.Rows[i]["total"].ToString(), out temptotal);
                    templinha.total = temptotal;
                    float.TryParse(vrmTable.Rows[i]["SubTotal"].ToString(), out tempsubtotal);
                    templinha.subTotal = tempsubtotal;
                    float.TryParse(vrmTable.Rows[i]["peso"].ToString(), out temppeso);
                    templinha.weight = temppeso;
                    BI.Add(templinha);
                }

                return BI;

            }
            catch (Exception ex)
            {
                return BI;
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
        private static string UpdtOrder(string bostamp, string status)
        {
            SqlConnection conn = new SqlConnection(StrConn);

            string saveStaff = "";
            saveStaff = "update bo set maquina='"+ status + "' where bo.bostamp='"+ bostamp + "'";

            conn.Open();
            try
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = saveStaff;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message+"\n"+saveStaff;
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
        private static string criarLinha(string bostamp, lines linha)
        {
            DateTime localDate = DateTime.Now;
            String hora = localDate.ToString("hh:mm:ss", CultureInfo.InvariantCulture);
            String datasql = localDate.ToString("yyyyMMdd");
            String dataano = localDate.ToString("yyyy");
            String bistamp = Guid.NewGuid().ToString().Substring(0, 24);


            SqlConnection conn = new SqlConnection(StrConn);

            int ivainc = 0;


       
            int stns = 0;
            int tabIva = 0;
            tabIva = getTabIVA(linha.percenttax);
            if (linha.reference == "PORTES")
            {
                stns = 1;
            }
            else
            {
                stns = 0;
            }

            string saveStaff = "";
            saveStaff = "INSERT into bi (ndos,nmdos,bostamp,bistamp,dataobra,rdata,ref,armazem,design,qtt,edebito,ETTDEB,ousrinis,ousrdata,ousrhora,usrinis,usrdata,usrhora, IVAINCL,stns, tabiva, iva, stipo,peso)  " +
               " VALUES (3,'Encomenda de Cliente Net','" + bostamp + "','" + bistamp + "','" + datasql + "','" + datasql + "','" + linha.reference + "' ,1,'" + linha.name + "'," + linha.quantity.ToString().Replace(',', '.') + "" +
               "," + (linha.subTotal / linha.quantity).ToString().Replace(',', '.') + "," + (linha.subTotal).ToString().Replace(',', '.') + ", 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "', 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "'" +
               "," + ivainc.ToString() + "," + stns.ToString() + "," + tabIva.ToString().Replace(',', '.') + "," + linha.percenttax.ToString().Replace(',', '.') + ",4,"+linha.weight.ToString().Replace(",",".")+");"+
               "update bo set bo.ETOTALDEB=isnull((Select sum(ettdeb) from bi where bostamp='" + bostamp + "'),0) where bostamp='" + bostamp + "'"; 

            conn.Open();
            try
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = saveStaff;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message+"\n"+saveStaff;
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
        private static bool OrderExiste(Encomenda enc)
        {

            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                SqlCommand uquery = new SqlCommand("Select * from bo where bo.ndos=33 and obranome='" + enc.orderId.ToString() + "'", conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                if (vrmTable.Rows.Count == 0)
                    return false;
                else
                    return true;


            }
            catch (Exception ex)
            {
                return false;
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
        private static int getTabIVA(float taxa)
        {
            int codigo = 0;

            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                SqlCommand uquery = new SqlCommand("Select codigo from taxasiva (nolock) where taxa=" + taxa.ToString().Replace(",", "."), conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                if (vrmTable.Rows.Count > 0)
                    codigo = Convert.ToInt32(vrmTable.Rows[0]["codigo"]);

                return codigo;

            }
            catch (Exception ex)
            {
                return 0;
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
      
      
      
        private static string criardossierencomenda(Encomenda enc)
        {

            String bostamp = Guid.NewGuid().ToString().Substring(0, 24);


            SqlConnection conn = new SqlConnection(StrConn);

            DateTime localDate = DateTime.Now;
            String hora = localDate.ToString("hh:mm:ss", CultureInfo.InvariantCulture);
            String datasql = localDate.ToString("yyyyMMdd");
            String dataano = enc.dateCreated.ToString("yyyy");

            int no = getmaxbono(enc) + 1;

           
           
            string saveStaff = "INSERT into bo (ndos,nmdos,bostamp,tabela1,obrano,dataobra,nome,etotaldeb,etotal,no,estab,boano,moeda,morada,local,codpost,ncont,origem,ousrinis,ousrdata,ousrhora,usrinis,usrdata,usrhora,OBRANOME,tpdesc,serie)  " +
               " VALUES (3,'Encomenda de Cliente Net','" + bostamp + "',''," + no + " ,'" + enc.dateCreated.ToString("yyyyMMdd") + "','" + enc.customerName + "'," + enc.subTotal.ToString().Replace(',', '.') + "," + enc.total.ToString().Replace(',', '.') + ","+enc.customer_number_phc.ToString().Replace(",",".")+","+enc.customer_estab_phc.ToString().Replace(",", ".") +"," + dataano + ",  'EURO','" + enc.customerAddress + "','" + enc.customerCity + "', '" + enc.customerZipcode + "'," +
                " '" + enc.nif + "','BO', 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "', 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "','" + enc.orderId + "','','A aguardar pagamento')" +
                "INSERT into bo2 (bo2stamp, ousrinis,ousrdata,ousrhora,usrinis,usrdata,usrhora, contacto ,morada ,local, codpost, telefone, email, tiposaft, idserie, descar, carga) " +
                " VALUES ('" + bostamp + "', 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "', 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "', '', '"+enc.shippingAddress+ "', '" + enc.shippingCity + "' , '" + enc.shippingZipcode + "', '', '" + enc.customerEmail + "', '--', 'BO','"+enc.shipping_address_desc+ "','AaZ do CAFÉ, LDA');" +
               "INSERT into bo3 (bo3stamp, ousrinis,ousrdata,ousrhora,usrinis,usrdata,usrhora, codpais, descpais) " +
                " VALUES ('" + bostamp + "', 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "', 'PHCAPI' , '" + datasql + "','" + hora.Trim() + "', '"+ enc.customerCountry + "', '"+ Getdescpais(enc.customerCountry) + "');";


            conn.Open();
            try
            {

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = saveStaff;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
                return bostamp;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);

                return ex.Message+"\n"+saveStaff;
                //return ex.Message;
              


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
        private static string Getdescpais(string pais)
        {
            String codpais = "";


            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                SqlCommand uquery = new SqlCommand("Select nomeabrv from paises where paises.nome='" + pais.Trim() + "'", conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                for (int i = 0; i < vrmTable.Rows.Count; i++)
                {
                    codpais = vrmTable.Rows[i]["nomeabrv"].ToString();
                }
                return codpais;

            }
            catch (Exception ex)
            {
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
        public static int getmaxbono(Encomenda enc)
        {
            int maxno = 0;


            SqlConnection conn = new SqlConnection(StrConn);

            SqlCommand cmd = new SqlCommand();

            try
            {

                DataTable vrmTable = new DataTable();

                conn.Open();
                SqlCommand uquery = new SqlCommand("Select isnull(max(bo.obrano),0) as no from bo (nolock) where bo.ndos=3 and boano="+enc.dateCreated.ToString("yyyy"), conn);

                SqlDataAdapter vrmAdapter = new SqlDataAdapter(uquery);
                vrmAdapter.Fill(vrmTable);
                conn.Close();
                for (int i = 0; i < vrmTable.Rows.Count; i++)
                {
                    maxno = Convert.ToInt32(vrmTable.Rows[i]["no"]);
                }
                return maxno;

            }
            catch (Exception ex)
            {

                return 0;
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
