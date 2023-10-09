﻿using Business_Management_System.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Business_Management_System.Controllers
{
    [RoutePrefix("api/bill")]
    public class BillController : ApiController
    {
        BusinessEntities db = new BusinessEntities();
        Response response = new Response();
        private string pdfPath = "E:\\";

        [HttpPost, Route("generateReport")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GenerateReport([FromBody] Bill bill)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                var tick = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueId = tick.ToString() + '-' + guid;
                bill.createdBy = tokenClaim.Email;
                bill.uuid = uniqueId;
                db.Bills.Add(bill);
                db.SaveChanges();
                Get(bill);
                return Request.CreateResponse(HttpStatusCode.OK, new { uuid = bill.uuid });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private void Get(Bill bill)
        {
            try
            {
                dynamic productDetails = JsonConvert.DeserializeObject(bill.productDetails);
                var todayDate = "Date:" + Convert.ToDateTime(DateTime.Today).ToString("MM/dd/yyyy");
                PdfWriter writer = new PdfWriter(pdfPath + bill.uuid + ".pdf");
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                //Header
                Paragraph header = new Paragraph("Business Management System")
                    .SetBold()
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(25);
                document.Add(header);

                //Body
                //New Line
                Paragraph newline = new Paragraph(new Text("\n"));

                //Line Separator
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                //Customer Details
                Paragraph customerDetails = new Paragraph("Name" + bill.name + "\nEmail: " + bill.email + "\nContact Number: " + bill.contactNumber + "\nPayment Method:" + bill.paymentMethod);
                document.Add(customerDetails);

                //Table
                Table table = new Table(5, false);
                table.SetWidth(new UnitValue(UnitValue.PERCENT, 100));

                //Header of Table
                Cell headerName = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Name"));
                Cell headerCategory = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Category"));
                Cell headerQuantity = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Quantity"));
                Cell headerPrice = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Price"));
                Cell headerSubTotal = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .Add(new Paragraph("Sub Total"));

                table.AddCell(headerName);
                table.AddCell(headerCategory);
                table.AddCell(headerQuantity);
                table.AddCell(headerPrice);
                table.AddCell(headerSubTotal);

                foreach(JObject product in productDetails)
                {
                    Cell nameCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["name"].ToString()));
                    Cell categoryCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["category"].ToString()));
                    Cell quantityCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["quantity"].ToString()));
                    Cell priceCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["price"].ToString()));
                    Cell totalCell = new Cell(1, 1)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .Add(new Paragraph(product["total"].ToString()));

                    document.Add(table);

                    //Footer
                    Paragraph last = new Paragraph("Total " + bill.totalAmount + "\nThank you!");
                    document.Add(last);
                    document.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [HttpPost, Route("getPdf")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetPdf([FromBody] Bill bill)
        {
            try
            {
                if (bill.name != null)
                {
                    Get(bill);
                }
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                string filePath = pdfPath + bill.uuid.ToString() + ".pdf";

                byte[] bytes = File.ReadAllBytes(filePath);
                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentLength = bytes.LongLength;
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = bill.uuid.ToString() + ".pdf";
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(MimeMapping.GetMimeMapping(bill.uuid.ToString() + ".pdf"));
                return response;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpGet, Route("getBills")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetBills()
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    var userResult = db.Bills.Where(b => b.createdBy == tokenClaim.Email)
                                                .AsEnumerable().Reverse();
                    return Request.CreateResponse(HttpStatusCode.OK, userResult);
                }

                var adminResult = db.Bills.AsEnumerable().Reverse().ToList();
                return Request.CreateResponse(HttpStatusCode.OK, adminResult);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost, Route("deleteBill/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteBill(int id)
        {
            try
            {
                Bill billObj = db.Bills.Find(id);
                if(billObj == null)
                {
                    response.message = "Bill id doesn't exist";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                db.Bills.Remove(billObj);
                db.SaveChanges();
                response.message = "Bill Deleted Successfully";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
