using System;
using System.Collections.Generic;

namespace EllipticCurves
{
    class Program
    {
        static void Main(string[] args)
        {
            Dot dot = new Dot(0, 5585);   //Все точки
            Dot toAdd = new Dot(0, 5585); //Генератор
            int count = 1;
            while (!dot.is0())  //Пока не занулится
            {
                Console.WriteLine(dot.toString()); //Вывод получившейся точки
                dot.sum(toAdd);  //Сложение
                count++;
            }
            Console.WriteLine(dot.toString());  //Вывод последней точки
            Console.WriteLine();
            Console.WriteLine("Количество точек: "+ count);   //Вывод количества точек
            Console.WriteLine();

            //-------El-Ghamal---------------
            int k = 523, m = 10000, c = 5103;
            Dot D = new Dot(12507, 2027);
            if (D.belongToCurve())
            {
                Console.WriteLine("Открытый ключ (точка D{0}) принадлежит кривой", D.toString());   //Если на кривой, то пишет, что на кривой
            }
            KeyValuePair<Dot, int> encrypted = encrypt(m, D, toAdd, k);   //То, что получилось после шифрования - точка R и число е
            int decrypted = decrypt(encrypted.Key, encrypted.Value, c);   //После расшифровки
            Console.WriteLine();
            Console.WriteLine("Сообщение: {0}", m);
            Console.WriteLine("Расшифрованное сообщение: " + decrypted);  //Вывод расшифрованного

        }

        public static KeyValuePair<Dot, int> encrypt(int message, Dot openKey, Dot gen, int k) //шифрование
        {
            Dot R = new Dot(0, 0);
            Dot P = new Dot(0, 0);
            for (int i = 0; i < k; i++)
            {
                R.sum(gen);  //[k]gen   (R - часть шифротекста)
                P.sum(openKey);  //[k]openKey=(x,y)  (openKey - Открытый ключ пользователя В)
            }
            int e = message * (int)P.x % P.getPower();  //сообщение * координату х точки Р % мощность поля(GF)
            return new KeyValuePair<Dot, int>(R, e);
        }
        public static int decrypt(Dot R, int e, int closedKey) //расшифровка
        {
            Dot Q = new Dot(0, 0);
            for (int i = 0; i < closedKey; i++)  //[closedKey]R  (closedKey - секретный ключ пользователя В)
            {
                Q.sum(R);
            }
            return e * Q.inverseElem((int)Q.x) % Q.getPower();  //расшифрованное сообщение = е*обратный элемент по сложению Х % мощность поля
        }

        public class Dot  //описание Точки
        {
            public long x;
            public long y;

            private int gf = 31991;
            private int a = 31988;
            private int b = 1000;

            public Dot(long x, long y)
            {
                this.x = x;
                this.y = y;
            }

            public int getPower() //получить мощность (мощность поля-количество элементов в нем)
            {
                return this.gf;
            }
            public String toString()  //Строчное представление
            {
                return "(" + x + ", " + y + ")";
            }

            public void sum(Dot toAdd)   //Добавление точки
            {
                if (!((toAdd.x == 0) && (toAdd.y == 0)))
                {
                    if ((this.x == 0) && (this.y == 0))
                    {
                        this.x = toAdd.x;
                        this.y = toAdd.y;
                    }
                    else if ((this.x == toAdd.x) && (this.y == gf - toAdd.y))
                    {
                        this.x = 0;
                        this.y = 0;
                    }
                    else
                    {
                        int m, v;  //Формулы из задания
                        if ((this.x != toAdd.x) || (this.y != toAdd.y))
                        {
                            m = inGroup((toAdd.y - this.y) * inverseElem(inGroup(toAdd.x - this.x)));
                            v = inGroup((this.y*toAdd.x - toAdd.y*this.x) * inverseElem(inGroup(toAdd.x - this.x)));
                        }
                        else
                        {
                            m = inGroup((3*this.x*this.x + this.a) * inverseElem(inGroup(2*this.y)));
                            v = inGroup((-this.x*this.x*this.x + this.a*this.x + 2*this.b) * inverseElem(inGroup(2*this.y)));
                        }
                        int resX = inGroup(m * m - this.x - toAdd.x);
                        int resY = inGroup(-m * resX - v);
                        this.x = resX;
                        this.y = resY;
                    }
                }
            }

            public bool is0()    // точка 0
            {
                return ((this.x == 0) && (this.y == 0));
            }

            public bool belongToCurve()  //Проверка принадлежности к кривой
            {
                if (this.is0() || (inGroup(this.x*this.x*this.x + this.a*this.x + this.b) == inGroup((int)Math.Pow(this.y, 2))))
                {
                    return true;
                }
                return false;
            }

            public int inverseElem(int elem)  //Обратный элемент по сложению в группе(твкой элемент, который при сложении с текущим дает еденичный элемент)
            {
                if (elem == 0)
                {
                    return 0;
                }
                int inverted = 2;
                while (inverted * elem % gf != 1)
                {
                    inverted++;
                }
                return inverted;
            }

            private int inGroup(long a)  //Ввод любого, возвращается его представление в группе
            {
                if (a >= gf)
                {
                    return (int)(a % gf);
                }
                if (a < 0)
                {
                    a %= gf;
                    a += gf;
                }
                return (int)a;
            }
        }

    }

}
