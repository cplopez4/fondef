using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Fondef.DAL;
using Fondef.Models;
using Catfood.Shapefile;

namespace Fondef.Controllers
{
    public class CuencaController : Controller
    {
        private FondefContext db = new FondefContext();

        // GET: Cuenca
        public ActionResult Index()
        {
            return View(db.Cuencas.ToList());
        }

        // GET: Cuenca/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuenca cuenca = db.Cuencas.Find(id);
            if (cuenca == null)
            {
                return HttpNotFound();
            }
            return View(cuenca);
        }

        [HttpPost]
        public ActionResult SaveShape(FormCollection collection)
        {
            Models.Cuenca entry = new Models.Cuenca()
            {
                Name = collection["Name"],
                Coordinates = collection["Coordinates"],
            };

            if (ModelState.IsValid)
            {
                db.Cuencas.Add(entry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Cuenca/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cuenca/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Area,Slope,Coordinates,LonCenter,LatCenter")] Cuenca cuenca)
        {
            if (ModelState.IsValid)
            {
                db.Cuencas.Add(cuenca);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cuenca);
        }

        // GET: Cuenca/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuenca cuenca = db.Cuencas.Find(id);
            if (cuenca == null)
            {
                return HttpNotFound();
            }
            return View(cuenca);
        }

        // POST: Cuenca/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Area,Slope,Coordinates,LonCenter,LatCenter")] Cuenca cuenca)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuenca).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuenca);
        }

        // GET: Cuenca/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cuenca cuenca = db.Cuencas.Find(id);
            if (cuenca == null)
            {
                return HttpNotFound();
            }
            return View(cuenca);
        }

        // POST: Cuenca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cuenca cuenca = db.Cuencas.Find(id);
            db.Cuencas.Remove(cuenca);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult SaveUploadedFile()
        {
            bool isSavedSuccessfully = true;

            foreach (string fileName in Request.Files)
            {
                //Guardamos los archivos en archivos temporales en la carpeta shapefiles
                HttpPostedFileBase file = Request.Files[fileName];
                string path = Server.MapPath("..\\Content\\ShapeFiles\\");
                if (Path.GetExtension(file.FileName).Equals(".shp"))
                    file.SaveAs(path + "tmp.shp");
                else if (Path.GetExtension(file.FileName).Equals(".dbf"))
                    file.SaveAs(path + "tmp.dbf");
                else if (Path.GetExtension(file.FileName).Equals(".shx"))
                    file.SaveAs(path + "tmp.shx");
                //else, ignorar el archivo extra
            }


            if (isSavedSuccessfully)
            {
                return Json(new { Message = "File saved" });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        public ActionResult GetCoordinatesFromShapeFile()
        {
            Shapefile shapefile;
            var shapeCoordinates = new Dictionary<string, string>();
            try
            {
                shapefile = new Shapefile(Server.MapPath("..\\Content\\ShapeFiles\\tmp"), Shapefile.ConnectionStringTemplateJet);

                foreach (Catfood.Shapefile.Shape shape in shapefile)
                {
                    ShapePolygon poligono = shape as ShapePolygon;
                    foreach (PointD[] part in poligono.Parts)
                    {
                        int i = 0;
                        foreach (PointD point in part)
                        {
                            shapeCoordinates.Add(i.ToString(), point.X.ToString() + ";" + point.Y.ToString());
                            i++;
                        }
                    }
                }

                //Eliminamos los archivos ya que no los necesitamos más
                deleteFiles();

                return Json(shapeCoordinates, JsonRequestBehavior.AllowGet);
            }
            catch (FileNotFoundException e)
            {
                var negativeResponse = new Dictionary<string, string>();
                negativeResponse.Add("0", "Error");

                //Eliminamos los archivos ya que no los necesitamos más
                deleteFiles();

                return Json(negativeResponse, JsonRequestBehavior.AllowGet);
            }
        }

        public void deleteFiles()
        {
            var shp = new FileInfo(Server.MapPath("..\\Content\\ShapeFiles\\tmp.shp"));
            var dbf = new FileInfo(Server.MapPath("..\\Content\\ShapeFiles\\tmp.dbf"));
            var shx = new FileInfo(Server.MapPath("..\\Content\\ShapeFiles\\tmp.shx"));
            if (shp.Exists)
                shp.Delete();
            if (shp.Exists)
                dbf.Delete();
            if (shp.Exists)
                shx.Delete();
        }
    }
}
