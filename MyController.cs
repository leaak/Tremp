using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using finalTrempProgect.Models;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Net;
namespace finalTrempProgect.Controllers
{
    public class MyController : Controller
    {
        // GET: /My/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult login(Dictionary<string, string> user)
        {
            string path = Server.MapPath("~/DB/usersTable.xml");

            if (finalTrempProgect.Models.User.isExsistingPassword(user["password"], path))
            {
                return Json(new { success = false, message2 = "הסיסמא קימת עבור משתמש אחר, אנא בחר סיסמא שונה", message1 = "", user = "" });
            }
            else
            {
                User registerUser = new User(path, user["password"], user["email"], user["sms"], user["fullname"]);
                return Json(new { success = true, message = "!!! TREMP נרשמת בהצלחה, ברוכים הבאים ל ", user = registerUser });
            }

        }
        public JsonResult registering(String password)
        {
            string path = Server.MapPath("~/DB/usersTable.xml");
            //Object obj= askTravel(new Dictionary<string, string>() );
            if (finalTrempProgect.Models.User.isExsistingPassword(password, path))
            {
                User U = new User();
                return Json(new { success = true, message = "", user = U.getUser(password, path) });
            }
            else

                return Json(new { success = false, message = "הסיסמא אינה קימת במערכת  ", user = "" });
        }
        public JsonResult askTravel(Dictionary<string, string> askDetails)
        {
            string path = Server.MapPath("~/DB/askTravlesTable.xml");
            string[] toArr = askDetails["valueto"].Split('#');
            string toName = toArr[0];
            string toLat = toArr[1];
            string toLen = toArr[2];
            string[] fromArr = askDetails["valuefrom"].Split('#');
            string fromName = fromArr[0];
            string fromLat = fromArr[1];
            string fromLen = fromArr[2];
            string pass = askDetails["userPass"];
            if (pass.Contains('=') && pass.Contains('"'))
            {
                string[] passSplit = pass.Split('=', '"');
                pass = passSplit[2];
            }
            XDocument x;
            x = XDocument.Load(path);
            XElement ask1 = new XElement("ask");
            ask1.Add(new XAttribute("when", askDetails["when"]));
            ask1.Add(new XAttribute("toName", toName));
            ask1.Add(new XAttribute("toLat", toLat));
            ask1.Add(new XAttribute("toLen", toLen));
            ask1.Add(new XAttribute("fromName", fromName));
            ask1.Add(new XAttribute("fromLat", fromLat));
            ask1.Add(new XAttribute("fromLen", fromLen));
            ask1.Add(new XAttribute("userPass", pass));
            x.Element("askTravels").Add(ask1);
            x.Save(path);
            // החזרת נסיעות מתאימות 
            string str = askDetails["when"].ToString();
            str = str.Split(new Char[] { ' ', 'A', 'P' })[0];
            DateTime askTime = new DateTime();
            askTime = DateTime.ParseExact(str, "dd-MM-yy", null);
            path = Server.MapPath("~/DB/suggestTravelsTable.xml");
            List<Object> suggestList = new List<object>();
            x = XDocument.Load(path);
            foreach (var item in x.Descendants("suggest").ToList())
            {
                DateTime suggTime = new DateTime();
                string time = item.Attribute("when").Value.ToString();
                String[] arrStr=time.Split(new Char[]{' ','A','P'});
                String strT= arrStr[0];
                suggTime = DateTime.ParseExact(strT, "dd-MM-yy", null);
               if (suggTime.Date == askTime.Date )
                {
                    path = Server.MapPath("~/DB/usersTable.xml");
                    Suggest s = new Suggest(path, item.Attribute("userPass").Value.ToString(), item.Attribute("fromName").Value.ToString(), item.Attribute("fromLat").Value.ToString(), item.Attribute("fromLen").Value.ToString(), item.Attribute("toName").Value.ToString(), item.Attribute("toLat").Value.ToString(), item.Attribute("toLen").Value.ToString(), item.Attribute("when").Value.ToString(), "1");
                    path = Server.MapPath("~/DB/suggestTravelsTable.xml");
                    var json = new JavaScriptSerializer().Serialize(s);
                    suggestList.Add(json);
              }
            }


            return Json(new { suggestList = Json(suggestList), askfromLat = fromLat, askfromLen = fromLen, askToLat = toLat, askToLen = toLen });
        }
        public JsonResult suggestTravel(Dictionary<string, string> suggest)
        {
            string path = Server.MapPath("~/DB/suggestTravelsTable.xml");
            string[] toArr = suggest["valuet"].Split('#');
            string toName = toArr[0];
            string toLat = toArr[1];
            string toLen = toArr[2];
            string[] fromArr = suggest["valuef"].Split('#');
            string fromName = fromArr[0];
            string fromLat = fromArr[1];
            string fromLen = fromArr[2];
            string pass = suggest["userPass"];
            if (pass.Contains('=') && pass.Contains('"'))
            {
                string[] passSplit = pass.Split('=', '"');
                pass = passSplit[2];
            }
            XDocument x;
            x = XDocument.Load(path);
            XElement sugg = new XElement("suggest");
            sugg.Add(new XAttribute("when", suggest["when"]));
            sugg.Add(new XAttribute("userPass", pass));
            sugg.Add(new XAttribute("toName", toName));
            sugg.Add(new XAttribute("toLat", toLat));
            sugg.Add(new XAttribute("toLen", toLen));
            sugg.Add(new XAttribute("fromName", fromName));
            sugg.Add(new XAttribute("fromLat", fromLat));
            sugg.Add(new XAttribute("fromLen", fromLen));
            //sugg.Add(new XAttribute("amount", suggest["amount"]));
            x.Element("suggTravels").Add(sugg);
            x.Save(path);
            return Json(new { messageResponse = " הצעתך התקבלה בהצלחה ", messageResponse2 = "טרמפ מודה על תרומתך למערכת " });
        }
        public JsonResult getData(Dictionary<string, string> user)
        {
            if(user.Count>2)
            { 
                string path = Server.MapPath("~/DB/suggestTravelsTable.xml");
                XDocument x;
                x = XDocument.Load(path);
                List<Object> travelstList = new List<object>();
                foreach (var item in x.Descendants("suggest").ToList())
                {
                    if (item.Attribute("userPass").Value == user["Password"])
                    {
                        Travel tr = new Travel(item.Attribute("fromName").Value.ToString(), item.Attribute("toName").Value.ToString(), item.Attribute("when").Value.ToString(), true);
                        var json = new JavaScriptSerializer().Serialize(tr);
                        travelstList.Add(json);
                    }
                }
                path = Server.MapPath("~/DB/askTravlesTable.xml");
                x = XDocument.Load(path);
                foreach (var item in x.Descendants("ask").ToList())
                {
                    if (item.Attribute("userPass").Value == user["Password"])
                    {
                        Travel tr = new Travel(item.Attribute("fromName").Value.ToString(), item.Attribute("toName").Value.ToString(), item.Attribute("when").Value.ToString(), false);
                        var json = new JavaScriptSerializer().Serialize(tr);
                        travelstList.Add(json);
                    }
            }
            return Json(travelstList);
            }
            return null;
        }
        public JsonResult attach(Dictionary<string, string> Travel, Dictionary<string, string> askUser)
        {
            string toPhone;
            User suggestUser = new User();
            string path = Server.MapPath("~/DB/usersTable.xml");
            suggestUser = new User().getUser(Travel["password"].ToString(), path);
            string smsBody = "שלום ";
            smsBody += suggestUser.Name; ;
            smsBody += ", ";
            smsBody += askUser["Name"];
            smsBody += " מעונין/ת להצטרף לנסיעתך מ";
            smsBody += ":";
            smsBody += " ";
            smsBody += Travel["SourceName"].ToString();
            smsBody += " ";
            smsBody += "ל";
            smsBody += ":";
            smsBody += Travel["DestinitionName"].ToString();
            smsBody += " ";
            smsBody += "\n";
            smsBody += " לתאום פרטים ";
            smsBody += "\n";
            smsBody += askUser["Sms"];
            smsBody += "\n";
            smsBody += askUser["Email"];
            smsBody += "\n";
            smsBody += " מערכת טרמפ ";
            smsBody += " מודה על תרומתך ומאחלת נסיעה טובה";
            if (suggestUser.Sms != null)
                toPhone = suggestUser.Sms;
            else
                toPhone = "0545722284";
            
            var proxy = new SmsService.SmsServiceClient();
            var credentials = new SmsService.CustomerCredentials
            {
                UniqueId = "986d835e-4c05-40d9-af73-5ebe664a5362",
            };

            var message = new SmsService.SmsMessage
            {  
                  FromPhone = "0527174210",
                  FromName="TREMP",
                  ToPhone = toPhone,
                  MessageText = smsBody
            };
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            // send the message
            proxy.SendMessage(credentials, message);
           ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";



            string num = suggestUser.Sms;
            string messageResponse2;
            string messageResponse = "  מציע הנסיעה ";
            messageResponse += suggestUser.Name;
            messageResponse += "  קיבל את פניתך  ";
            messageResponse += " לתאום פרטים  ";
            messageResponse2= suggestUser.Sms;
            messageResponse2 += " ";
            messageResponse2 += suggestUser.Email;

            return Json(new { messageResponse = messageResponse, messageResponse2 = messageResponse2 });
        }
       
    }
}
