using System;
using System.Collections.Generic;

namespace Day3
{
    public class Wires
    {
        public enum Direction
        {
            R,
            L,
            U,
            D,
        }

        public struct Wire
        {
            public Direction direction;
            public int pos;

            public Wire(Direction d, int p)
            {
                direction = d;
                pos = p;
            }
        }

        public struct Pos2
        {
            public int X;
            public int Y;

            public Pos2(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Pos2(Pos2 pos)
            {
                X = pos.X;
                Y = pos.Y;
            }

            public static Pos2 operator -(Pos2 a, Pos2 b) => new Pos2(a.X - b.X, a.Y - b.Y);
        }

        private List<List<Wire>> _wireData;

        int _minX = 0;
        int _maxX = 0;
        int _minY = 0;
        int _maxY = 0;

        bool[,] _grid;

        public Wires(IEnumerable<string> wireStrings)
        {
            _wireData = new List<List<Wire>>();
            foreach(var wireString in wireStrings)
            {
                ParseWire(wireString);
            }
            CalcBounds();
            Trace();
        }

        private void ParseWire(string wire)
        {
            //System.Console.WriteLine(wire);
            var paths = wire.Split(',');
            var wires = new List<Wire>();
            foreach(var path in paths)
            {
                wires.Add(ParsePath(path));
            }
            _wireData.Add(wires);
        }

        private Wire ParsePath(string path)
        {
            //Direction dir = (Direction)path[0];
            Direction dir = (Direction)Enum.Parse(typeof(Direction), path.Substring(0, 1));
            int pos = int.Parse(path.Substring(1));
            return new Wire(dir, pos);
        }

        private void CalcBounds()
        {

            foreach(var wire in _wireData)
            {
                var currentPosition = new Pos2(0, 0);
                foreach(var pos in wire)
                {
                    int val = pos.pos;
                    switch (pos.direction)
                    {
                        case Direction.U:
                            currentPosition.Y += val;
                            break;
                        case Direction.D:
                            currentPosition.Y -= val;
                            break;
                        case Direction.L:
                            currentPosition.X -= val;
                            break;
                        case Direction.R:
                            currentPosition.X += val;
                            break;
                    }
                    if(currentPosition.X < _minX) _minX = currentPosition.X;
                    if(currentPosition.X > _maxX) _maxX = currentPosition.X;
                    if(currentPosition.Y < _minY) _minY = currentPosition.Y;
                    if(currentPosition.Y > _maxY) _maxY = currentPosition.Y;
                }
            }
            //System.Console.WriteLine($"Min: {_minX},{_minY} max: {_maxX},{_maxY}");
        }

        Pos2 _center;
        private void Trace()
        {
            _center = new Pos2(_minX < 0 ? -_minX : 0, _minY < 0 ? -_minY : 0); 
            var size = new Pos2(_maxX - _minX, _maxY - _minY);
            _grid = new bool[size.X+1, size.Y+1];
            //System.Console.WriteLine($"center {_center.X},{_center.Y}");
            //System.Console.WriteLine($"size {size.X},{size.Y}");

            var curPos = new Pos2(_center);
            foreach(var wire in _wireData[0])
            {
                curPos = Draw(curPos, wire);
            }
            var endPos = new Pos2(curPos);

            curPos = new Pos2(_center);
            foreach(var wire in _wireData[1])
            {
                curPos = Check(curPos, wire);
            }

            System.Console.WriteLine(_minDist);
        }

        private Pos2 Draw(Pos2 from, Wire wirePart)
        {
            var pos = new Pos2(from);

            for(int i = 0; i < wirePart.pos; i++)
            {
                switch (wirePart.direction)
                {
                    case Direction.U:
                        pos.Y++;
                        break;
                    case Direction.D:
                        pos.Y--;
                        break;
                    case Direction.L:
                        pos.X--;
                        break;
                    case Direction.R:
                        pos.X++;
                        break;
                }
                if(pos.X < 0 || pos.Y < 0 || pos.X >= _grid.GetLength(0) || pos.Y >= _grid.GetLength(1))
                {
                    System.Console.WriteLine($"Out of bounds! {pos.X},{pos.Y}");
                }
                _grid[pos.X, pos.Y] = true;
            }
            return pos;
        }

        int _minDist = int.MaxValue;

        private Pos2 Check(Pos2 from, Wire wirePart)
        {
            var pos = new Pos2(from);

            for(int i = 0; i < wirePart.pos; i++)
            {
                switch (wirePart.direction)
                {
                    case Direction.U:
                        pos.Y++;
                        break;
                    case Direction.D:
                        pos.Y--;
                        break;
                    case Direction.L:
                        pos.X--;
                        break;
                    case Direction.R:
                        pos.X++;
                        break;
                }
                if(_grid[pos.X, pos.Y])
                {
                    var p = pos - _center;
                    //System.Console.WriteLine($"Cross! {p.X},{p.Y}");
                    var dist = Math.Abs(p.X) + Math.Abs(p.Y);
                    if(dist < _minDist) _minDist = dist;
                }
            }
            return pos;
        }
    }
}