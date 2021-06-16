using System;

namespace WojnaMrowisk
{
    internal class Menu
    {
        public (int dl, int num) menu()
        {
            int num;
            Console.WriteLine("Wojna Mrowisk");
            Console.WriteLine("Mapa jest kwadratowa. Podaj długość boku (min. 20): ");
            var dl = Convert.ToInt32(Console.ReadLine());
            while (dl < 20)
            {
                Console.WriteLine("Podaj wartość większą niż 10");
                dl = Convert.ToInt32(Console.ReadLine());
            }

            Console.WriteLine("Podaj ilość kolonii");
            if (dl <= 60)
            {
                Console.WriteLine("Ze względu na rozmiar mapy, podaj wartość z zakresu 2-4");
                num = Convert.ToInt32(Console.ReadLine());
                while (num < 2 || num > 4)
                {
                    Console.WriteLine("Podaj wartość z zakresu 2-4");
                    num = Convert.ToInt32(Console.ReadLine());
                }
            }
            else
            {
                if (dl <= 100)
                {
                    Console.WriteLine("Ze względu na rozmiar mapy, podaj wartość z zakresu 2-8");
                    num = Convert.ToInt32(Console.ReadLine());
                    while (num < 2 || num > 8)
                    {
                        Console.WriteLine("Podaj wartość z zakresu 2-8");
                        num = Convert.ToInt32(Console.ReadLine());
                    }
                }
                else
                {
                    Console.WriteLine("Ze względu na rozmiar mapy, podaj wartość z zakresu 2-16");
                    num = Convert.ToInt32(Console.ReadLine());
                    while (num < 2 || num > 16)
                    {
                        Console.WriteLine("Podaj wartość z zakresu 2-16");
                        num = Convert.ToInt32(Console.ReadLine());
                    }
                }
            }

            return (dl, num);
        }
    }
}