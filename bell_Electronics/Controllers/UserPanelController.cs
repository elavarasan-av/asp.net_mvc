using bell_Electronics.Models;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;


namespace bell_Electronics.Controllers
{
    public class UserPanelController : Controller
    {
        string db_con = ConfigurationManager.ConnectionStrings["LTTE"].ConnectionString;
        DbConext db = new DbConext();
        // GET: UserPanel
        [HttpGet]
        public ActionResult Login(string UserName)
        {
            if (Session["id"] == null)
            {
                var row = db.GetUser().Where(model => model.UserName == UserName).FirstOrDefault();
            }
            if (Session["id"] != null)
            {
                Session["id"] = null;
                Session["name"] = null;
                return RedirectToAction("Index", "Customer");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(UsersModel um)
        {

            if (Session["id"] == null)
            {
                var data = db.GetUser().Where(model => model.UserName == um.UserName && model.Password == um.Password).FirstOrDefault();
                if (data != null)
                {
                    Session["id"] = um.UserName;
                    Session["usname"] = um.Name;
                    return RedirectToAction("Index", "Customer");
                }
                else if (um.UserName == "Admin" && um.Password == "LTTE")
                {
                    Session["id"] = um.UserName;
                    Session["usname"] = um.Name;
                    return RedirectToAction("Index", "Category");
                }
                else
                {
                    ViewBag.Showmsg = "Invalid Email id or Password";
                    ModelState.Clear();
                    return View();
                }
            }
            else if (Session["id"] != null)
            {
                Session["id"] = null;
                Session["name"] = null;
            }
            return View();
        }

        [HttpGet]
        public ActionResult UserRegistraion()
        {
            if (Session["id"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("UserDetails");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRegistraion(UsersModel usm)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_User_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@Name", usm.Name);
                    cmd.Parameters.AddWithValue("@MobileNo", usm.MobileNo);
                    cmd.Parameters.AddWithValue("@Email", usm.Email);
                    cmd.Parameters.AddWithValue("@UserName", usm.UserName);
                    cmd.Parameters.AddWithValue("@Password", usm.Password);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                TempData["success"] = "User Data Registration Successfully!!!";
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                return View();
            }
        }


        public ActionResult UserDetails()
        {
            try
            {
                if (Session["id"] != null)
                {
                    UsersModel usmd = new UsersModel();
                    using (SqlConnection con = new SqlConnection(db_con))
                    {
                        SqlCommand cmd = new SqlCommand("sp_User_CURD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        cmd.Parameters.AddWithValue("@Action", "FETCH");
                        cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            usmd = new UsersModel
                            {
                                id = Convert.ToInt32(reader["id"]),
                                Name = reader["Name"].ToString(),
                                MobileNo = reader["MobileNo"].ToString(),
                                Email = reader["Email"].ToString(),
                                UserName = reader["UserName"].ToString(),
                                Password = reader["Password"].ToString(),
                            };
                        }
                        con.Close();
                        return PartialView(usmd);
                    }
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            catch (Exception)
            {
                return View();
            }

        }

        public ActionResult UserEdit()
        {
            if (Session["id"] != null)
            {
                UsersModel usmd = new UsersModel();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_User_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Action", "FETCH");
                    cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        usmd = new UsersModel
                        {
                            id = Convert.ToInt32(reader["id"]),
                            Name = reader["Name"].ToString(),
                            MobileNo = reader["MobileNo"].ToString(),
                            Email = reader["Email"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            Password = reader["Password"].ToString(),
                        };
                    }
                    con.Close();
                    return PartialView(usmd);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        public ActionResult UserEdit(UsersModel usmod)
        {
            if (Session["id"] != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(db_con))
                    {
                        SqlCommand cmd = new SqlCommand("sp_User_CURD", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "UPDATE");
                        cmd.Parameters.AddWithValue("@UserName", Session["id"]);
                        cmd.Parameters.AddWithValue("@Name", usmod.Name);
                        cmd.Parameters.AddWithValue("@MobileNo", usmod.MobileNo);
                        cmd.Parameters.AddWithValue("@Email", usmod.Email);
                        cmd.Parameters.AddWithValue("@Password", usmod.Password);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    TempData["userSuss"] = usmod.Name + " User Data Updated Successfully!!!!";
                    return RedirectToAction("UserDetails");
                }
                catch (Exception ex)
                {
                    ViewBag.error = "Error" + ex;
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult OrderDetails()
        {
            if (Session["id"] != null)
            {
                List<Order> ordlst = new List<Order>();
                using (SqlConnection con = new SqlConnection(db_con))
                {

                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_Order_CURD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    cmd.Parameters.AddWithValue("@UserName", Session["id"]);
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
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}