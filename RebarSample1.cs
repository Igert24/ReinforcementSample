using System;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;

namespace RebarSample1
{
    /// <summary>
    /// This sample creates two top longitudinal bars (12mm), 
    /// four bottom longitudinal bars (25mm), and stirrups.
    /// </summary>
    class Class1
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Model myModel = new Model();
                var selector = new TSMUI.ModelObjectSelector();
                ModelObjectEnumerator parts = selector.GetSelectedObjects();

                while (parts.MoveNext())
                {
                    Beam myPart = parts.Current as Beam;
                    if (myPart == null) continue;

                    // Store current work plane
                    var wpHandler = myModel.GetWorkPlaneHandler();
                    TransformationPlane originalPlane = wpHandler.GetCurrentTransformationPlane();

                    // Set work plane to part's local coordinate system
                    var localPlane = new TransformationPlane(myPart.GetCoordinateSystem());
                    wpHandler.SetCurrentTransformationPlane(localPlane);

                    // Get part solid for point calculations
                    Solid solid = myPart.GetSolid() as Solid;

                    // --------- Top longitudinal bars (12mm) ---------
                    Action<SingleRebar> initTopBar = bar =>
                    {
                        bar.Father = myPart;
                        bar.Size = "12";
                        bar.Grade = "A500HW";
                        bar.OnPlaneOffsets.Add(0.0);
                        bar.FromPlaneOffset = 0.0;
                        bar.Name = "Top Longitudinal";
                        bar.Class = 7;
                        bar.StartPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
                        bar.StartPointOffsetValue = 25.0;
                        bar.EndPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
                        bar.EndPointOffsetValue = 25.0;
                    };

                    // Upper-left bar
                    var barUL = new SingleRebar(); initTopBar(barUL);
                    barUL.Polygon.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y - 40, solid.MinimumPoint.Z + 40));
                    barUL.Polygon.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y - 40, solid.MinimumPoint.Z + 40));
                    barUL.Insert();

                    // Upper-right bar
                    var barUR = new SingleRebar(); initTopBar(barUR);
                    barUR.Polygon.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y - 40, solid.MaximumPoint.Z - 40));
                    barUR.Polygon.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y - 40, solid.MaximumPoint.Z - 40));
                    barUR.Insert();

                    // --------- Bottom longitudinal bars (4 × 25mm) ---------
                    Action<SingleRebar> initBottomBar = bar =>
                    {
                        bar.Father = myPart;
                        bar.Size = "25";
                        bar.Grade = "A500HW";
                        bar.OnPlaneOffsets.Add(0.0);
                        bar.FromPlaneOffset = 0.0;
                        bar.Name = "Bottom Longitudinal";
                        bar.Class = 7;
                        bar.StartPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
                        bar.StartPointOffsetValue = 25.0;
                        bar.EndPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
                        bar.EndPointOffsetValue = 25.0;
                    };

                    double bottomY = solid.MinimumPoint.Y + 40;
                    // Z positions: close to min Z and max Z, equally spaced for 4 bars
                    double width = solid.MaximumPoint.Z - solid.MinimumPoint.Z;
                    double[] zOffsets = {
                        solid.MinimumPoint.Z + 40,
                        solid.MinimumPoint.Z + (width/3),
                        solid.MinimumPoint.Z + 2*(width/3),
                        solid.MaximumPoint.Z - 40
                    };

                    for (int i = 0; i < 4; ++i)
                    {
                        var botBar = new SingleRebar(); initBottomBar(botBar);
                        botBar.Polygon.Points.Add(new Point(solid.MinimumPoint.X, bottomY, zOffsets[i]));
                        botBar.Polygon.Points.Add(new Point(solid.MaximumPoint.X, bottomY, zOffsets[i]));
                        botBar.Insert();
                    }

                    // --------- Stirrups Group ---------
                    var stirrup = new RebarGroup
                    {
                        Father = myPart,
                        Size = "8",
                        Grade = "A500HW",
                        Name = "Stirrup",
                        Class = 4,
                        FromPlaneOffset = 50,
                        StartPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS,
                        StartPointOffsetValue = 20.0,
                        EndPointOffsetType = Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS,
                        EndPointOffsetValue = 20.0,
                        SpacingType = RebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_TARGET_SPACE,
                    };
                    stirrup.OnPlaneOffsets.Add(20.0);
                    stirrup.RadiusValues.Add(16.0);
                    stirrup.Spacings.Add(250.0);

                    stirrup.StartHook.Angle = 135;
                    stirrup.StartHook.Length = 80;
                    stirrup.StartHook.Radius = 16;
                    stirrup.StartHook.Shape = RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES;
                    stirrup.EndHook.Angle = 135;
                    stirrup.EndHook.Length = 80;
                    stirrup.EndHook.Radius = 16;
                    stirrup.EndHook.Shape = RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES;

                    // Define two stirrup polygons
                    Polygon poly1 = new Polygon();
                    poly1.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z));
                    poly1.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y, solid.MaximumPoint.Z));
                    poly1.Points.Add(new Point(solid.MinimumPoint.X, solid.MinimumPoint.Y, solid.MaximumPoint.Z));
                    poly1.Points.Add(new Point(solid.MinimumPoint.X, solid.MinimumPoint.Y, solid.MinimumPoint.Z));
                    poly1.Points.Add(new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z));

                    Polygon poly2 = new Polygon();
                    poly2.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z));
                    poly2.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y, solid.MaximumPoint.Z));
                    poly2.Points.Add(new Point(solid.MaximumPoint.X, solid.MinimumPoint.Y, solid.MaximumPoint.Z));
                    poly2.Points.Add(new Point(solid.MaximumPoint.X, solid.MinimumPoint.Y, solid.MinimumPoint.Z));
                    poly2.Points.Add(new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z));

                    stirrup.Polygons.Add(poly1);
                    stirrup.Polygons.Add(poly2);
                    stirrup.Insert();

                    // Restore original work plane
                    wpHandler.SetCurrentTransformationPlane(originalPlane);
                }

                myModel.CommitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }
        }
    }
}
