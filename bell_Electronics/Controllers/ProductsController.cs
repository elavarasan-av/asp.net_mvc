using bell_Electronics.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace bell_Electronics.Controllers
{
    public class ProductsController : Controller
    {
        string db_con = ConfigurationManager.ConnectionStrings["LTTE"].ConnectionString;

        public ActionResult Index()
        {

            List<Products> prdFetc = new List<Products>();
            using (SqlConnection con = new SqlConnection(db_con))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_FetchProducts", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        prdFetc.Add(new Products
                        {
                            ProductsID = Convert.ToInt32(rdr["ProductsID"].ToString()),
                            ProductName = rdr["ProductsName"].ToString(),
                            Description = rdr["Description"].ToString(),
                            CategoryName = rdr["CategoryName"].ToString(),
                            Price = Convert.ToInt32(rdr["Price"].ToString()),
                            PrdimageUrl = rdr["PrdimageUrl"].ToString(),
                            Prod_Status = Convert.ToInt32(rdr["Prod_Status"].ToString())
                        });
                    }
                    con.Close();
                }
                catch (Exception)
                {

                }
            }
            return View(prdFetc);
        }


        public ActionResult Details(int id)
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


        public ActionResult Create()
        {
            try
            {
                List<tbl_Category> getcatde = new List<tbl_Category>();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_CategoryFetchAct", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        getcatde.Add(new tbl_Category
                        {
                            CategoryId = Convert.ToInt32(reader["CategoryId"].ToString()),
                            CategoryName = reader["CategoryName"].ToString(),
                        });
                    }
                    ViewBag.list = getcatde;
                    con.Close();
                }
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products prodObj, HttpPostedFileBase productimg)
        {
            try
            {
                string path = uploadimgfile(productimg);
                if (path.Equals("-1"))
                {
                    ViewBag.error = "Image could not be uploaded....";
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(db_con))
                    {
                        SqlCommand cmd = new SqlCommand("sp_AddProducts", con);
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductsName", prodObj.ProductName);
                        cmd.Parameters.AddWithValue("@Description", prodObj.Description);
                        cmd.Parameters.AddWithValue("@CategoryId", prodObj.CategoryId);
                        cmd.Parameters.AddWithValue("@Price", prodObj.Price);
                        cmd.Parameters.AddWithValue("@PrdimageUrl", path);
                        cmd.Parameters.AddWithValue("@Prod_Status", prodObj.Prod_Status);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Edit(int id)
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
        public ActionResult Edit(int id, Products updprod, HttpPostedFileBase productimg)
        {
            try
            {
                string path;
                if (productimg == null)
                {
                    path = updprod.PrdimageUrl;
                }
                else
                {
                    path = uploadimgfile(productimg);
                    string path1 = Path.Combine(Server.MapPath(updprod.PrdimageUrl));
                    FileInfo fi = new FileInfo(path1);
                    if (fi != null)
                    {
                        System.IO.File.Delete(path1);
                        fi.Delete();
                    }
                }
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateProd", con);
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductsID", id);
                    cmd.Parameters.AddWithValue("@ProductsName", updprod.ProductName);
                    cmd.Parameters.AddWithValue("@Description", updprod.Description);
                    cmd.Parameters.AddWithValue("@CategoryId", updprod.CategoryId);
                    cmd.Parameters.AddWithValue("@Price", updprod.Price);
                    cmd.Parameters.AddWithValue("@PrdimageUrl", path);
                    cmd.Parameters.AddWithValue("@Prod_Status", updprod.Prod_Status);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                TempData["success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Delete(int id)
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
        public ActionResult Delete(int id, Products prodDel)
        {
            try
            {
                string path1 = Path.Combine(Server.MapPath(prodDel.PrdimageUrl));
                FileInfo fi = new FileInfo(path1);
                if (fi != null)
                {
                    System.IO.File.Delete(path1);
                    fi.Delete();
                }
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteProds", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductsID", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult OrderList()
        {
            //if (Session["id"] != null)
            //{
                List<Order> ordlst = new List<Order>();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_Order_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "ADMINSELECT");
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ordlst.Add(new Order
                        {
                            OrderNo = reader["OrderNo"].ToString(),
                            ProductsId = Convert.ToInt32(reader["ProductsID"].ToString()),
                            Quantity = Convert.ToInt32(reader["Quantity"].ToString()),
                            UserName = reader["UserName"].ToString(),
                            Status = reader["Status"].ToString(),
                            PaymentId = reader["PaymentID"].ToString(),
                            OrderDate = reader["OrderDate"].ToString(),
                            ProductsName = reader["ProductsName"].ToString(),
                            PrdimageUrl = reader["PrdimageUrl"].ToString()
                        });
                    }
                    con.Close();
                    return View(ordlst);
                }
            //}
            //else
            //{
            //    return RedirectToAction("Login");
            //}
        }

        public ActionResult OrderUpdate(string orno)
        {
            try
            {
                Order ordObj = new Order();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_Order_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "FETCH");
                    cmd.Parameters.AddWithValue("@OrderNo", orno);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ordObj = new Order
                        {
                            OrderNo = reader["OrderNo"].ToString(),
                            ProductsId = Convert.ToInt32(reader["ProductsID"].ToString()),
                            Quantity = Convert.ToInt32(reader["Quantity"].ToString()),
                            UserName = reader["UserName"].ToString(),
                            Status = reader["Status"].ToString(),
                            PaymentId = reader["PaymentID"].ToString(),
                            OrderDate = reader["OrderDate"].ToString(),
                            ProductsName = reader["ProductsName"].ToString(),
                            PrdimageUrl = reader["PrdimageUrl"].ToString()
                        };
                    }
                    con.Close();
                }
                List<OrderStatus> ListStatus = new List<OrderStatus>()
            {
                new OrderStatus() {Id = 1, Status="Pending" },
                new OrderStatus() {Id = 2, Status="Dispatch" },
                new OrderStatus() {Id = 3, Status="Delivery" },
            };
                ViewBag.OrderStatus = ListStatus;
                return View(ordObj);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderUpdate(Order order, string Status)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_Order_CURD", con);
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action","UPDATE");
                    cmd.Parameters.AddWithValue("@UserName", order.UserName);
                    cmd.Parameters.AddWithValue("@ProductsID", order.ProductsId);
                    cmd.Parameters.AddWithValue("@OrderNo", order.OrderNo);
                    cmd.Parameters.AddWithValue("@Status", Status);                    
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                TempData["statusupdate"] = "Status Updated Successfully";
                return RedirectToAction("OrderList");
            }
            catch
            {
                return View();
            }
        }

        //public ActionResult OrderUpdate()
        //{

        //}

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Images/products"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Images/products/" + random + Path.GetFileName(file.FileName);
                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }
            return path;
        }

    }
}
