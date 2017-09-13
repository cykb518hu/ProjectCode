using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIMap.Controllers
{
    public class MeetingNoteController : Controller
    {
        // GET: MeetingNote
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMeetingNotes(string docGuid)
        {
            var data = DocQueryDB.GetMeetingNotes(docGuid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveMeetingNotes(string notes)
        {
            if (!string.IsNullOrEmpty(notes))
            {
                var noteList = JsonConvert.DeserializeObject<List<MeetingNote>>(notes);
                DocQueryDB.UpdateMeetingNotes(noteList);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}