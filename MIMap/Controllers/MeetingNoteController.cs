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

        IMeetingNote meetingNoteRepository;
        public MeetingNoteController()
        {
            meetingNoteRepository = DependencyResolver.Current.GetService<IMeetingNote>();

        }
        // GET: MeetingNote
        public ActionResult Index()
        {
            var municipalityList = DocQueryDB.GetMapMunicipality();
            ViewData["municipalityList"] = municipalityList;
            return View();
        }

        [HttpGet]
        public JsonResult GetMeetingNotes(string docGuid)
        {
            var data = meetingNoteRepository.GetMeetingNotes(docGuid, "");
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveMeetingNotes(string notes)
        {
            if (!string.IsNullOrEmpty(notes))
            {
                var noteList = JsonConvert.DeserializeObject<List<MeetingNote>>(notes);
                meetingNoteRepository.UpdateMeetingNotes(noteList);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllNotes(DocQueryMessage message)
        {
            int total = 0;
            var result = meetingNoteRepository.GetAllDataList(message, out total);
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);
        }
    }
}