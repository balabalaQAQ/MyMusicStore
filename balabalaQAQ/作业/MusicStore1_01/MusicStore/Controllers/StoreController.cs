using MusicStore.ViewModels;
using MusicStoreEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicStore.Controllers
{
    public class StoreController : Controller
    {
        private static readonly EntityDbContext _context = new EntityDbContext();
        // GET: Store

        public ActionResult Detail(Guid id)
        {

            if ((Session["LoginUserSessionModel"] as LoginUserSessionModel) == null)
            {
                ViewBag.img = "/Content/images/boys.jpg";
                ViewBag.name = "<a href='../../Account/login'> 请登录 </a>";
            }
            else
            {
                ViewBag.img = (Session["LoginUserSessionModel"] as LoginUserSessionModel).Person.Avarda;
                ViewBag.name = (Session["LoginUserSessionModel"] as LoginUserSessionModel).Person.Name;
            }

            var Albums = _context.Albums.SingleOrDefault(x => x.ID == id);

            foreach (var item in Albums.Reply.OrderByDescending(x => x.ReplyTime))
            {
                var sonCmt = _context.Reply.Where(x => x.ParentReply.ID == item.ID).ToList();
                
                ViewBag.count = sonCmt.Count();
            }
            return View(Albums);
            
            }

        public ActionResult ShowCmt(string pid)
        {
            var htmlString = "";
            //子回复
            Guid id = Guid.Parse(pid);
            var cmts = _context.Reply.Where(x => x.ParentReply.ID == id).OrderByDescending(x => x.CreateDateTime).ToList();
            //原回复
            var pcmt = _context.Reply.Find(id);
            htmlString += "<div class=\"modal-header\">";
            htmlString += "<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\">×</button>";
            htmlString += "<h4 class=\"modal-title\" id=\"myModalLabel\">";
            htmlString += "<em>楼主</em>" + pcmt.Person.Name + "  发表于" + pcmt.CreateDateTime.ToString("yyyy年MM月dd日 hh点mm分ss秒") + ":<br/>" + pcmt.Content;
            htmlString += " </h4> </div>";

            htmlString += "<div class=\"modal-body\">";
            foreach (var item in cmts)
            {
                htmlString += "<p>"+ item.Content+"</p>";
             }
            htmlString += "</div>";
            //子回复

            htmlString += "</div><div class=\"modal-footer\"></div>";
            return Json(htmlString);
        }

        /// <summary>
        /// 按分类显示专辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Browser(Guid id)
        {
            var list = _context.Albums.Where(x => x.Genre.ID == id)
                .OrderByDescending(x => x.PublisherDate).ToList();
            return View(list);
        }

    }
}