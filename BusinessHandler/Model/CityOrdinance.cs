using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessHandler.Model
{
    public class CityOrdinance
    {
        public string Municipality { get; set; }
        public string CityGuid { get; set; }

        public string OptStatus { get; set; }

        public string OrdinanceTime { get; set; }
        public string DraftDate { get; set; }
        public string FinalDate { get; set; }

        public string Measurement { get; set; }

        public string BufferSchoolFeet { get; set; }

        public string BufferSchoolNote { get; set; }

        public string BufferDaycareFeet { get; set; }
        public string BufferDaycareNote { get; set; }
        public string BufferParkFeet { get; set; }
        public string BufferParkNote { get; set; }
        public string BufferSDMFeet { get; set; }
        public string BufferSDMNote { get; set; }
        public string BufferReligiousFeet { get; set; }
        public string BufferReligiousNote { get; set; }
        public string BufferOtherFeet { get; set; }
        public string BufferOtherNote { get; set; }
        public string BufferResidentialFeet { get; set; }
        public string BufferResidentialNote { get; set; }
        public string BufferRoadFeet { get; set; }
        public string BufferRoadNote { get; set; }

        public string FacililtyGrowerClassAPermit { get; set; }
        public string FacililtyGrowerClassAZoningInd { get; set; }
        public string FacililtyGrowerClassAZoningCom { get; set; }
        public string FacililtyGrowerClassALimit { get; set; }
        public string FacililtyGrowerClassANote { get; set; }
        public string FacililtyGrowerClassAZoning { get; set; }


        public string FacililtyGrowerClassBPermit { get; set; }
        public string FacililtyGrowerClassBZoningInd { get; set; }
        public string FacililtyGrowerClassBZoningCom { get; set; }
        public string FacililtyGrowerClassBLimit { get; set; }
        public string FacililtyGrowerClassBNote { get; set; }
        public string FacililtyGrowerClassBZoning { get; set; }

        public string FacililtyGrowerClassCPermit { get; set; }
        public string FacililtyGrowerClassCZoningInd { get; set; }
        public string FacililtyGrowerClassCZoningCom { get; set; }
        public string FacililtyGrowerClassCLimit { get; set; }
        public string FacililtyGrowerClassCNote { get; set; }
        public string FacililtyGrowerClassCZoning { get; set; }

        public string FacililtyProvPermit { get; set; }
        public string FacililtyProvZoningInd { get; set; }
        public string FacililtyProvZoningCom { get; set; }
        public string FacililtyProvLimit { get; set; }
        public string FacililtyProvNote { get; set; }
        public string FacililtyProvZoning { get; set; }

        public string FacililtyProcPermit { get; set; }
        public string FacililtyProcZoningInd { get; set; }
        public string FacililtyProcZoningCom { get; set; }
        public string FacililtyProcLimit { get; set; }
        public string FacililtyProcNote { get; set; }
        public string FacililtyProcZoning { get; set; }


        public string FacililtySCPermit { get; set; }
        public string FacililtySCZoningInd { get; set; }
        public string FacililtySCZoningCom { get; set; }
        public string FacililtySCLimit { get; set; }
        public string FacililtySCNote { get; set; }
        public string FacililtySCZoning { get; set; }

        public string FacililtySTPermit { get; set; }
        public string FacililtySTZoningInd { get; set; }
        public string FacililtySTZoningCom { get; set; }
        public string FacililtySTLimit { get; set; }
        public string FacililtySTNote { get; set; }
        public string FacililtySTZoning { get; set; }

        public string ModifyUser { get; set; }

        public string Action { get; set; }
        public string CityFileName { get; set; }
        public string CityFileDisplayName { get; set; }

    }
}
