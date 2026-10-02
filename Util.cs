using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System.Data;

namespace 심근우
{
    public class Util
    {
        /// <summary>
        /// 레빗에서 FamilySymbol을 이름으로 검색하여 반환하는 함수
        /// </summary>
        /// <param name="name"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static FamilySymbol GetFamilySymbolByName(String name, Document document)
        {
            FilteredElementCollector collecter = new FilteredElementCollector(document);
            collecter.OfCategory(BuiltInCategory.OST_StructuralFraming);
            collecter.OfClass(typeof(FamilySymbol));

            FamilySymbol fs = null;

            foreach (FamilySymbol item in collecter)
            {
                if (item.Name == name)
                {
                    fs = item;
                    break;
                }
            }

            return fs;
        }
        public static FloorType FindTypeByName(Document doc, string name)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Floors);
            collector.OfClass(typeof(FloorType));

            foreach (FloorType item in collector)
            {
                if (string.Equals(item.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }
        public static void CreateFloor(Document doc, IList<CurveLoop> cl, ElementId floorid, ElementId levelid, double thickness)
        {
            using (Transaction trans = new Transaction(doc, "바닥을 생성합니다."))
            {
                trans.Start();
                Floor f = Floor.Create(doc, cl, floorid, levelid);
                Parameter param = f.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);
                param.Set(thickness);
                trans.Commit();
            }
        }
        public static WallType GetWallTypeByName(Document doc, string name)
        {
            WallType walltype = null;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfCategory(BuiltInCategory.OST_Walls);
            col.OfClass(typeof(WallType));

            foreach (WallType item in col)
            {
                if (item.Name == name)
                {
                    walltype = item;
                    break;
                }
            }
            return walltype;
        }

        public static void CreateWall
            (Document doc, Curve c, WallType wt, Level level, double height, XYZ basisz, bool structural)
        {
            using (Transaction trans = new Transaction(doc, "마감벽을 그립니다."))
            {
                trans.Start();
                Wall.Create(doc, c, wt.Id, level.Id, height, 0, false, structural);
                trans.Commit();
            }
        }

        public static double GetWallWidth(WallType wt)
        {
            Parameter param = wt.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM);
            return param.AsDouble();
        }
        public static CeilingType GetCeilingTypeByName(Document doc, string name)
        {
            CeilingType ceilingtype = null;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfCategory(BuiltInCategory.OST_Ceilings);
            col.OfClass(typeof(CeilingType));


            foreach (CeilingType item in col)
            {
                if (item.Name == name)
                {
                    ceilingtype = item;
                    break;
                }
            }
            return ceilingtype;
        }
        public static Level GetLevelByName(Document doc, string name)
        {
            Level findLevel = null;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(Level));
            foreach (Level item in col)
            {
                if (item.Name == name)
                {
                    findLevel = item;
                    break;
                }
            }
            return findLevel;
        }
    }
}
