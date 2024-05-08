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
    public class CategoryController : Controller
    {
        string db_con = ConfigurationManager.ConnectionStrings["LTTE"].ConnectionString;

     
        public ActionResult Index()
        {
            try
            {
                if (Session["id"] != null)
                {
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
                    return View(cateObj);
                }
                else
                {
                    return RedirectToAction("Login", "UserPanel");
                }
            }
            catch
            {
                return View();
            }
        }

   
        public ActionResult Details(int id)
        {
            try
            {
                tbl_Category catedd = new tbl_Category();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_CategoryDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        catedd = new tbl_Category
                        {
                            CategoryId = Convert.ToInt32(reader["CategoryId"].ToString()),
                            CategoryName = reader["CategoryName"].ToString(),
                            imageURL = reader["imageURL"].ToString(),
                            Cate_Status = Convert.ToInt32(reader["Cate_Status"].ToString()),
                        };
                    }
                    con.Close();
                }
                return View(catedd);
            }
            catch (Exception)
            {
                return View();
            }        

        }

        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_Category cateObj, HttpPostedFileBase categoryimg)
        {
            try
            {
                string path = uploadimgfile(categoryimg);
                if (path.Equals("-1"))
                {
                    ViewBag.error = "Image could not be uploaded....";
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(db_con))
                    {
                        SqlCommand cmd = new SqlCommand("sp_AddCategory", con);
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CategoryName", cateObj.CategoryName);
                        cmd.Parameters.AddWithValue("@imageURL",path);
                        cmd.Parameters.AddWithValue("@Cate_Status", cateObj.Cate_Status);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                TempData["success"] = "Category Create Successfully";
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
                tbl_Category cateObjd1 = new tbl_Category();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_CategoryDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cateObjd1 = new tbl_Category
                        {
                            CategoryId = Convert.ToInt32(reader["CategoryId"].ToString()),
                            CategoryName = reader["CategoryName"].ToString(),
                            imageURL = reader["imageURL"].ToString(),
                            Cate_Status = Convert.ToInt32(reader["Cate_Status"].ToString()),
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
        public ActionResult Edit(int id, tbl_Category editcate, HttpPostedFileBase categoryimg)
        {
            try
            {
                string path;
                if (categoryimg == null)
                {
                     path = editcate.imageURL;
                }
                else
                {
                     path = uploadimgfile(categoryimg);
                    string path1 = Path.Combine(Server.MapPath(editcate.imageURL));
                    FileInfo fi = new FileInfo(path1);
                    if (fi != null)
                    {
                        System.IO.File.Delete(path1);
                        fi.Delete();
                    }
                }
                using (SqlConnection con = new SqlConnection(db_con))
                {                    
                    SqlCommand cmd = new SqlCommand("sp_EditCategory", con);
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", id);
                    cmd.Parameters.AddWithValue("@CategoryName", editcate.CategoryName);
                    cmd.Parameters.AddWithValue("@imageURL", path);
                    cmd.Parameters.AddWithValue("@Cate_Status", editcate.Cate_Status);
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
                tbl_Category cateObjd1 = new tbl_Category();
                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_CategoryDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cateObjd1 = new tbl_Category
                        {
                            CategoryId = Convert.ToInt32(reader["CategoryId"].ToString()),
                            CategoryName = reader["CategoryName"].ToString(),
                            imageURL = reader["imageURL"].ToString(),
                            Cate_Status = Convert.ToInt32(reader["Cate_Status"].ToString()),
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
        public ActionResult Delete(int id, tbl_Category delCate)
        {
            try
            {
                string path1 = Path.Combine(Server.MapPath(delCate.imageURL));
                FileInfo fi = new FileInfo(path1);
                if (fi != null)
                {
                    System.IO.File.Delete(path1);
                    fi.Delete();
                }

                using (SqlConnection con = new SqlConnection(db_con))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteCategory", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CategoryId", id);
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
                        path = Path.Combine(Server.MapPath("~/Images/category"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Images/category/" + random + Path.GetFileName(file.FileName);

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
