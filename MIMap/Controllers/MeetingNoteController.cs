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
        IMapDataRepository mapRepository;
        public MeetingNoteController()
        {
            meetingNoteRepository = DependencyResolver.Current.GetService<IMeetingNote>();
            mapRepository = DependencyResolver.Current.GetService<IMapDataRepository>();

        }
        // GET: MeetingNote
        public ActionResult Index()
        {
            var user = (UserAccount)Session["UserAccount"];
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var municipalityList = mapRepository.GetFilterData();
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
            var count = 0;
            if (!string.IsNullOrEmpty(notes))
            {
                var noteList = JsonConvert.DeserializeObject<List<MeetingNote>>(notes);
                var user = (UserAccount)Session["UserAccount"];
                if (user != null)
                {
                    foreach(var r in noteList)
                    {
                        r.ModifyUser = user.Email;
                    }
                }
                count =meetingNoteRepository.UpdateMeetingNotes(noteList);

            }
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllNotes(DocQueryMessage message)
        {
            int total = 0;
            var result = meetingNoteRepository.GetAllDataList(message, out total);
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMapPopUpInfo(string objectid)
        {
            var cityId = Convert.ToInt32(objectid);
            var data = meetingNoteRepository.GetMapPopUpInfo(cityId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}