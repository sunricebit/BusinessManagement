using Business_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Business_Management_System.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        BusinessEntities db = new BusinessEntities();
        Response response = new Response();

        [HttpPost,Route("addNewProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewProduct([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    response.message = "Only admin can add Product!";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                product.status = "true";
                db.Products.Add(product);
                db.SaveChanges();
                response.message = "Product added Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet, Route("getAllProduct")]
        public HttpResponseMessage GetAllProduct()
        {
            try
            {
                var result = from Products in db.Products
                             join Categories in db.Categories
                             on Products.categoryId equals Categories.id
                             select new
                             {
                                 Products.id,
                                 Products.name,
                                 Products.quantity,
                                 Products.description,
                                 Products.price,
                                 Products.status,
                                 categoryId = Categories.id,
                                 categoryName = Categories.name,
                             };
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet, Route("getProductByCategory/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductByCateogry(int id)
        {
            try
            {
                var result = db.Products.Where(p => p.categoryId == id && p.status == "true")
                    .Select(p => new { p.id, p.name}).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet, Route("getProductById/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductById(int id)
        {
            try
            {
                var product = db.Products.FirstOrDefault(p => p.id == id);
                return Request.CreateResponse(HttpStatusCode.OK, product);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("updateProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateProduct([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    response.message = "Only admin can update product!";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                Product productObj = db.Products.Find(product.id);
                if (productObj == null)
                {
                    response.message = "Product doesn't exist!";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                productObj.name = product.name;
                productObj.description = product.description;
                productObj.price = product.price;
                productObj.quantity = product.quantity;
                productObj.categoryId = product.categoryId;
                db.Entry(productObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "Product updated Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route("deleteProduct/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteProduct(int id)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    response.message = "Only admin can delete category!";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                Product productObj = db.Products.Find(id);
                if (productObj == null)
                {
                    response.message = "Product doesn't exist!";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                db.Products.Remove(productObj);
                db.SaveChanges();
                response.message = "Product deleted Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route("updateProductStatus")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateProductStatus([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    response.message = "Only admin can update product!";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                Product productObj = db.Products.Find(product.id);
                if (productObj == null)
                {
                    response.message = "Product doesn't exist!";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                productObj.status = product.status;
                db.Entry(productObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "Product status updated Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
