using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Configuration;
using bell_Electronics.Models;

namespace bell_Electronics.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        string db_con = ConfigurationManager.ConnectionStrings["LTTE"].ConnectionString;

        [HttpGet]
        public ActionResult Index()
        {
            CartQtyGet();
            List<tbl_Category> cateObj = new List<tbl_Category>();
            using (SqlConnection con = new SqlConnection(db_con))
            {
                SqlCommand cmd = new SqlCommand("sp_CategoryFetch", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cateObj.Add(new tbl_Category
                    {
                        CategoryId = Convert.ToInt32(reader["CategoryId"].ToString()),
                        CategoryName = reader["CategoryName"].ToString(),
                        imageURL = reader["imageURL"].ToString(),
                        Cate_Status = Convert.ToInt32(reader["Cate_Status"].ToString()),
                    });
                }
                con.Close();
            }

            if (Session["id"] == null)
            {
                Session["log"] = "Login";
                Session["sign"] = "New User";
            }
            else if (Session["id"] != null)
            {
                Session["log"] = "Logout";
                Session["sign"] = "User Details";
                
            }
            return View(cateObj);
        }

        [HttpGet]
        public ActionResult ProductsList(int id)
        {
            List<Products> prodlist = new List<Products>();
            using (SqlConnection con = new SqlConnection(db_con))
            {
                SqlCommand cmd = new SqlCommand("sp_fetchProdList", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryId", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    prodlist.Add(new Products
                    {
                        ProductsID = Convert.ToInt32(reader["ProductsID"].ToString()),
                        ProductName = reader["ProductsName"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = Convert.ToInt32(reader["Price"].ToString()),
                        PrdimageUrl = reader["PrdimageUrl"].ToString(),
                        Prod_Status = Convert.ToInt32(reader["Prod_Status"].ToString()),
                    });
                }
                con.Close();
            }
            return View(prodlist);
        }

        [HttpGet]
        public ActionResult ProductDetails(int id)
        {
            try
            {
                Products prdObj = new Products();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_ProductsDetail", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductsID", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        prdObj = new Products
                        {
                            ProductsID = Convert.ToInt32(reader["ProductsID"].ToString()),
                            ProductName = reader["ProductsName"].ToString(),
                            Description = reader["Description"].ToString(),
                            CategoryId = Convert.ToInt32(reader["CategoryId"].ToString()),
                            Price = Convert.ToInt32(reader["Price"].ToString()),
                            PrdimageUrl = reader["PrdimageUrl"].ToString(),
                            Prod_Status = Convert.ToInt32(reader["Prod_Status"].ToString())
                        };
                    }
                    con.Close();
                }
                return View(prdObj);
            }
            catch (Exception)
            {
                return View();
            }

        }
        [HttpGet]
        public ActionResult ProductsCart(int id)
        {
            try
            {
                Products cateObjd1 = new Products();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_ProductsDetail", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductsID", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cateObjd1 = new Products
                        {
                            ProductsID = Convert.ToInt32(reader["ProductsID"].ToString()),
                            ProductName = reader["ProductsName"].ToString(),
                            Description = reader["Description"].ToString(),
                            CategoryId = Convert.ToInt32(reader["CategoryId"].ToString()),
                            Price = Convert.ToInt32(reader["Price"].ToString()),
                            PrdimageUrl = reader["PrdimageUrl"].ToString(),
                            Prod_Status = Convert.ToInt32(reader["Prod_Status"].ToString())
                        };
                    }
                    con.Close();
                }
                return View(cateObjd1);
            }
            catch (Exception)
            {
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductsCart(Products prodObj, int txtQty)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "UserPanel");
            }
            else
            {
                try
                {
                    string path = prodObj.PrdimageUrl;
                    if (path.Equals("-1"))
                    {
                        ViewBag.error = "Image could not be uploaded....";
                    }
                    else
                    {
                        var am = (Convert.ToInt32(txtQty) * Convert.ToInt32(prodObj.Price)).ToString();
                        using (SqlConnection con = new SqlConnection(db_con))
                        {
                            SqlCommand cmd = new SqlCommand("sp_Cart_CURD", con);
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Action", "INSERT");
                            cmd.Parameters.AddWithValue("@ProductsID", prodObj.ProductsID);
                            cmd.Parameters.AddWithValue("@Quantity", txtQty.ToString());
                            cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    TempData["success"] = "Product Add to Cart Successfully";
                    return RedirectToAction("ProdCartList");
                }
                catch
                {
                    return View();
                }
            }
        }

        public ActionResult ProdCartList()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "UserPanel");
            }
            else
            {
                int Amount = 0;
                List<tbl_Cart> cartlist = new List<tbl_Cart>();
                using (SqlConnection conn = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_Cart_CURD", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        cartlist.Add(new tbl_Cart
                        {
                            ProductsID = Convert.ToInt32(read["ProductsID"].ToString()),
                            ProductName = read["ProductsName"].ToString(),
                            Quantity = Convert.ToInt32(read["Quantity"].ToString()),
                            Price = Convert.ToInt32(read["Price"].ToString()),
                            UserName = Session["id"].ToString(),
                            Cartimg = read["PrdimageUrl"].ToString(),
                            total = Convert.ToInt32(read["Price"]) * Convert.ToInt32(read["Quantity"].ToString())
                        });
                        Amount += Convert.ToInt32(read["Price"]) * Convert.ToInt32(read["Quantity"].ToString());
                    }
                    Session["Total"] = Amount;
                    Session["crdt"] = cartlist;
                    conn.Close();
                }
                CartQtyGet();
                return View(cartlist);
            }

        }

        public ActionResult CartUpdate(int id)
        {
            //CartQtyGet();
            tbl_Cart crtlst = new tbl_Cart();
            using (SqlConnection con = new SqlConnection(db_con))
            {
                SqlCommand cmd = new SqlCommand("sp_Cart_CURD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "FETCH");
                con.Open();
                cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                cmd.Parameters.AddWithValue("@ProductsID", id);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    crtlst = new tbl_Cart
                    {
                        ProductsID = Convert.ToInt32(reader["ProductsID"].ToString()),
                        ProductName = reader["ProductsName"].ToString(),
                        Quantity = Convert.ToInt32(reader["Quantity"].ToString()),
                        Price = Convert.ToInt32(reader["Price"].ToString()),
                        Cartimg = reader["PrdimageUrl"].ToString()
                    };
                }
                con.Close();
            }
            return View(crtlst);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CartUpdate(tbl_Cart crt, int txtQty)
        {
            using (SqlConnection con = new SqlConnection(db_con))
            {
                SqlCommand cmd = new SqlCommand("sp_Cart_CURD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                con.Open();
                cmd.Parameters.AddWithValue("@Quantity", txtQty);
                cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                cmd.Parameters.AddWithValue("@ProductsID", crt.ProductsID);
                cmd.ExecuteNonQuery();
                con.Close();
                CartQtyGet();
                return RedirectToAction("ProdCartList");
            }
        }

        public ActionResult CartDelete(int id)
        {
            using (SqlConnection con = new SqlConnection(db_con))
            {
                SqlCommand cmd = new SqlCommand("sp_Cart_CURD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                con.Open();
                cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                cmd.Parameters.AddWithValue("@ProductsID", id);
                cmd.ExecuteNonQuery();
                con.Close();
                CartQtyGet();
                return RedirectToAction("ProdCartList");
            }
        }


        public ActionResult Payment()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login", "UserPanel");
            }
            else
            {
                if (Convert.ToInt32(Session["crtqty"]) != 0)
                {
                    int Amount = 0;
                    List<tbl_Cart> cartlist = new List<tbl_Cart>();
                    using (SqlConnection conn = new SqlConnection(db_con))
                    {
                        SqlCommand cmd = new SqlCommand("sp_Cart_CURD", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        cmd.Parameters.AddWithValue("@Action", "SELECT");
                        cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                        SqlDataReader read = cmd.ExecuteReader();
                        while (read.Read())
                        {
                            cartlist.Add(new tbl_Cart
                            {
                                ProductsID = Convert.ToInt32(read["ProductsID"].ToString()),
                                ProductName = read["ProductsName"].ToString(),
                                Quantity = Convert.ToInt32(read["Quantity"].ToString()),
                                Price = Convert.ToInt32(read["Price"].ToString()),
                                UserName = Session["id"].ToString(),
                                Cartimg = read["PrdimageUrl"].ToString(),
                                total = Convert.ToInt32(read["Price"]) * Convert.ToInt32(read["Quantity"].ToString())
                            });
                            Amount += Convert.ToInt32(read["Price"]) * Convert.ToInt32(read["Quantity"].ToString());
                        }
                        Session["Total"] = Amount;
                        ViewBag.cart = cartlist;
                        TempData["cartfetch"] = cartlist;
                        conn.Close();
                    }
                    CartQtyGet();
                    return View();
                }
                else
                {
                    return RedirectToAction("AllProductsView", "Customer");
                }
               
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(Payment paymt, string modpay)
        {
            DataTable dt;
            string holder,Add;

            Random nr = new Random();
            int read = nr.Next(0, 10000);
            if (modpay == "card")
            {
                holder = paymt.name;
                Add = paymt.CardAddress;
            }
            else
            {
                holder = paymt.Name1;
                Add = paymt.Address;
            }
            using (SqlConnection con = new SqlConnection(db_con))
            {
                SqlCommand cmd = new SqlCommand("sp_Cart_CURD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@Action", "SELECT");
                cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                con.Close();
            }
            // payment save
            using (SqlConnection con = new SqlConnection(db_con))
            {
                SqlCommand cmd = new SqlCommand("sp_Save_Payment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", holder.ToString());
                cmd.Parameters.AddWithValue("@CardNo", paymt.CardNo);
                cmd.Parameters.AddWithValue("@ExpriyDate", paymt.ExpDate);
                cmd.Parameters.AddWithValue("@CVVno", paymt.CVVno);
                cmd.Parameters.AddWithValue("@Address", Add.ToString());
                cmd.Parameters.AddWithValue("@PaymentMode", modpay);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            // order stored
            using (SqlConnection con = new SqlConnection(db_con))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SqlCommand cmd = new SqlCommand("sp_Order_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    Random r = new Random();
                    string random = r.Next(1, 1000000).ToString();
                    cmd.Parameters.AddWithValue("@OrderNo", random.ToString());
                    cmd.Parameters.AddWithValue("@ProductsID", dt.Rows[i][1].ToString());
                    cmd.Parameters.AddWithValue("@Quantity", dt.Rows[i][2].ToString());
                    cmd.Parameters.AddWithValue("@UserName", dt.Rows[i][3].ToString());
                    cmd.Parameters.AddWithValue("@PaymentID", read.ToString());
                    cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Status", "Pending");

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            // after order delete the card item
            using (SqlConnection con = new SqlConnection(db_con))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SqlCommand cmd = new SqlCommand("sp_Cart_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@ProductsID", dt.Rows[i][1].ToString());
                    cmd.Parameters.AddWithValue("@UserName", dt.Rows[i][3].ToString());
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            Session["pyid"] = read;
            CartQtyGet();
            return RedirectToAction("Invoice");
        }


        public ActionResult Invoice()
        {
            List<Invoice> invoils = new List<Invoice>();
            int Amount = 0;
            using (SqlConnection conn = new SqlConnection(db_con))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_Invoice", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Action", "INVOICBYID");
                    cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                    cmd.Parameters.AddWithValue("@PaymentID", Session["pyid"]);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        invoils.Add(new Invoice
                        {
                            OrderNo = read["OrderNo"].ToString(),
                            ProductsName = read["ProductsName"].ToString(),
                            Quantity = Convert.ToInt32(read["Quantity"].ToString()),
                            Price = Convert.ToInt32(read["Price"].ToString()),
                            TotalPrice = Convert.ToInt32(read["TotalPrice"].ToString()),
                            OrderDate = read["OrderDate"].ToString(),
                            Status = read["Status"].ToString(),

                        });
                        Amount += Convert.ToInt32(read["TotalPrice"]);
                    }
                }
                catch (Exception)
                {

                }
                Session["Amount"] = Amount;
                Session["pdfdt"] = invoils;
                conn.Close();
            }
            return View(invoils);
        }

        public ActionResult Pdf(Invoice inv)
        {
            DataTable GetOrderDetails()
            {
                double grandTotal = 0;
                SqlConnection conn = new SqlConnection(db_con);
                SqlCommand cmd = new SqlCommand("sp_Invoice", conn);
                cmd.Parameters.AddWithValue("@Action", "INVOICBYID");
                cmd.Parameters.AddWithValue("@PaymentID", Session["pyid"]);
                cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow drow in dt.Rows)
                    {
                        grandTotal += Convert.ToDouble(drow["TotalPrice"]);
                    }
                }
                DataRow dr = dt.NewRow();
                dr["TotalPrice"] = grandTotal;
                dt.Rows.Add(dr);
                return dt;
            }
            try
            {
                Random r = new Random();
                int random = r.Next();
                string downloadPath = @"D:\BellElec_order_invoice_" + random + ".pdf";
                DataTable dtbl = GetOrderDetails();
                ExportToPdf(dtbl, downloadPath, "Order Invoice");

                WebClient clint = new WebClient();
                byte[] buffer = clint.DownloadData(downloadPath);
                if (buffer != null)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", buffer.Length.ToString());
                    Response.BinaryWrite(buffer);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Text = "Error Message:- " + ex.Message.ToString();
            }
            CartQtyGet();
            return RedirectToAction("Index");
        }


        public ActionResult AllProductsView()
        {
            CartQtyGet();
            List<Products> prodAlllist = new List<Products>();
            using (SqlConnection con = new SqlConnection(db_con))
            {
                SqlCommand cmd = new SqlCommand("sp_FetchProducts", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    prodAlllist.Add(new Products
                    {
                        ProductsID = Convert.ToInt32(reader["ProductsID"].ToString()),
                        ProductName = reader["ProductsName"].ToString(),
                        Description = reader["Description"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        Price = Convert.ToInt32(reader["Price"].ToString()),
                        PrdimageUrl = reader["PrdimageUrl"].ToString(),
                        Prod_Status = Convert.ToInt32(reader["Prod_Status"].ToString()),
                    });
                }
                con.Close();
            }
            return View(prodAlllist);
        }


        public void CartQtyGet()
        {
            if (Session["id"] != null)
            {
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    //int a;
                    SqlCommand cmd = new SqlCommand("sp_Cart_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    int a = dt.Rows.Count;
                    int qty = 0;
                    for (int i = 0; i < a; i++)
                    {
                        qty += Convert.ToInt32(dt.Rows[i][2].ToString());
                    }
                    con.Close();
                    Session["crtqty"] = qty;
                }
            }
            else
            {
                Session["crtqty"] = 0;
            }
        }
        private void ExportToPdf(DataTable dtblTable, String strPdfPath, String strHeader)
        {

            FileStream fs = new FileStream(strPdfPath, FileMode.Create, FileAccess.Write, FileShare.None);
            Document document = new Document();
            document.SetPageSize(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            //Report Header
            BaseFont bfntHead = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font fntHead = new Font(bfntHead, 16, 1, Color.GRAY);
            Paragraph prgHeading = new Paragraph();
            prgHeading.Alignment = Element.ALIGN_CENTER;
            prgHeading.Add(new Chunk(strHeader.ToUpper(), fntHead));
            document.Add(prgHeading);

            //Author
            Paragraph prgAuthor = new Paragraph();
            BaseFont btnAuthor = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font fntAuthor = new Font(btnAuthor, 8, 2, Color.GRAY);
            prgAuthor.Alignment = Element.ALIGN_RIGHT;
            prgAuthor.Add(new Chunk("Order From : BELL ELECTRONICS", fntAuthor));
            prgAuthor.Add(new Chunk("\nOrder Date : " + dtblTable.Rows[0]["OrderDate"].ToString(), fntAuthor));
            document.Add(prgAuthor);

            //Add a line seperation
            Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, Color.BLACK, Element.ALIGN_LEFT, 1)));
            document.Add(p);

            //Add line break
            document.Add(new Chunk("\n", fntHead));

            //Write the table
            PdfPTable table = new PdfPTable(dtblTable.Columns.Count - 2);
            //Table header
            BaseFont btnColumnHeader = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Font fntColumnHeader = new Font(btnColumnHeader, 9, 1, Color.WHITE);
            for (int i = 0; i < dtblTable.Columns.Count - 2; i++)
            {
                PdfPCell cell = new PdfPCell();
                cell.BackgroundColor = Color.GRAY;
                cell.AddElement(new Chunk(dtblTable.Columns[i].ColumnName.ToUpper(), fntColumnHeader));
                table.AddCell(cell);
            }
            //table Data
            Font fntColumnData = new Font(btnColumnHeader, 8, 1, Color.BLACK);
            for (int i = 0; i < dtblTable.Rows.Count; i++)
            {
                for (int j = 0; j < dtblTable.Columns.Count - 2; j++)
                {
                    PdfPCell cell = new PdfPCell();
                    cell.AddElement(new Chunk(dtblTable.Rows[i][j].ToString(), fntColumnData));
                    table.AddCell(cell);
                }
            }
            document.Add(table);
            document.Close();
            writer.Close();
            fs.Close();
        }
    }
}