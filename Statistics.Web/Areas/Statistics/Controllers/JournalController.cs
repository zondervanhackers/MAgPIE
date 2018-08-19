using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ZondervanLibrary.SharedLibrary.Entities;
namespace ZondervanLibrary.Statistics.Web.Areas.Statistics.Controllers
{
    struct VendorValue
    {
        public string name;
        public int? value;
    }
    struct DatabaseValue
    {
        public string name;
        public int? value;
    }
    struct JournalValue
    {
        public string Name;
        public int? value;
    }
    struct DummyValue
    {
        public List<List<JournalValue>> Journals;
    }

    public class JournalController : Controller
    {
        // GET: Statistics/Journal
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Welcome(string name, int numTimes = 1)
        {
            ViewBag.Message = "Hello" + name;
            ViewBag.NumTimes = numTimes;
            return View();
        }
        public JsonResult Overall()
        {
            using (DataClasses1DataContext context = new DataClasses1DataContext(@"Data Source=.\SQLEXPRESS;Initial Catalog=Statistics;Integrated Security=True"))
            {
                var thing = new
                {
                    OverallFullText = context.VendorRecords.Select(record => record.FullText).Sum(),
                    OverallRecordViews = context.VendorRecords.Select(record => record.RecordViews).Sum(),
                    OverallResultClicks = context.VendorRecords.Select(record => record.ResultClicks).Sum(),
                    OverallSearches = context.VendorRecords.Select(record => record.Searches).Sum()
                };
                return Json(thing,JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult TopVendors(string category="FT")
        {
            using (DataClasses1DataContext context = new DataClasses1DataContext(@"Data Source=.\SQLEXPRESS;Initial Catalog=Statistics;Integrated Security=True"))
            {
                if (category == "FT")
                    return Json(context.VendorRecords.Select(record => new VendorValue { name = record.Vendor.VendorName, value = record.FullText }).Where(record => record.value > 0).Take(10).ToArray().OrderByDescending(record => record.value), JsonRequestBehavior.AllowGet);
                else if (category == "RV")
                    return Json(context.VendorRecords.Select(record => new VendorValue { name = record.Vendor.VendorName, value = record.RecordViews }).Where(record => record.value > 0).Take(10).ToArray().OrderByDescending(record => record.value), JsonRequestBehavior.AllowGet);
                else if (category == "RC")
                    return Json(context.VendorRecords.Select(record => new VendorValue { name = record.Vendor.VendorName, value = record.ResultClicks }).Where(record => record.value > 0).Take(10).ToArray().OrderByDescending(record => record.value), JsonRequestBehavior.AllowGet);
                else if (category == "SS")
                    return Json(context.VendorRecords.Select(record => new VendorValue { name = record.Vendor.VendorName, value = record.Searches }).Where(record => record.value > 0).Take(10).ToArray().OrderByDescending(record => record.value), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult TopDatabases(string category="FT")
        {
            using (DataClasses1DataContext context = new DataClasses1DataContext(@"Data Source=.\SQLEXPRESS;Initial Catalog=Statistics;Integrated Security=True"))
            {
                if (category == "FT")
                    return Json(context.DatabaseRecords.Select(record => new DatabaseValue { name = record.Database.DatabaseName, value = record.FullText }).Where(record => record.value > 0).Take(10).ToArray().OrderByDescending(record => record.value), JsonRequestBehavior.AllowGet);
                else if (category == "RV")
                    return Json(context.DatabaseRecords.Select(record => new DatabaseValue { name = record.Database.DatabaseName, value = record.RecordViews }).Where(record => record.value > 0).Take(10).ToArray().OrderByDescending(record => record.value), JsonRequestBehavior.AllowGet);
                else if (category == "RC")
                    return Json(context.DatabaseRecords.Select(record => new DatabaseValue { name = record.Database.DatabaseName, value = record.RecordViews }).Where(record => record.value > 0).Take(10).ToArray().OrderByDescending(record => record.value), JsonRequestBehavior.AllowGet);
                else if (category == "SS")
                    return Json(context.DatabaseRecords.Select(record => new DatabaseValue { name = record.Database.DatabaseName, value = record.Searches }).Where(record => record.value > 0).Take(10).ToArray().OrderByDescending(record => record.value), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //Added base info such as Year Array and number of total Months(DONE)
        //Fix Chart drawing. Something wrong with values being passed.
        //Added grabbing actual Database values.
        public JsonResult BeginInfo()
        {
            using (DataClasses1DataContext context = new DataClasses1DataContext(@"Data Source=.\SQLEXPRESS;Initial Catalog=Statistics;Integrated Security=True"))
            {
                DateTime startDate = context.VendorRecords.Select(date => date.RunDate).Min();
                DateTime endDate = context.VendorRecords.Select(date => date.RunDate).Max();
                var thing = new
                {
                    startMonth = 0,//startDate.Month,
                    startYear = 2000,//startDate.Year,
                    endMonth = 0,//endDate.Month,
                    endYear = 2010//endDate.Year
                };
                return Json(thing, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ChartData(int startMonth = 0, int startYear = 0, int endMonth = 0, int endYear = 0)
        {
            DummyValue values = new DummyValue() { Journals = new List<List<JournalValue>>()};
            Random rand = new Random();
            DateTime date = new DateTime(2000, 1, 1);
            for (int i = 0; i < 120; i++)
            {
                JournalValue journalitem = new JournalValue()
                {
                    Name = "FullText",
                    value = rand.Next(400)
                };
                JournalValue journalitem1 = new JournalValue()
                {
                    Name = "RecordViews",
                    value = rand.Next(400)
                };
                JournalValue journalitem2 = new JournalValue()
                {
                    Name = "ResultClicks",
                    value = rand.Next(400)
                };
                JournalValue journalitem3 = new JournalValue()
                {
                    Name = "Searches",
                    value = rand.Next(400)
                };
                values.Journals.Add(new List<JournalValue>());
                values.Journals[i].Add(journalitem);
                values.Journals[i].Add(journalitem1);
                values.Journals[i].Add(journalitem2);
                values.Journals[i].Add(journalitem3);
            }
            return Json(values, JsonRequestBehavior.AllowGet);
        }
    }
}