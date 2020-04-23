﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace winforms_image_processor
{
    [Serializable]
    class MidPointCircle : Shape
    {
        Point? center = null;
        int? radius = null;

        public MidPointCircle(Color color) : base(color)
        { shapeType = DrawingShape.CIRCLE; }

        public override string ToString()
        {
            return "Circle";
        }

        public override int AddPoint(Point point)
        {
            if (center == null)
                center = point;
            else if (radius == null)
            {
                radius = (int)Math.Sqrt(Math.Pow(point.X - center.Value.X, 2) + Math.Pow(point.Y - center.Value.Y, 2));
                return 1;
            }
            return 0;
        }

        public override List<Point> GetPixels()
        {
            if (!center.HasValue || !radius.HasValue)
                throw new MissingMemberException();

            if (radius.Value == 0)
                return new List<Point>() { center.Value };

            var points = new List<Point>();

            int x = radius.Value, y = 0;
            int P = 1 - x;

            while (x > y)
            {

                y++;

                if (P <= 0)
                    P = P + 2 * y + 1;
                else
                {
                    x--;
                    P = P + 2 * y - 2 * x + 1;
                }

                if (x < y)
                    break;

                points.Add(new Point(x + center.Value.X, y + center.Value.Y));
                points.Add(new Point(-x + center.Value.X, y + center.Value.Y));
                points.Add(new Point(x + center.Value.X, -y + center.Value.Y));
                points.Add(new Point(-x + center.Value.X, -y + center.Value.Y));

                // If the generated point is on the  
                // line x = y then the perimeter points 
                // have already been printed 
                if (x != y)
                {
                    points.Add(new Point(y + center.Value.X, x + center.Value.Y));
                    points.Add(new Point(-y + center.Value.X, x + center.Value.Y));
                    points.Add(new Point(y + center.Value.X, -x + center.Value.Y));
                    points.Add(new Point(-y + center.Value.X, -x + center.Value.Y));
                }
            }

            return points;
        }

        public override List<ValueTuple<Point, Color>> GetPixelsAA(byte[] buffer, int stride)
        {
            var cpDict = new List<ValueTuple<Point, Color>>();

            foreach (var point in GetPixels())
                cpDict.Add(new ValueTuple<Point, Color>(point, shapeColor));

            return cpDict;
        }
    }
}