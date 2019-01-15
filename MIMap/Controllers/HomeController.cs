using BusinessHandler.MessageHandler;
using BusinessHandler.Model;
using MIMap.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIMap.Controllers
{
    [SessionCheck]
    public class HomeController : Controller
    {
        ISearchQueryRepository searchQueryRepository;
        IMapDataRepository mapRepository;
        IMeetingNote meetingNoteRepository;
        IKeyWord _keyWord;
        IDocumentRepository _documentRepository;
        public HomeController()
        {
            meetingNoteRepository = DependencyResolver.Current.GetService<IMeetingNote>();
            searchQueryRepository = DependencyResolver.Current.GetService<ISearchQueryRepository>();
            mapRepository = DependencyResolver.Current.GetService<IMapDataRepository>();
            _keyWord = DependencyResolver.Current.GetService<IKeyWord>();
            _documentRepository= DependencyResolver.Current.GetService<IDocumentRepository>();

        }
        public ActionResult Index()
        {
            var state = "MI";
            if (Request.QueryString["state"] != null)
            {
                state = Request.QueryString["state"];
            }
            var data = mapRepository.GetDashboardData(state);
            return View(data);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Map()
        {
            var state = "MI";
            if (Request.QueryString["state"] != null)
            {
                state = Request.QueryString["state"];
            }
            var keyWordList = _keyWord.GetKeyWordList();
            ViewData["municipalityList"] = mapRepository.GetFilterData(state);
            ViewData["keyWordList"] = keyWordList;
            var message = new DocQueryMessage();
            message.State = state;
            var mapInitialData = mapRepository.GetMapAreaData(message);

            ViewData["mapInitialData"] = mapInitialData;
            return View();
        }

        public JsonResult GetParentDataList(DocQueryMessage message)
        {
            var result = new List<MapMeeting>();
            int total = 0;
            result = mapRepository.GetMainDataList(message, out total);
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetMapMasterData(DocQueryMessage message)
        {
            var result = mapRepository.GetMapAreaData(message);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateDocStatus(DocQueryResultModel message)
        {
            DocQueryDB.UpdateDocStatus(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDocImportant(DocQueryResultModel message)
        {
            DocQueryDB.UpdateDocImportant(message);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSearchQuery()
        {
            var data = searchQueryRepository.GetSearchQuery();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSearchQuery(string query, string title)
        {
            var data = JsonConvert.DeserializeObject<DocQueryMessage>(query);
            if (string.IsNullOrEmpty(title))
            {
                if (!string.IsNullOrWhiteSpace(data.CityName))
                {
                    title = data.CityName;
                }
                if (!string.IsNullOrWhiteSpace(data.CountyName))
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        title += "&";
                    }
                    title += data.CountyName;
                }
                if (!string.IsNullOrWhiteSpace(data.KeyWord))
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        title += "&";
                    }
                    title += data.KeyWord;
                }
                if (!string.IsNullOrWhiteSpace(data.DeployDate))
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        title += "&";
                    }
                    title += data.DeployDate;
                }
                if (!string.IsNullOrWhiteSpace(data.MeetingDate))
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        title += "&";
                    }
                    title += data.MeetingDate;
                }
            }
            if (!string.IsNullOrEmpty(title))
            {
                searchQueryRepository.AddSearchQuery(query, title);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSearchQuery(string guid, string title, string query)
        {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(guid))
            {
                searchQueryRepository.UpdateSearchQuery(guid, title, query);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSearchQuery(string guid)
        {
            if (!string.IsNullOrWhiteSpace(guid))
            {
                searchQueryRepository.DeleteSearchQuery(guid);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSearchQueryAmount(string guid)
        {
            if (!string.IsNullOrWhiteSpace(guid))
            {
                searchQueryRepository.AddSearchQueryAmount(guid);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateMapColor(string cityGuid, string color)
        {
            mapRepository.UpdateMapColor(cityGuid, color);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityOrdinance(string guid)
        {
            int total = 0;
            var message = new DocQueryMessage();
            message.limit = 10;
            var list = mapRepository.GetCityOrdinanceList(message, out total, guid);
            var data = new CityOrdinance();
            data.FacililtyGrowerClassAZoningInd = "--";
            data.FacililtyGrowerClassAZoningCom = "--";
            data.FacililtyGrowerClassALimit = "No Cap";

            data.FacililtyGrowerClassBZoningInd = "--";
            data.FacililtyGrowerClassBZoningCom = "--";
            data.FacililtyGrowerClassBLimit = "No Cap";

            data.FacililtyGrowerClassCZoningInd = "--";
            data.FacililtyGrowerClassCZoningCom = "--";
            data.FacililtyGrowerClassCLimit = "No Cap";

            data.FacililtyProvZoningInd = "--";
            data.FacililtyProvZoningCom = "--";
            data.FacililtyProvLimit = "No Cap";

            data.FacililtyProcZoningInd = "--";
            data.FacililtyProcZoningCom = "--";
            data.FacililtyProcLimit = "No Cap";

            data.FacililtySTZoningInd = "--";
            data.FacililtySTZoningCom = "--";
            data.FacililtySTLimit = "No Cap";

            data.FacililtySCZoningInd = "--";
            data.FacililtySCZoningCom = "--";
            data.FacililtySCLimit = "No Cap";
            if (list.Any())
            {
                data = list.FirstOrDefault();
                var limitA = data.FacililtyGrowerClassALimit;
                data.FacililtyGrowerClassALimit = GetOrdianceLimit(limitA, true);
                data.FacililtyGrowerClassAComCap = GetOrdianceLimit(limitA, false);

                var limitB = data.FacililtyGrowerClassBLimit;
                data.FacililtyGrowerClassBLimit = GetOrdianceLimit(limitB, true);
                data.FacililtyGrowerClassBComCap = GetOrdianceLimit(limitB, false);

                var limitC = data.FacililtyGrowerClassCLimit;
                data.FacililtyGrowerClassCLimit = GetOrdianceLimit(limitC, true);
                data.FacililtyGrowerClassCComCap = GetOrdianceLimit(limitC, false);

                var limitpProc = data.FacililtyProcLimit;
                data.FacililtyProcLimit = GetOrdianceLimit(limitpProc, true);
                data.FacililtyProcComCap = GetOrdianceLimit(limitpProc, false);

                var limitProv = data.FacililtyProvLimit;
                data.FacililtyProvLimit = GetOrdianceLimit(limitProv, true);
                data.FacililtyProvComCap = GetOrdianceLimit(limitProv, false);

                var limitSC = data.FacililtySCLimit;
                data.FacililtySCLimit = GetOrdianceLimit(limitSC, true);
                data.FacililtySCComCap = GetOrdianceLimit(limitSC, false);

                var limitST = data.FacililtySTLimit;
                data.FacililtySTLimit = GetOrdianceLimit(limitST, true);
                data.FacililtySTComCap = GetOrdianceLimit(limitST, false);
            }
            if(!string.IsNullOrEmpty(data.CityFileName))
            {
              
                data.CityFileDisplayName = data.CityFileName.Substring(data.CityFileName.LastIndexOf("--") + 2);
                data.CityFileName = StaticSetting.uploadPath + "/uploads/" + data.CityFileName;
                data.CityFileDisplayName = "<a href='" + data.CityFileName + "' target='_blank'>" + data.CityFileDisplayName + "</a>"
;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveCityOrdinance(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var data = JsonConvert.DeserializeObject<CityOrdinance>(str);
                var user = (UserAccount)Session["UserAccount"];
                if (user != null)
                {
                    data.ModifyUser = user.Email;
                }
                if(!string.IsNullOrEmpty(data.CityFileName))
                {
                    data.CityFileName = data.CityFileName.Replace("\"", "");
                }
                if (mapRepository.UpdateCityOrdinance(data))
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }

            }
            return Json("Error in server", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityOrdinanceList(DocQueryMessage message)
        {
            var result = new List<CityOrdinance>();
            int total = 0;
            result = mapRepository.GetCityOrdinanceList(message, out total);

            foreach (var r in result)
            {

                // these information is used for ordinances tab page
                if (!string.IsNullOrEmpty(r.DraftDate))
                {
                    r.OrdinanceTime = "Draft:" + r.DraftDate;
                }
                if (!string.IsNullOrEmpty(r.FinalDate))
                {
                    if(!string.IsNullOrEmpty(r.OrdinanceTime))
                    {
                        r.OrdinanceTime += "<br>";
                    }
                    r.OrdinanceTime += "Final:" + r.FinalDate;
                }
                r.FacililtyGrowerClassAZoning += "Industrial " + r.FacililtyGrowerClassAZoningInd;
                r.FacililtyGrowerClassAZoning += "<br>";
                r.FacililtyGrowerClassAZoning += "Commercial " + r.FacililtyGrowerClassAZoningCom;

                r.FacililtyGrowerClassBZoning += "Industrial " + r.FacililtyGrowerClassBZoningInd;
                r.FacililtyGrowerClassBZoning += "<br>";
                r.FacililtyGrowerClassBZoning += "Commercial " + r.FacililtyGrowerClassBZoningCom;

                r.FacililtyGrowerClassCZoning += "Industrial " + r.FacililtyGrowerClassCZoningInd;
                r.FacililtyGrowerClassCZoning += "<br>";
                r.FacililtyGrowerClassCZoning += "Commercial " + r.FacililtyGrowerClassCZoningCom;

                r.FacililtyProvZoning += "Industrial " + r.FacililtyProvZoningInd;
                r.FacililtyProvZoning += "<br>";
                r.FacililtyProvZoning += "Commercial " + r.FacililtyProvZoningCom;

                r.FacililtyProcZoning += "Industrial " + r.FacililtyProcZoningInd;
                r.FacililtyProcZoning += "<br>";
                r.FacililtyProcZoning += "Commercial " + r.FacililtyProcZoningCom;

                r.FacililtySTZoning += "Industrial " + r.FacililtySTZoningInd;
                r.FacililtySTZoning += "<br>";
                r.FacililtySTZoning += "Commercial " + r.FacililtySTZoningCom;

                r.FacililtySCZoning += "Industrial " + r.FacililtySCZoningInd;
                r.FacililtySCZoning += "<br>";
                r.FacililtySCZoning += "Commercial " + r.FacililtySCZoningCom;

                //r.BufferSchoolFeet = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.BufferSchoolNote.Replace("'", "") + "'>" + r.BufferSchoolFeet + "</a>";
                //r.BufferDaycareFeet = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.BufferDaycareNote.Replace("'", "") + "'>" + r.BufferDaycareFeet + "</a>";
                //r.BufferParkFeet = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.BufferParkNote.Replace("'", "") + "'>" + r.BufferParkFeet + "</a>";
                //r.BufferSDMFeet = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.BufferSDMNote.Replace("'", "") + "'>" + r.BufferSDMFeet + "</a>";
                //r.BufferReligiousFeet = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.BufferReligiousNote.Replace("'", "") + "'>" + r.BufferReligiousFeet + "</a>";
                //r.BufferResidentialFeet = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.BufferResidentialNote.Replace("'", "") + "'>" + r.BufferResidentialFeet + "</a>";
                //r.BufferRoadFeet = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.BufferRoadNote.Replace("'", "") + "'>" + r.BufferRoadFeet + "</a>";
                //r.BufferOtherFeet = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.BufferOtherNote.Replace("'", "") + "'>" + r.BufferOtherFeet + "</a>";

                //r.FacililtySCPermit = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.FacililtySCNote.Replace("'", "") + "'>" + r.FacililtySCPermit + "</a>";
                //r.FacililtyProvPermit = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.FacililtyProvNote.Replace("'", "") + "'>" + r.FacililtyProvPermit + "</a>";
                //r.FacililtyProcPermit = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.FacililtyProcNote.Replace("'", "") + "'>" + r.FacililtyProcPermit + "</a>";
                //r.FacililtyGrPermit = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.FacililtyGrNote.Replace("'", "") + "'>" + r.FacililtyGrPermit + "</a>";
                //r.FacililtySTPermit = @"<a  data-toggle='tooltip' data-placement='right' title='" + r.FacililtySTNote.Replace("'", "") + "'>" + r.FacililtySTPermit + "</a>" class='btn btn-sm btn-info' role='button' ;

                if (string.IsNullOrEmpty(r.BufferSchoolFeet) && !string.IsNullOrEmpty(r.BufferSchoolNote))
                {
                    r.BufferSchoolFeet = "Note.";
                }
                if (!string.IsNullOrEmpty(r.BufferSchoolFeet) && !string.IsNullOrEmpty(r.BufferSchoolNote))
                {
                    r.BufferSchoolFeet = @"<a tabindex='0'  data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.BufferSchoolNote.Replace("'", "") + "'>" + r.BufferSchoolFeet + "</a>";

                }

             
                if (string.IsNullOrEmpty(r.BufferDaycareFeet) && !string.IsNullOrEmpty(r.BufferDaycareNote))
                {
                    r.BufferDaycareFeet = "Note.";
                }
                if (!string.IsNullOrEmpty(r.BufferDaycareFeet) && !string.IsNullOrEmpty(r.BufferDaycareNote))
                {
                    r.BufferDaycareFeet = @"<a  tabindex='1' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.BufferDaycareNote.Replace("'", "") + "'>" + r.BufferDaycareFeet + "</a>";
                }

              
                if (string.IsNullOrEmpty(r.BufferParkFeet) && !string.IsNullOrEmpty(r.BufferParkNote))
                {
                    r.BufferParkFeet = "Note.";
                }
                if (!string.IsNullOrEmpty(r.BufferParkFeet) && !string.IsNullOrEmpty(r.BufferParkNote))
                {
                    r.BufferParkFeet = @"<a  tabindex='2' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.BufferParkNote.Replace("'", "") + "'>" + r.BufferParkFeet + "</a>";
                }

             
                if (string.IsNullOrEmpty(r.BufferSDMFeet) && !string.IsNullOrEmpty(r.BufferSDMNote))
                {
                    r.BufferSDMFeet = "Note.";
                }
                if (!string.IsNullOrEmpty(r.BufferSDMFeet) && !string.IsNullOrEmpty(r.BufferSDMNote))
                {
                    r.BufferSDMFeet = @"<a tabindex='3' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.BufferSDMNote.Replace("'", "") + "'>" + r.BufferSDMFeet + "</a>";
                }

              
                if (string.IsNullOrEmpty(r.BufferReligiousFeet) && !string.IsNullOrEmpty(r.BufferReligiousNote))
                {
                    r.BufferReligiousFeet = "Note.";
                }
                if (!string.IsNullOrEmpty(r.BufferReligiousFeet) && !string.IsNullOrEmpty(r.BufferReligiousNote))
                {
                    r.BufferReligiousFeet = @"<a tabindex='4' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.BufferReligiousNote.Replace("'", "") + "'>" + r.BufferReligiousFeet + "</a>";
                }

               
                if (string.IsNullOrEmpty(r.BufferResidentialFeet) && !string.IsNullOrEmpty(r.BufferResidentialNote))
                {
                    r.BufferResidentialFeet = "Note.";
                }
                if (!string.IsNullOrEmpty(r.BufferResidentialFeet) && !string.IsNullOrEmpty(r.BufferResidentialNote))
                {
                    r.BufferResidentialFeet = @"<a tabindex='5' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.BufferResidentialNote.Replace("'", "") + "'>" + r.BufferResidentialFeet + "</a>";

                }

                if (string.IsNullOrEmpty(r.BufferRoadFeet) && !string.IsNullOrEmpty(r.BufferRoadNote))
                {
                    r.BufferRoadFeet = "Note.";
                }
                if (!string.IsNullOrEmpty(r.BufferRoadFeet) && !string.IsNullOrEmpty(r.BufferRoadNote))
                {
                    r.BufferRoadFeet = @"<a  tabindex='6'  data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.BufferRoadNote.Replace("'", "") + "'>" + r.BufferRoadFeet + "</a>";
                }

             
                if (string.IsNullOrEmpty(r.BufferOtherFeet) && !string.IsNullOrEmpty(r.BufferOtherNote))
                {
                    r.BufferOtherFeet = "Note.";
                }
                if (!string.IsNullOrEmpty(r.BufferOtherFeet) && !string.IsNullOrEmpty(r.BufferOtherNote))
                {
                    r.BufferOtherFeet = @"<a tabindex='7'  data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.BufferOtherNote.Replace("'", "") + "'>" + r.BufferOtherFeet + "</a>";
                }

                if (!string.IsNullOrEmpty(r.FacililtySCPermit) && !string.IsNullOrEmpty(r.FacililtySCNote))
                {
                    r.FacililtySCPermit = @"<a tabindex='8'  data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.FacililtySCNote.Replace("'", "") + "'>" + r.FacililtySCPermit + "</a>";
                }
                if (!string.IsNullOrEmpty(r.FacililtyProvPermit) && !string.IsNullOrEmpty(r.FacililtyProvNote))
                {
                    r.FacililtyProvPermit = @"<a tabindex='9' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.FacililtyProvNote.Replace("'", "") + "'>" + r.FacililtyProvPermit + "</a>";
                }
                if (!string.IsNullOrEmpty(r.FacililtyProcPermit) && !string.IsNullOrEmpty(r.FacililtyProcNote))
                {
                    r.FacililtyProcPermit = @"<a tabindex='10' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.FacililtyProcNote.Replace("'", "") + "'>" + r.FacililtyProcPermit + "</a>";
                }
                if (!string.IsNullOrEmpty(r.FacililtyGrowerClassAPermit) && !string.IsNullOrEmpty(r.FacililtyGrowerClassANote))
                {
                    r.FacililtyGrowerClassAPermit = @"<a tabindex='11' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.FacililtyGrowerClassANote.Replace("'", "") + "'>" + r.FacililtyGrowerClassAPermit + "</a>";
                }
                if (!string.IsNullOrEmpty(r.FacililtySTPermit) && !string.IsNullOrEmpty(r.FacililtySTNote))
                {
                    r.FacililtySTPermit = @"<a tabindex='12' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='left' data-content='" + r.FacililtySTNote.Replace("'", "") + "'>" + r.FacililtySTPermit + "</a>";
                }
                if (!string.IsNullOrEmpty(r.FacililtyGrowerClassBPermit) && !string.IsNullOrEmpty(r.FacililtyGrowerClassBNote))
                {
                    r.FacililtyGrowerClassBPermit = @"<a tabindex='13' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.FacililtyGrowerClassBNote.Replace("'", "") + "'>" + r.FacililtyGrowerClassBPermit + "</a>";
                }
                if (!string.IsNullOrEmpty(r.FacililtyGrowerClassCPermit) && !string.IsNullOrEmpty(r.FacililtyGrowerClassCNote))
                {
                    r.FacililtyGrowerClassCPermit = @"<a tabindex='14' data-toggle='popover' data-trigger='focus' title='Note'  data-placement='right' data-content='" + r.FacililtyGrowerClassCNote.Replace("'", "") + "'>" + r.FacililtyGrowerClassCPermit + "</a>";
                }
                r.Action = "<button type='button' class='btn btn-default'   data-cityGuid='" + r.CityGuid + "'  onclick='editCityNote(this); return false'>Edit</button>";
                if (!string.IsNullOrEmpty(r.CityFileName))
                {
                    r.CityFileName = StaticSetting.uploadPath + "/uploads/" + r.CityFileName;
                    r.Municipality = "<a href='" + r.CityFileName + "' target='_blank'>" + r.Municipality + "</a>";
                }
            }
            return Json(new { total = total, rows = result }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UploadCityFile()
        {
            var fileName = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                fileName = file.FileName;
                string mimeType = file.ContentType;
                fileName = Guid.NewGuid().ToString() + "--" + fileName;
                
                var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                file.SaveAs(path);
            }
            return Json(fileName);

        }

        [HttpGet]
        public JsonResult GetCartoSearchResult(string objectIds, string state)
        {
            if(string.IsNullOrWhiteSpace(objectIds))
            {
                return null;
            }
            objectIds = objectIds.Substring(0, objectIds.Length - 1);
            var result = mapRepository.GetCartoSearchResult(objectIds,state);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetContentDetail(string contentId,string keyWord)
        {
            var data = mapRepository.GetContentDetail(contentId, keyWord);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult UpdateDocumentMeetingDate(string docGuid, string meetingDateStr)
        {
            var result = "Success";
            if (!string.IsNullOrWhiteSpace(docGuid) && !string.IsNullOrWhiteSpace(meetingDateStr))
            {
                var meetingDate = new DateTime();
                if (DateTime.TryParse(meetingDateStr, out meetingDate))
                {
                    _documentRepository.UpdateDocumentMeetingDate(docGuid, meetingDate.ToString("yyyy-MM-dd"));
                }
                else
                {
                    result = "meeting date is invalid";
                }
                
            }
            else
            {
                result = "parameter is empty";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public string GetOrdianceLimit(string data, bool limit)
        {
            var limitStr = "";
            var comCap = "";
            int i = 0;
            if (int.TryParse(data, out i))
            {
                limitStr = data;
            }
            else if (data.ToLower() == "no cap")
            {
                limitStr = data;
            }
            else
            {
                comCap = data;
            }
            if (limit)
            {
                return limitStr;
            }
            else
            {
                return comCap;
            }
        }

    }
}