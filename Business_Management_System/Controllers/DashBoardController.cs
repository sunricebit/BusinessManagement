using Business_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Business_Management_System.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashBoardController : ApiController
    {
        BusinessEntities db = new BusinessEntities();

        [HttpGet, Route("details")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetDetail()
        {
            try
            {
                var result = new
                {
                    category = db.Categories.Count(),
                    product = db.Products.Count(),
                    bill = db.Bills.Count(),
                    user = db.Users.Count(),
                };
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
