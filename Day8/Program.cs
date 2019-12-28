using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Day8
{
    class Program
    {
        const int WIDTH = 25;
        const int HEIGTH = 6;

        static void Main(string[] args)
        {
            List<int[,]> image = new List<int[,]>();

            using(var reader = new StreamReader("input.txt"))
            {
                while (reader.Peek() !=  '\n') 
                {
                    image.Add(new int[WIDTH, HEIGTH]);
                    var curLayer = image[image.Count - 1];
                    for(int x = 0 ; x < HEIGTH; x++)
                    {
                        for(int y = 0; y < WIDTH; y++)
                        {
                            var ch = (char)reader.Read();
                            var pixel = (int)Char.GetNumericValue(ch);
                            if(pixel < 0) 
                            {
                                throw new Exception("Invalid image");
                            }
                            curLayer[y, x] = pixel;
                        }
                    }
                }
            }

            var leastZeros = int.MaxValue;
            var ones = 0;
            var twos = 0;

            foreach(var layer in image)
            {
                var groups = layer.Cast<int>().GroupBy( x => x).ToDictionary(v => v.Key, v => v.Count());
                var curZeros = groups[0];
                if(curZeros < leastZeros)
                {
                    leastZeros = curZeros;
                    ones = groups[1];
                    twos = groups[2];
                }
            }

            System.Console.WriteLine("Res: " + ones * twos);

            var decoded = DecodeImage(image, true);
        }

        static int[,] DecodeImage(IEnumerable<int[,]> image, bool saveBitmap = false)
        {
            var decoded = new int[WIDTH, HEIGTH];
            var bitmap = new Bitmap(WIDTH, HEIGTH);

            for(int x = 0 ; x < HEIGTH; x++)
            {
                for(int y = 0; y < WIDTH; y++)
                {
                    foreach(var layer in image)
                    {
                        var pixel = layer[y,x];
                        if(pixel != 2)
                        {
                            decoded[y, x] = pixel;
                            bitmap.SetPixel(y, x, pixel == 0 ? Color.Black : Color.White);
                            break;
                        }
                    }
                }
            }

            if(saveBitmap)
            {
                bitmap.Save("output.bmp");
            }

            return decoded;
        }
    }
}
