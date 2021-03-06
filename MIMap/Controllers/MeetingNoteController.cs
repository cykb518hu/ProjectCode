﻿using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using MIMap.Tools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MIMap.Controllers
{
    [SessionCheck]
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
            var municipalityList = mapRepository.GetFilterData("");
            ViewData["municipalityList"] = municipalityList;
            return View();
        }

        [HttpGet]
        public JsonResult GetMeetingNotes(string docGuid)
        {
            var data = meetingNoteRepository.GetMeetingNotes(docGuid, "");
            foreach (var r in data)
            {
                r.AllTags = new List<NoteTags>();
                var tags = StaticSetting.DefaultTags + r.Tags;
                var arr = tags.Split(',');
                foreach (var a in arr)
                {
                    if (r.AllTags.Any(x => x.tag == a))
                    {
                        continue;
                    }
                    r.AllTags.Add(new NoteTags { tag = a });
                }
            }
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
                meetingNoteRepository.UpdateMeetingNotes(noteList);
                count = meetingNoteRepository.GetMeetingRelatedNotesAmount(noteList[0].DocGuid);
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
        public JsonResult GetMapPopupInfo(string guid)
        {
            var data = meetingNoteRepository.GetMapPopupInfo(guid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMeetingCalendar(DocQueryMessage message)
        {
            if(message==null)
            {
                message = new DocQueryMessage();
            }
            message.Important = "";
            message.IsViewed = "";
            var result = meetingNoteRepository.GetMeetingCalendar(message);
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMeetingType(string guid)
        {
            var result = meetingNoteRepository.GetMeetingType(guid);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}