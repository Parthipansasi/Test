﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Catagorytask.Models;
using ClosedXML.Excel;


namespace Catagorytask.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        private sapEntities1 db = new sapEntities1();

        [Authorize]
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public FileResult ExportToExcel()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[10] { new DataColumn("Sno"),
                                                     new DataColumn("Catagory"),
                                                     new DataColumn("Name"),
                                                     new DataColumn("Quantity"),
                                                     new DataColumn("Created_on"),
                                                     new DataColumn("Created_by"),
                                                     new DataColumn("Updated_on"),
                                                     new DataColumn("Updated_by"),
                                                     new DataColumn("Price"),
                                                     new DataColumn("Status_report") });

            var sapEntities1 = from AllCatagory_Table in db.AllCatagory_Table1 select AllCatagory_Table;

            foreach (var insurance in sapEntities1)
            {
                dt.Rows.Add(insurance.Sno, insurance.Catagory, insurance.Name, insurance.Quantity,
                    insurance.Created_on, insurance.Created_by, insurance.Updated_on, insurance.Updated_by,
                    insurance.Price, insurance.Status_report);
            }

            using (XLWorkbook wb = new XLWorkbook()) //Install ClosedXml from Nuget for XLWorkbook  
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream()) //using System.IO;  
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelFile.xlsx");
                }
            }
        }

        
        [HttpPost]

        public ActionResult ImportFromExcel(HttpPostedFileBase postedFile)

        {


            if (ModelState.IsValid)
            {
                if (postedFile != null && postedFile.ContentLength > (1024 * 1024 * 50))  // 50MB limit  
                {
                    ModelState.AddModelError("postedFile", "Your file is to large. Maximum size allowed is 50MB !");
                }
                else
                {
                    string filePath = string.Empty;
                    if (postedFile != null)

                    {

                        string path = Server.MapPath("~/file/");

                        if (!Directory.Exists(path))

                        {

                            Directory.CreateDirectory(path);

                        }

                        filePath = path + Path.GetFileName(postedFile.FileName);

                        string extension = Path.GetExtension(postedFile.FileName);

                        postedFile.SaveAs(filePath);

                        string conString = string.Empty;

                        switch (extension)

                        {

                            case ".xls": //Excel 97-03.

                                conString = ConfigurationManager.ConnectionStrings["ExcelConString03"].ConnectionString;

                                break;

                            case ".xlsx": //Excel 07 and above.

                                conString = ConfigurationManager.ConnectionStrings["ExcelConString07"].ConnectionString;

                                break;

                        }

                        DataTable dt = new DataTable();

                        conString = string.Format(conString, filePath);

                        using (OleDbConnection oleconexcel = new OleDbConnection(conString))

                        {

                            using (OleDbCommand cmdexcel = new OleDbCommand())

                            {

                                using (OleDbDataAdapter oleexcel = new OleDbDataAdapter())

                                {

                                    cmdexcel.Connection = oleconexcel;

                                    //Get the name of First Sheet.

                                    oleconexcel.Open();

                                    DataTable dtExcelSchema;

                                    dtExcelSchema = oleconexcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();

                                    oleconexcel.Close();

                                    //Read Data from First Sheet.

                                    oleconexcel.Open();

                                    cmdexcel.CommandText = "SELECT * From [" + sheetName + "]";

                                    oleexcel.SelectCommand = cmdexcel;

                                    oleexcel.Fill(dt);

                                    oleconexcel.Close();

                                }

                            }

                        }

                        conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;



                        using (SqlConnection con = new SqlConnection(conString))

                        {

                            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))

                            {

                                //Set the database table name.

                                sqlBulkCopy.DestinationTableName = "[dbo].[AllCatagory Table1]";

                                //[OPTIONAL]: Map the Excel columns with that of the database table

                                sqlBulkCopy.ColumnMappings.Add("Sno", "Sno");

                                sqlBulkCopy.ColumnMappings.Add("Catagory", "Catagory");

                                sqlBulkCopy.ColumnMappings.Add("Name", "Name");

                                sqlBulkCopy.ColumnMappings.Add("Quantity", "Quantity");

                                sqlBulkCopy.ColumnMappings.Add("Created_on", "Created_on");

                                sqlBulkCopy.ColumnMappings.Add("Created_by", "Created_by");

                                sqlBulkCopy.ColumnMappings.Add("Updated_on", "Updated_on");
                                sqlBulkCopy.ColumnMappings.Add("Updated_by", "Updated_by");
                                sqlBulkCopy.ColumnMappings.Add("Price", "Price");
                                sqlBulkCopy.ColumnMappings.Add("Status_report", "Status_report");

                                con.Open();
                                
                                sqlBulkCopy.WriteToServer(dt);
                                con.Close();

                            }
                        }
                    }
                }
            }               
            return View("Index");
        }        
        public ActionResult Getlist()
        {
            List<AllCatagory_Table1> empResult = null;
            try {
                using (sapEntities1 db = new sapEntities1())
                {
                    List<AllCatagory_Table1> emplist = db.AllCatagory_Table1.Where(x=>x.Status_report==null).ToList<AllCatagory_Table1>();
                    empResult = emplist;
                    
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                empResult = null;
            }
            return Json(new { data = empResult }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult Add(int id=0)
        {
            if (id == 0)
            {
                return View(new AllCatagory_Table1());
            }
            else
            {
                using (sapEntities1 db = new sapEntities1())
                {
                    return View(db.AllCatagory_Table1.Where(x => x.Id == id).FirstOrDefault<AllCatagory_Table1>());
                }
            }
        }

       
        [HttpPost]
        public ActionResult Add(AllCatagory_Table1 emp)
        {
            using(sapEntities1 db =new sapEntities1())
            {
                if(emp.Id == 0)
                {
                    db.AllCatagory_Table1.Add(emp);   
                    db.SaveChanges();
                    return Json(new {success=true,message="Saved successfully"},JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(emp).State=EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new AllCatagory_Table1());
            }
            else
            {
                using (sapEntities1 db = new sapEntities1())
                {
                    return View(db.AllCatagory_Table1.Where(x => x.Id == id).FirstOrDefault<AllCatagory_Table1>());
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(AllCatagory_Table1 emp)
        {
            using (sapEntities1 db = new sapEntities1())
            {
                if (emp.Id == 0)
                {
                    db.AllCatagory_Table1.Add(emp);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Saved successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(emp).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            using (sapEntities1 db = new sapEntities1())
            {
                return View(db.AllCatagory_Table1.Where(x => x.Id == id).FirstOrDefault<AllCatagory_Table1>());
            }
            
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            AllCatagory_Table1 registartion = db.AllCatagory_Table1.Find(id);
            registartion.Status_report = "InActive";
            db.Entry(registartion).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted successfully" }, JsonRequestBehavior.AllowGet);
        }

        private sapEntities2 DB = new sapEntities2();
        public ActionResult register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult register(Register_Table Registers)
        {
            using (sapEntities2 db = new sapEntities2())
            {
                if (Registers.Id == 0)
                {
                    db.Register_Tables.Add(Registers);
                    db.SaveChanges();
                    return Json(new { success = true, message = "User register successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(Registers).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        

    }
}