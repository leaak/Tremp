using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace finalTrempProgect.Models
{
    public class Suggest
    {
        #region parameters

        //===========Details========
        public string password { get; set; }
        public String SourceName { get; set; }
        public String SourceLat { get; set; }
        public String SourceLen { get; set; }
        public String DestinitionName { get; set; }
        public String DestinitionLat { get; set; }
        public String DestinitionLen { get; set; }
        public String TravelTime { get; set; }
        public User UserSuggest { get; set; }
        public bool IsConst { get; set; }
        //---------For  const travel--------
        public Boolean[] Days { get; set; }
        public bool IsActive { get; set; }
        //===========Optios========
        public String NumSeats { get; set; }
        public bool AllowWomen { get; set; }
        public bool AllowMen { get; set; }
        #endregion
        #region ctors
        public Suggest(String password, String sourceN, String sourceLa, String sourceLe, String DestinitionN, String DestinitionLa, String DestinitionLe, String TravelTime, String NumSeats)
        {
            this.password = password;
            this.SourceName = sourceN; ;
            this.SourceLat = sourceLa; ;
            this.SourceLen = sourceLe; ;
            this.DestinitionName = DestinitionN;
            this.DestinitionLat = DestinitionLa;
            this.DestinitionLen = DestinitionLe;
            this.TravelTime = TravelTime;
        }
        public Suggest(String path, String password, String sourceN, String sourceLa, String sourceLe, String DestinitionN, String DestinitionLa, String DestinitionLe, String TravelTime, String NumSeats)
        {
            this.password = password;
            this.SourceName = sourceN; ;
            this.SourceLat = sourceLa; ;
            this.SourceLen = sourceLe; ;
            this.DestinitionName = DestinitionN;
            this.DestinitionLat = DestinitionLa;
            this.DestinitionLen = DestinitionLe;
            this.TravelTime = TravelTime;
            this.NumSeats = NumSeats;
            this.UserSuggest = new User();
            UserSuggest = UserSuggest.getUser(this.password, path);
        }
        #endregion
    }
}