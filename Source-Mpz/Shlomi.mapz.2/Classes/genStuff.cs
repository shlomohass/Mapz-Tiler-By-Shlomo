using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Data.Common;
using GMap.NET.MapProviders;
using System.Text;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Shlomi.mapz._2
{
   public struct VehicleData
   {
      public int Id;
      public double Lat;
      public double Lng;
      public string Line;
      public string LastStop;
      public string TrackType;
      public string AreaName;
      public string StreetName;
      public string Time;
      public double? Bearing;
   }

   public enum TransportType
   {
      Bus,
      TrolleyBus,
   }

   public struct FlightRadarData
   {
      public string name;
      public PointLatLng point;
      public int bearing;
      public string altitude;
      public string speed;
      public int Id;
   }

   public static class Stuff
   {
      public static bool PingNetwork(string hostNameOrAddress)
      {
         bool pingStatus = false;

         using(Ping p = new Ping())
         {
            byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            int timeout = 4444; // 4s

            try
            {
               PingReply reply = p.Send(hostNameOrAddress, timeout, buffer);
               pingStatus = (reply.Status == IPStatus.Success);
            }
            catch(Exception)
            {
               pingStatus = false;
            }
         }

         return pingStatus;
      }

      static readonly Random r = new Random();

      static string sessionId = string.Empty;

      public static string ReplaceInvalidFileNameChars(string s, string replacement = "")
      {
          return Regex.Replace(s,
            "[" + Regex.Escape(new String(System.IO.Path.GetInvalidPathChars())) + "]",
            replacement,
            RegexOptions.IgnoreCase);
      }

   }

   public static class ReflectionExtensions
   {
       public static bool setPropertyValue(this object Target, string PropertyName, object NewValue)
       {
           if (Target == null) return false; //or throw exception

           System.Reflection.PropertyInfo prop = Target.GetType().GetProperty(PropertyName);

           if (prop == null) return false; //or throw exception

           prop.SetValue(Target, NewValue, null);

           return true;
       }
       public static object getPropertyValue(this object Target, string PropertyName)
       {
           if (Target == null) return null; //or throw exception

           System.Reflection.PropertyInfo prop = Target.GetType().GetProperty(PropertyName);

           if (prop == null) return null; //or throw exception

           return prop.GetValue(Target, null);

       }
   }

   public static class RectParse
   {
       public static RectLatLng parse(GMapPolygon area)
       {
           double UPlat = RectParse.get_upper_lat(area);
           double LElon = get_left_lon(area);
           double LOlat = get_lower_lat(area);
           double RIlon = get_right_lon(area);
           return new RectLatLng(UPlat,LElon,Math.Abs(Math.Abs(RIlon) - Math.Abs(LElon)),Math.Abs(Math.Abs(UPlat) - Math.Abs(LOlat)));
       }
       public static List<SubRect> generate_subs(RectLatLng main, int cols, int rows) 
       {
           List<SubRect> returnSubRects = new List<SubRect>();
           double width = main.Size.WidthLng;
           double height = main.Size.HeightLat;
           double TileWidth = width / (double)cols;
           double TileHeight = height / (double)rows;
           PointLatLng Pointer = main.LocationTopLeft;
           double PointerLng;
           double PointerLat;
           for (int r = 0; r < rows; r++)
           {
               PointerLat = Pointer.Lat - (r * TileHeight);
               for (int c = 0; c < cols; c++)
               {
                   PointerLng = Pointer.Lng + (c * TileWidth);
                   RectLatLng tile = new RectLatLng(new PointLatLng(PointerLat, PointerLng), new SizeLatLng(TileHeight, TileWidth));
                   returnSubRects.Add(new SubRect(tile, c, r));
               }
           }
           return returnSubRects;
       }
       public static double get_upper_lat(GMapPolygon area)
       {
           double returnValue = -2222220.0;

           foreach (PointLatLng point in area.Points)
           {
               if (point.Lat > returnValue)
               {
                   returnValue = point.Lat;
               }
           }
           return returnValue;
       }
       public static double get_lower_lat(GMapPolygon area)
       {
           double returnValue = 2222220.0;
           foreach (PointLatLng point in area.Points)
           {
               if (point.Lat < returnValue)
               {
                   returnValue = point.Lat;
               }
           }
           return returnValue;
       }
       public static double get_left_lon(GMapPolygon area)
       {
           double returnValue = 2222220.0;
           foreach (PointLatLng point in area.Points)
           {
               if (point.Lng < returnValue)
               {
                   returnValue = point.Lng;
               }
           }
           return returnValue;
       }
       public static double get_right_lon(GMapPolygon area)
       {
           double returnValue = -2222220.0;
           foreach (PointLatLng point in area.Points)
           {
               if (point.Lng > returnValue)
               {
                   returnValue = point.Lng;
               }
           }
           return returnValue;
       }
   }

   public struct SubRect
   {
       public RectLatLng subArea;
       public int colIndex;
       public int rowIndex;
       public SubRect(RectLatLng area, int col, int row)
       {
           this.subArea = area;
           this.colIndex = col;
           this.rowIndex = row;
       }
   }
}