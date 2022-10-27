using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;
using PagedList.Mvc;
using PagedList;
using System.Net;
using System.Data.Entity;

namespace DoUongOnline.Controllers
{
    public class EmployeeController : Controller
    {
        private WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        // GET: Manager
        public ActionResult Index()
        {
            var dh = db.DonHang.Where(x => x.IdTinhTrangDH == "1").ToList();
            ViewBag.tongdhmoi = dh.Count();
            
            return View();
        }
        [HttpPost]
        public ActionResult Index(String start, String end)
        {
            var dh = db.DonHang.Where(x => x.IdTinhTrangDH == "1").ToList();
            ViewBag.tongdhmoi = dh.Count();
            if (start == "" || end == "")
                return RedirectToAction("Index", "Employee");
            
            return View();
        }        
        

        //Danh sách loại nhân viên
        public ActionResult ListLoaiNV(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 5;
            int pagenum = (page ?? 1);
            {
                var c = db.LoaiNhanVien.OrderBy(x => x.IdLoaiNV);
                return View(c.ToPagedList(pagenum, pagesize));
            }
        }

        //Tạo loại nhân viên
        public ActionResult CreateLoaiNV()
        {        
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLoaiNV,TenLoaiNhanVien")] LoaiNhanVien lnv)
        {                 
            if (ModelState.IsValid)
            {                
                db.LoaiNhanVien.Add(lnv);
                db.SaveChanges();
                return RedirectToAction("ListLoaiNV");
            }
            return View(lnv) ;
        }

        //Danh sách nhân viên
        public ActionResult List(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 5;
            int pagenum = (page ?? 1);
            {
                var c = db.NhanVien.OrderBy(x => x.IdNV);              
                return View(c.ToPagedList(pagenum, pagesize));
            }
        }


        //Tạo mới nhân viên
        public bool IsNameExist(string name)
        {
            var result = db.NhanVien.Any(c => c.EmailNV == name);
            return result;
        }
        public ActionResult Create()
        {
            ViewBag.IdLoaiNV = new SelectList(db.LoaiNhanVien, "IdLoaiNV", "TenLoaiNhanVien");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdNV,EmailNV,MatKhau,TenNhanVien,SDTNV,NgaySinh,DiaChi,NgayCapTK,TinhTrang,IdLoaiNV")] NhanVien nv)
        {
            if (IsNameExist(nv.EmailNV))
            {
                ModelState.AddModelError("EmailNV", "Email đã tồn tại");
            }
            if (ModelState.IsValid)
            {                     
                    nv.NgayCapTK = DateTime.Now;
                    nv.TinhTrang = true;
                    db.NhanVien.Add(nv);
                    db.SaveChanges();
                Response.Write("<script> alert('Thêm nhân viên thành công.')</script>");
                return RedirectToAction("List");          
            }
            ViewBag.IdLoaiNV = new SelectList(db.LoaiNhanVien, "IdLoaiNV", "TenLoaiNhanVien");
            return View(nv);
        }

        //Edit Nhân viên
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }          
            NhanVien nv = db.NhanVien.Find(id);
            nv.NgaySinh.ToShortDateString();
            if (nv == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdLoaiNV = new SelectList(db.LoaiNhanVien, "IdLoaiNV", "TenLoaiNhanVien", nv.IdLoaiNV);
            return View(nv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdNV,EmailNV,MatKhau,TenNhanVien,SDTNV,NgaySinh,DiaChi,NgayCapTK,TinhTrang,IdLoaiNV")] NhanVien nv)
        {
            if (ModelState.IsValid)
            {
                nv.NgaySinh.ToShortDateString();
                db.Entry(nv).State = EntityState.Modified;
                db.SaveChanges();            
                return RedirectToAction("List");
            }
            ViewBag.IdLoaiNV = new SelectList(db.LoaiNhanVien, "IdLoaiNV", "TenLoaiNhanVien", nv.IdLoaiNV);
            return View(nv);
        }

        //Đăng nhập nhân viên
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["employee"] != null)
                return RedirectToAction("Index", "Employee");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(NhanVien ut)
        {
            if (ModelState.IsValid)
            {
                var log = db.NhanVien.Where(model => model.EmailNV.Equals(ut.EmailNV) && model.MatKhau.Equals(ut.MatKhau)).FirstOrDefault();
                if (log != null)
                {                                  
                    Session["employee"] = log;
                    Session["TypeNV"] = log.IdLoaiNV;
                    Session["employeeid"] = log.IdNV;
                    return RedirectToAction("Index", "Employee");
                }
                else if (log != null && log.TinhTrang == false)
                {
                    ModelState.AddModelError("MatKhau", "Tài khoản không khả dụng");
                }
                else
                {
                    ModelState.AddModelError("MatKhau", "Vui lòng kiểm tra email hoặc mật khẩu");
                }
            }
            return View(ut);
        }

        //Đăng xuất nhân viên
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Employee");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}