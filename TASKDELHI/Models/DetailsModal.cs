using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TASKDELHI.Models
{
    public class DetailsModal
    {
      public int id {  get; set; }
      public string lname {  get; set; }
      public string fname {  get; set; }
      public string password {  get; set; }
      public string email {  get; set; }
      public string phoneno {  get; set; }
      public int age {  get; set; }
      public string address {  get; set; }
      public HttpPostedFileBase profile {  get; set; }
        public string confirmpassword { get; set; }
    }
}