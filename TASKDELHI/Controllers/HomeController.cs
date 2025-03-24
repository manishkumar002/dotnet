using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TASKDELHI.Models;
using System.Text;

namespace TASKDELHI.Controllers
{
    public class HomeController : Controller
    {
        DBManagement db = new DBManagement();
        public ActionResult Index()
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
               new SqlParameter("@action",2),
            };
            DataTable dt1 = db.Exceuteselect("sp_details", parameters);
            ViewBag.data1 = dt1;
            return View();
        }
        [HttpPost]
        public ActionResult Index(DetailsModal s)
        {
            SqlParameter[] param = new SqlParameter[] {
        new SqlParameter("@action", 1),
        new SqlParameter("@fname", s.fname),
        new SqlParameter("@lname", s.lname),
        new SqlParameter("@email", s.email),
        new SqlParameter("@phoneno", s.phoneno),
        new SqlParameter("@age", s.age),
        new SqlParameter("@address", s.address),
        new SqlParameter("@password", s.password),
        new SqlParameter("@confirmpassword", s.confirmpassword),
        new SqlParameter("@profile", s.profile.FileName)
    };

            int res = db.ExceuteCUD("sp_details", param);

            if (res > 0)
            {
                s.profile.SaveAs(Server.MapPath("/Content/pic/" + s.profile.FileName));
                return Content("<script>alert('Data inserted');window.location.href='/home/index'</script>");
            }
            else
            {
                return Content("<script>alert('Data not inserted');window.location.href='/home/index'</script>");
            }
        }

        public ActionResult Delete(int? id)
        {
            SqlParameter[] para1 = new SqlParameter[] {
        new SqlParameter("@action", 3),
        new SqlParameter("@id", id),
    };

            int res = db.ExceuteCUD("sp_details", para1);
            if (res > 0)
            {
                return Content("<script>alert('Data deleted');window.location.href='/home/index'</script>");
            }
            else
            {
                return Content("<script>alert('Data not deleted');window.location.href='/home/index'</script>");
            }
        }
        public ActionResult edit(int? id)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@action",4),
                  new SqlParameter("@id",id),
            };
            DataTable dt1 = db.Exceuteselect("sp_details", parameters);
            ViewBag.data1 = dt1;
            return View();
        }
        [HttpPost]
        public ActionResult edit(DetailsModal s)
        {
            
                SqlParameter[] param = new SqlParameter[]
                {
             new SqlParameter("@action",5),
             new SqlParameter("@id",s.id),
             new SqlParameter("@fname",s.fname),
            new SqlParameter("@lname",s.lname),
            new SqlParameter("@email",s.email),
            new SqlParameter("@phoneno",s.phoneno),
            new SqlParameter("@age",s.age),
            new SqlParameter("@address",s.address),
            new SqlParameter("@password",s.password),
            new SqlParameter("@confirmpassword",s.confirmpassword),
            new SqlParameter("@profile",s.profile.FileName)
                };
                int res = db.ExceuteCUD("sp_details", param);
                if (res > 0)
                {
                    s.profile.SaveAs(Server.MapPath("/Content/pic/" + s.profile.FileName));
                    return Content("<script>alert('Data updated');window.location.href='/home/index'</script>");
                }
                else
                {
                    return Content("<script>alert('Data  not updated');window.location.href='/home/index'</script>");
                }
            }


        [HttpPost]
        public ActionResult SaveApiDetails(string apikey, string senderid, string number, string message)
        {
            string encodedConnectionStringFromCookie = Request.Cookies["ConnectionString"]?.Value;
            byte[] bytes = Convert.FromBase64String(encodedConnectionStringFromCookie);
            string Database_WF_APPLICATION = Encoding.UTF8.GetString(bytes);

            try
            {
                // Construct the insert query
                string query = $@"
                     INSERT INTO APIDETAILS (apiUrl, apikey, senderid, message, number) 
                     VALUES ('/vb/apikey.php', '{apikey}', '{senderid}', '{message}', '{number}')
                 ";



                // Execute query with additional parameter
                Database.ExecuteNonQuery(Database_WF_APPLICATION, query, null);
                return Json(new { status = "success", message = "API details saved successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}