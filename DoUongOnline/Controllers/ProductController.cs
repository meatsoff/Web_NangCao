using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoUongOnline.Models;
using PagedList.Mvc;
using PagedList;
using System.Data.Entity;
using System.Net;
using System.IO;

namespace DoUongOnline.Controllers
{
    public class ProductController : Controller
    {
        private WebsiteCoffeeShopEntities1 db = new WebsiteCoffeeShopEntities1();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        //Danh sách loại sản phẩm
        public ActionResult ListLoaiSP(int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pagesize = 5;
            int pagenum = (page ?? 1);
            {
                var c = db.LoaiSanPham.OrderBy(x => x.IdLoaiSP);
                return View(c.ToPagedList(pagenum, pagesize));
            }
        }

        //Tạo loại sản phẩm
        public ActionResult CreateLoaiSP()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLoaiSP([Bind(Include = "IdLoaiSP,TenLoaiSP")] LoaiSanPham lsp)
        {
            if (ModelState.IsValid)
            {
                db.LoaiSanPham.Add(lsp);
                db.SaveChanges();
                return RedirectToAction("ListLoaiSP");
            }
            return View(lsp);
        }

        //Danh sách sản phẩm

        public ActionResult List(string searchBy, string search, int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            List<SanPham> sp = db.SanPham.ToList();
            foreach (var item in sp)
            {
                if(item.SoLuongCon == 0)
                {
                    item.TinhTrang = false;
                    db.SaveChanges();
                }
                if (item.IdKM != null)
                {
                    if (item.KhuyenMai.TinhTrang == true)
                    {
                        double gia = item.GiaBan - ((item.GiaBan * item.KhuyenMai.PhanTramKM) / 100);
                        item.GiaSauKM = gia;
                    }
                    else if (item.KhuyenMai.TinhTrang == false)
                    {
                        item.IdKM = null;
                        db.SaveChanges();
                    }
                }
            }
            List<CongThuc> ct = db.CongThuc.ToList();
            foreach (var item in ct)
            {
                List<Kho> k = db.Kho.Where(c => c.ChiTietPhieuNhap.IdNL == item.IdNL).ToList();
                if (k != null)
                {
                    foreach (var item1 in k)
                    {
                        item.SanPham.SoLuongCon = k.Sum(x => x.SoLuong) / item.DinhLuong;
                        db.SaveChanges();
                    }
                }
            }
            if (searchBy == "TenLoaiSP" )
            {              
                int pagesize = 5;
                int pagenum = (page ?? 1);
                {
                    var c = db.SanPham.OrderBy(x => x.IdSP).Where(x => x.LoaiSanPham.TenLoaiSP.Contains(search) || search == null ).ToList();                   
                    return View(c.ToPagedList(pagenum, pagesize));
                }
            }
            else
            {            
                int pagesize = 5;
                int pagenum = (page ?? 1);
                {
                    var c = db.SanPham.OrderBy(x => x.IdSP).Where(x => x.TenSP.Contains(search) || search == null).ToList();
                    return View(c.ToPagedList(pagenum, pagesize));
                }
            }
           
        }
    

        //Tạo sản phẩm
        public ActionResult SelectCate()
        {
            LoaiSanPham lsp = new LoaiSanPham();
            lsp.ListLoaiSanPham = db.LoaiSanPham.ToList<LoaiSanPham>();          
            return PartialView(lsp);
        }
        public ActionResult SelectKM()
        {
            KhuyenMai lkm = new KhuyenMai();
            lkm.ListKM = db.KhuyenMai.Where(x=> x.TinhTrang == true).ToList<KhuyenMai>();
            return PartialView(lkm);
        }
        public ActionResult Create()
        {
            SanPham sp = new SanPham();       
            return View(sp);
        }
        [HttpPost]
       
        public ActionResult Create(SanPham sp)
        {
            try 
            { 
                if (sp.UploadImage != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(sp.UploadImage.FileName);
                    string extent = Path.GetExtension(sp.UploadImage.FileName);
                    filename = filename + extent;
                    sp.HinhAnh = "~/Content/Images/" + filename;
                    sp.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), filename));
                }
                sp.TinhTrang = true;
                db.SanPham.Add(sp);              
                db.SaveChanges();
                Response.Write("<script> alert('Thêm sản phẩm thành công.')</script>");
                return RedirectToAction("List");
            }
            catch
            {            
                return View(sp);          
            }                
        }

        //Edit sản phẩm
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sp = db.SanPham.Find(id);                
            Session["imgPath"] = sp.HinhAnh;
            Session["condition"] = sp.TinhTrang;
            Session["tensp"] = sp.TenSP;
            if (sp == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdLoaiSP = new SelectList(db.LoaiSanPham, "IdLoaiSP", "TenLoaiSP", sp.IdLoaiSP);
            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham sp,int? id)
        {
            if (ModelState.IsValid)
            {
                sp.GiaSauKM = sp.GiaBan;
                if (sp.UploadImage != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(sp.UploadImage.FileName);
                    string extent = Path.GetExtension(sp.UploadImage.FileName);
                    filename = filename + extent;
                    sp.HinhAnh = "~/Content/Images/" + filename;
                    sp.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), filename));
                } 
                else
                {
                    sp.HinhAnh = Session["imgPath"].ToString();
                }             
               
                db.Entry(sp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.IdLoaiSP = new SelectList(db.LoaiSanPham, "IdLoaiSP", "TenLoaiSP", sp.IdLoaiSP);
            return View(sp);
        }

        
        

        //Menu sản phẩm
        public ActionResult Menu(string search, string type)
        {
            if (search == null)
            {
                if (type != null)
                {
                    if (type == "Tất cả")
                    {
                        var obj1 = db.SanPham.ToList();
                        Session["type"] = "Tất cả";
                        return View(obj1);
                    }
                    else
                    {
                        var obj = db.SanPham.OrderBy(x => x.IdSP).Where(x => x.LoaiSanPham.TenLoaiSP == type);
                        Session["type"] = type;
                        return View(obj.ToList());
                    }
                }
                else
                {
                    var obj = db.SanPham.ToList();
                    Session["type"] = "Tất cả";
                    return View(obj);
                }
            }
            else
            {
                var c = db.SanPham.OrderBy(x => x.IdSP).Where(x => x.TenSP.Contains(search));
                return View(c.ToList());
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sp = db.SanPham.Find(id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sp = db.SanPham.Find(id);
            db.SanPham.Remove(sp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}