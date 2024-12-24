using Ontap2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Ontap2.Controllers
{
    public class HomeController : Controller
    {
        QuanLyBanQuanAoEntities db = new QuanLyBanQuanAoEntities();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.PhanLoaiSanPham = db.PhanLoaiSanPhams.ToList();

            var distinctproduct = db.Sanphams.GroupBy(x => x.TenSanpham)
                .Select(x => x.FirstOrDefault()).ToList();

            ViewBag.SanPham = distinctproduct;
           
            return View();
        }
      

        public ActionResult GetProductsByCategory(string category)
        {
            List<Sanpham> products;
            string categoryName = category.TrimStart('#');

            var categoryId = db.PhanLoaiSanPhams
                .Where(c => c.TenPhanLoai.ToLower() == categoryName)
                .Select(c => c.PhanLoaiSanPhamID)
                .FirstOrDefault();

            products = db.Sanphams
                .Where(c => c.PhanLoaiSanPhamID == categoryId)
                .ToList();

            return PartialView("_ProductList", products);
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Create()
        {
            var phanloai = db.PhanLoaiSanPhams;
            ViewBag.PhanLoaiSanPhamID = new SelectList(phanloai, "PhanLoaiSanPhamID", "TenPhanLoai");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Sanpham s, HttpPostedFileBase fileupload)
        {
            var phanloai = db.PhanLoaiSanPhams;
            ViewBag.PhanLoaiSanPhamID = new SelectList(phanloai, "PhanLoaiSanPhamID", "TenPhanLoai");

            if (ModelState.IsValid)
            {
                var allowExtension = new[] { ".jpg" };
                var fileExtension = Path.GetExtension(fileupload.FileName).ToLower();

                if (!allowExtension.Contains(fileExtension))
                {
                    ViewBag.ThongBao = "Ảnh phải có đuôi .jpg";
                    return View(s);
                }    
                var fileName = Path.GetFileName(fileupload.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);

                if (System.IO.File.Exists(path)) {
                    ViewBag.ThongBao = "Ảnh đã tồn tại";
                    return View(s);
                }

                fileupload.SaveAs(path);
                s.AnhDaiDien = fileName;
                db.Sanphams.Add(s);
                db.SaveChanges();
                ViewBag.ThanhCong = "Create successfully";
                return RedirectToAction("Index");
            }

            return View(s);
        }

        public ActionResult Detail(int id)
        {
            var product = db.Sanphams.SingleOrDefault(p => p.SanphamID == id);

            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var product = db.Sanphams.SingleOrDefault(p => p.SanphamID == id);

            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }

        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteSP(int id)
        {
            var product = db.Sanphams.SingleOrDefault(p => p.SanphamID == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            db.Sanphams.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit (int id)
        {
            var phanloai = db.PhanLoaiSanPhams.ToList();
            ViewBag.PhanLoaiSanPhamID = new SelectList(phanloai, "PhanLoaiSanPhamID", "TenPhanLoai");

            var product = db.Sanphams.SingleOrDefault(s => s.SanphamID == id);

            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(product);
        }
        [HttpPost]
        public ActionResult Edit (Sanpham s, HttpPostedFileBase fileupload)
        {
            var phanloai = db.PhanLoaiSanPhams.ToList();
            ViewBag.PhanLoaiSanPhamID = new SelectList(phanloai, "PhanLoaiSanPhamID", "TenPhanLoai");

            if (fileupload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn một file ảnh.";
                return View(s);
            }

            if (ModelState.IsValid)
            {
                var allowExtension = new[] { ".jpg" };
                var fileExtention = Path.GetExtension(fileupload.FileName).ToLower();

                if (!allowExtension.Contains(fileExtention))
                {
                    ViewBag.ThongBao = "Ảnh có đuôi .jpg";
                    return View(s);
                }    
                var filename = Path.GetFileName(fileupload.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images/"), filename);        

                if (System.IO.File.Exists(path))
                {
                    ViewBag.ThongBao = "ảnh đã tồn tại";
                    return View(s);
                }   
                fileupload.SaveAs(path);

                s.AnhDaiDien = filename;
                db.Sanphams.AddOrUpdate(s);
                db.SaveChanges();
            return RedirectToAction("Index");
            }
            return View(s);
        }

    }

}