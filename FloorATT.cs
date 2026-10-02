using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using 심근우;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using Autodesk.Revit.UI.Selection;

namespace 심근우FA
{
    public class FloorATT
    {
        public static List<FloorData> GetFloorData(Document doc, UIDocument uidoc, string name)
        {
            List<FloorData> floors = new List<FloorData>();

            IList<Reference> refs = uidoc.Selection.PickObjects
               (ObjectType.Face, "구조바닥을 선택하세요.");
            List<IList<CurveLoop>> loops = new List<IList<CurveLoop>>();

            foreach (Reference item in refs)
            {
                FloorData fclass = new FloorData();

                fclass.m_Level = doc.GetElement(item).LevelId;


                Face f = doc.GetElement(item).GetGeometryObjectFromReference(item) as Face;
                EdgeArrayArray EAA = f.EdgeLoops;
                IList<CurveLoop> cls = new List<CurveLoop>();
                foreach (EdgeArray edgeArray in EAA)
                {
                    CurveLoop cl = new CurveLoop();
                    foreach (Edge edge in edgeArray)
                    {
                        Curve c = edge.AsCurveFollowingFace(f);
                        cl.Append(c);
                    }
                    cls.Add(cl);
                }
                fclass.m_CurveLoops = cls;
                FloorType ft = Util.FindTypeByName(doc, name);
                if (ft == null)
                {
                    TaskDialog.Show("오류", name + " 바닥 타입을 찾을 수 없습니다.");
                    return null;
                }
                fclass.m_FloorType = ft.Id;

                Parameter param = ft.get_Parameter
                    (BuiltInParameter.FLOOR_ATTR_DEFAULT_THICKNESS_PARAM);
                if (param == null)
                {
                    TaskDialog.Show("Error", "해당두께의 파라미터를 찾을 수 없습니다.");
                    return null;
                }
                double thickness = param.AsDouble();

                fclass.m_FloorTypeTHK = thickness;
                floors.Add(fclass);
            }

            return floors;
        }
    }

    public class FloorData
    {
        public ElementId m_Level { get; set; }
        public ElementId m_FloorType { get; set; }
        public IList<CurveLoop> m_CurveLoops { get; set; }
        public double m_FloorTypeTHK { get; set; }

    }
    
}
