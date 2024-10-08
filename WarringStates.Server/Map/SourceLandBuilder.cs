﻿//using LocalUtilities.TypeGeneral;
//using WarringStates.Server.Map;
//using static WarringStates.Map.SourceLand;

//namespace WarringStates.Map.Terrain;

//internal class SourceLandBuilder
//{
//    public static bool TryBuild(Coordinate site, LandMap landMap, Types targetType, out SourceLand? sourceLand)
//    {
//        sourceLand = null;
//        if (!GetTerrains(site, landMap, out var counts, out var points))
//            return false;
//        if (!CanBuild(counts, targetType))
//            return false;
//        sourceLand = targetType switch
//        {
//            Types.HorseLand => GetSourceLand(site, points, targetType, 1000, 0),
//            Types.MineLand => GetSourceLand(site, points, targetType, 0, 1000),
//            Types.FarmLand => GetSourceLand(site, points, targetType, 1000, 0),
//            Types.MulberryLand => GetSourceLand(site, points, targetType, 0, 1000),
//            Types.WoodLand => GetSourceLand(site, points, targetType, 0, 1000),
//            Types.FishLand => GetSourceLand(site, points, targetType, 1000, 0),
//            Types.TerraceLand => GetSourceLand(site, points, targetType, 500, 500),
//            _ => new()
//        };
//        return true;

//    }

//    private static SourceLand GetSourceLand(Coordinate site, Dictionary<Coordinate, Directions> points, Types type, int foodIncrement, int moneyIncrement)
//    {
//        return new(site, points, type, [
//            new(Product.Types.FoodStuff, 0, foodIncrement),
//            new(Product.Types.Money, 0, moneyIncrement),
//        ]);
//    }

//    private static bool GetTerrains(Coordinate site, LandMap landMap, out Dictionary<SingleLand.Types, int> counts, out Dictionary<Coordinate, Directions> points)
//    {
//        points = [];
//        counts = new()
//        {
//            [SingleLand.Types.Plain] = 0,
//            [SingleLand.Types.Stream] = 0,
//            [SingleLand.Types.Wood] = 0,
//            [SingleLand.Types.Hill] = 0,
//        };
//        var left = site.X - 1;
//        var top = site.Y - 1;
//        var directionOrder = 0;
//        for (var i = 0; i < 3; i++)
//        {
//            for (var j = 0; j < 3; j++)
//            {
//                var point = SetPointWithinTerrainMap(new(left + i, top + j));
//                var land = landMap[point];
//                if (land is not SingleLand singleLand)
//                    return false;
//                points[point] = GetDirectionByOrder(directionOrder++);
//                counts[(SingleLand.Types)singleLand.LandType]++;
//            }
//        }
//        return true;
//        static Directions GetDirectionByOrder(int order)
//        {
//            return order switch
//            {
//                0 => Directions.LeftTop,
//                1 => Directions.Left,
//                2 => Directions.LeftBottom,
//                3 => Directions.Top,
//                4 => Directions.Center,
//                5 => Directions.Bottom,
//                6 => Directions.TopRight,
//                7 => Directions.Right,
//                8 => Directions.BottomRight,
//                _ => Directions.None,
//            };
//        }
//        Coordinate SetPointWithinTerrainMap(Coordinate point)
//        {
//            var x = point.X % landMap.WorldWidth;
//            if (x < 0)
//                x += landMap.WorldWidth;
//            var y = point.Y % landMap.WorldHeight;
//            if (y < 0)
//                y += landMap.WorldHeight;
//            return new(x, y);
//        }
//    }

//    private static bool CanBuild(Dictionary<SingleLand.Types, int> counts, Types targetType)
//    {
//        return targetType switch
//        {
//            Types.HorseLand => counts[SingleLand.Types.Plain] + counts[SingleLand.Types.Stream] is 9,
//            Types.MineLand => counts[SingleLand.Types.Wood] + counts[SingleLand.Types.Hill] is 9,
//            Types.FarmLand => counts[SingleLand.Types.Plain] > 3,
//            Types.MulberryLand => counts[SingleLand.Types.Plain] > 3,
//            Types.WoodLand => counts[SingleLand.Types.Wood] > 3,
//            Types.FishLand => counts[SingleLand.Types.Stream] > 3,
//            Types.TerraceLand => counts[SingleLand.Types.Hill] > 3,
//            _ => false
//        };
//    }
//}
