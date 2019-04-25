using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceManagement.Models;
using System.IO;
using ExcelDataReader;
using System.Data;
using WebApplication.Models;

namespace AttendanceManagement.Controllers
{
    public class AttendanceController : Controller
    {
		AttendanceEntities db = new AttendanceEntities();

		// GET: Attendance
		public ActionResult Index()
		{
			var courselist = db.Courses.ToList();
			return View(courselist);

		}

		public ActionResult Index1()
		{
			return View();
		}

		//public ActionResult CreateClassView()
		//{
		//	return PartialView("CreateClassView");
		//}

		//[HttpPost]
		//public ActionResult CreateClass(Class Class)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		Class nClass = new Class();
		//		nClass.ClassName = Class.ClassName;
		//		nClass.StartDate = Class.StartDate;
		//		nClass.CreatedDate = DateTime.Now.Date;
		//		nClass.CreatedBy = User.Identity.Name;
		//		db.Classes.Add(nClass);
		//		db.SaveChanges();
		//		return RedirectToAction("manageClass");
		//	}
		//	return RedirectToAction("Index");

		//}

		//public ActionResult Edit(string id)
		//{
		//	var ClassID = int.Parse(id);
		//	var editClass = db.Classes.FirstOrDefault(x => x.ID == ClassID);
		//	return PartialView("EditClassView", editClass);
		//}

		//[HttpPost]
		//public ActionResult Edit(Class editClass)
		//{
		//	var eClass = db.Classes.FirstOrDefault(x => x.ID == editClass.ID);

		//	if (ModelState.IsValid)
		//	{
		//		eClass.StartDate = editClass.StartDate;
		//		eClass.Description = editClass.Description;
		//		eClass.ModifiedBy = User.Identity.Name;
		//		eClass.ModifiedDate = DateTime.Now.Date;
		//		db.SaveChanges();
		//	}
		//	return RedirectToAction("Index");

		//}
		public ActionResult manageClass(string id)
		{
			Session["CourseID"] = id;
			int courseID = int.Parse(id);
			var student = db.CourseMembers.Where(x => x.CourseID == courseID).ToList();
			ViewData["students"] = student;
			return View();
		}
		public ActionResult ManageStudent()
		{
			var user = db.CourseMembers.ToList();
			ViewBag.User = user;
			return View();
		}
		public ActionResult manageSession()
		{
			return View();
		}

		public ActionResult CreateFaculty()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateFaculty([Bind(Include = "Id,FacultyName,Description")] Faculty faculty)
		{
			if (ModelState.IsValid)
			{
				db.Faculties.Add(faculty);
				db.SaveChanges();
				return RedirectToAction("ManageFaculty");
			}

			return View(faculty);
		}

		public ActionResult ManageFaculty()
		{
			return View();
		}

		public ActionResult DetailFaculty()
		{
			return View();
		}

		public ActionResult EditClass1()
		{
			return View();
		}

		public ActionResult CreateClass1()
		{
			return View();
		}
		public ActionResult DetailClass1()
		{
			return View();
		}
		public ActionResult MajorList()
		{
			var major = db.Majors.ToList();
			return View(major);
		}
		public void SynMajor()
		{
			APIController api = new APIController();
			string data = api.ReadData("https://cntttest.vanlanguni.edu.vn:18081/SoDauBai/API/getMajors");
			List<MajorsModel> major = JsonConvert.DeserializeObject<List<MajorsModel>>(data);
			foreach (var item in major)
			{
				if (db.Majors.FirstOrDefault(x => x.Code == item.code) == null)
				{
					Major newMajor = new Major();
					newMajor.Code = item.code;
					newMajor.Name = item.name;
					db.Majors.Add(newMajor);
					db.SaveChanges();
				}
			}
		}
		public ActionResult CourseList()
		{
			//APIController api = new APIController();
			//string data = api.ReadData("https://cntttest.vanlanguni.edu.vn:18081/SoDauBai/API/getCourses");
			//CourseModel course = JsonConvert.DeserializeObject<CourseModel>(data);
			var course = db.Courses.ToList();
			return View(course);
		}
		//public ActionResult AddStudent()
		//{
		//	ViewBag.Group = new SelectList(groupdb.Groups.Where(x => x.GroupParent != null).ToList(), "ID", "GroupName");
		//	return View();
		//}
		public ActionResult AddStudent(string groupID)
		{
			//ViewBag.Group = new SelectList(groupdb.Groups.Where(x => x.GroupParent != null).ToList(), "ID", "GroupName");
			if (groupID != null)
			{
				int gID = int.Parse(groupID);
				//var user = groupdb.Groups.FirstOrDefault(x => x.ID == gID).Users.ToList();
				//return View(user);
				return View();
			}
			else
				return View();
		}
		[HttpPost]
		public ActionResult AddStudent(List<string> studentlist)
		{
			foreach (var item in studentlist)
			{
				int id = int.Parse(item);
				//var user = groupdb.Users.FirstOrDefault(x => x.ID == id);
				//CourseMember newMember = new CourseMember();
				//newMember.CourseID = int.Parse(Session["CourseID"].ToString());
				//newMember.StudentID = user.StID;
				//newMember.Name = user.FullName;
				//newMember.Email = user.Email;
				//newMember.DoB = user.DoB;
				//newMember.Avatar = user.AvatarBase64;
				//db.CourseMembers.Add(newMember);

			}
			db.SaveChanges();
			return RedirectToAction("manageClass", new { id = Session["CourseID"] });
		}
		public ActionResult SynCourse()
		{
			APIController api = new APIController();
			string data = api.ReadData("https://sodaubai.vanlanguni.edu.vn/API/getCourses");
			CourseModel course = JsonConvert.DeserializeObject<CourseModel>(data);
			foreach (var item in course.Courses)
			{
				var SynCourse = db.Courses.FirstOrDefault(x => x.Code == item.Code && x.Type1 == item.Type1 && x.Type2 == item.Type2 && x.Semester == course.Semester);
				if (SynCourse == null)
				{
					Course newCourse = new Course();
					newCourse.Code = item.Code;
					newCourse.CourseName = item.Name;
					newCourse.Type1 = item.Type1;
					newCourse.Type2 = item.Type2;
					newCourse.Major = db.Majors.FirstOrDefault(x => x.Code == item.Major).ID;
					newCourse.Credit = item.Credit;
					newCourse.Lecturer = item.Lecturer;
					newCourse.Students = item.Students;
					newCourse.DayOfWeek = item.DayOfWeek;
					newCourse.TimeSpan = item.TimeSpan;
					newCourse.Periods = item.Periods;
					newCourse.Room = item.Room;
					newCourse.Semester = course.Semester;
					db.Courses.Add(newCourse);
				}
				else
				{
					SynCourse.Code = item.Code;
					SynCourse.CourseName = item.Name;
					SynCourse.Type1 = item.Type1;
					SynCourse.Type2 = item.Type2;
					SynCourse.Major = db.Majors.FirstOrDefault(x => x.Code == item.Major).ID;
					SynCourse.Credit = item.Credit;
					SynCourse.Lecturer = item.Lecturer;
					SynCourse.Students = item.Students;
					SynCourse.DayOfWeek = item.DayOfWeek;
					SynCourse.TimeSpan = item.TimeSpan;
					SynCourse.Periods = item.Periods;
					SynCourse.Room = item.Room;
					SynCourse.Semester = course.Semester;
				}
			}
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult FacultyIndex()
		{
			var faculty = db.Faculties.ToList();
			List<FacultyViewModel> listfaculty = new List<FacultyViewModel>();
			foreach (var item in faculty)
			{
				FacultyViewModel facultyView = new FacultyViewModel();
				facultyView.ID = item.ID;
				facultyView.Name = item.Name;
				facultyView.Description = item.Description;
				//facultyView.GroupLink = groupdb.Groups.FirstOrDefault(x => x.ID == item.GroupID).GroupName;
				facultyView.Majors = item.Majors;
				listfaculty.Add(facultyView);
			}
			return View(listfaculty);
		}
		[HttpGet]
		public ActionResult CreateFaculty1()
		{
		//	ViewBag.GroupMap = groupdb.Groups.Where(x => x.GroupParent != null).ToList();
			return View();
		}
		[HttpPost]
		public ActionResult CreateFaculty1(Faculty faculty)
		{
			Faculty newfaculty = new Faculty();
			newfaculty.Name = faculty.Name;
			newfaculty.Description = faculty.Description;
			newfaculty.GroupID = faculty.GroupID;
			db.Faculties.Add(newfaculty);
			db.SaveChanges();
			return RedirectToAction("FacultyIndex");
		}

        public ActionResult Import(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReadExcel(HttpPostedFileBase fileupload)
        {
            List<Attendance> lstStudent = new List<Attendance>();
            if (ModelState.IsValid)
            {

                string filePath = string.Empty;
                if (Request != null)
                {
                    HttpPostedFileBase file = fileupload;
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {

                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        string path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        filePath = path + Path.GetFileName(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        file.SaveAs(filePath);
                        Stream stream = file.InputStream;
                        // We return the interface, so that
                        IExcelDataReader reader = null;
                        if (file.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (file.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "Try to get another file. This file cann't import because wrong type");
                            return RedirectToAction("Index");
                        }
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                        reader.Close();
                        string filedetails = path + fileName;
                        FileInfo fileinfo = new FileInfo(filedetails);
                        if (fileinfo.Exists)
                        {
                            fileinfo.Delete();
                        }
                        DataTable dt = result.Tables[0];
                        lstStudent = ConvertDataTable<Attendance>(dt);
                        TempData["Excelstudent"] = lstStudent;
                        TempData.Keep();
                    }
                }

            }
            return View("Import", new { id = Session["GroupID"] });
        }

        private static List<Attendance> ConvertDataTable<T>(DataTable dt)
        {
            List<Attendance> data = new List<Attendance>();
            dt.Rows.RemoveAt(0);

            DataRowCollection list = dt.Rows;
            foreach (DataRow row in list)
            {

                if (String.IsNullOrEmpty(row[0].ToString()))
                {
                    break;
                }
                Attendance us = new Attendance();

                us.ID = (int)row[1];
                us.MemberID = (int)row[2];
                //us.PhoneNumber = row[3].ToString();
                us.SessionID = (int)row[3];
                us.Status = Convert.ToByte(row[4]);
                data.Add(us);
            }
            return data;
        }
    }
}