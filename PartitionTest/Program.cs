using System;
using System.Collections.Generic;
using System.Text;
//
// Notes :
//
namespace PartitionTest
{
    class Program
    {
        const int nb = 8;

        /*static int[,] matrix = new int[,]{
            {0,0,0,0,0,0,0,0 },
            {1,0,0,0,0,0,0,0 },
            {0,1,0,0,0,0,0,0 },
            {1,1,1,0,0,0,0,0 },
            {1,0,0,1,0,1,0,0 },
            {1,1,1,1,1,0,0,0 },
            {0,0,0,0,0,1,0,0 },
            {1,0,0,0,0,0,0,0 }
        };*/

        static int[,] matrix = new int[,]{
            {0,1,1,0,1,1,1,1 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,1,0,0,0 },
            {1,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,0,0,0,0,0 },
            {0,0,0,1,0,0,0,1 },
            {0,0,0,0,0,0,0,0 }
        };


        class Relation
        {
            public Element el;
            public int w;

            public Relation(Element e, int n)
            {
                el = e;
                w = n;
            }
        }

        class Element
        {
            public int id;
            public int pos;
            public Dictionary<Element, Relation> Relations;

            public Element( int p1, int p2 )
            {
                id = p1;
                pos = p2;
                Relations = new Dictionary<Element,Relation>();
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("StartMAtrix");
                Console.WriteLine();

                IList<Element> l = MatrixToList(matrix);
                Print(l);
                l = Sort(l);
                Print(l);

                l = Sort(l);
                Print(l);

                l = Sort(l);
                Print(l);

                l = Sort(l);
                Print(l);

                l = Sort(l);
                Print(l);

                l = Sort(l);
                Print(l);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            
            Console.ReadLine();
        }


        static IList<Element> Sort( IList<Element> input )
        {
            IList<Element> output = new List<Element>(nb);

            Element e = null;
 
            int i = 0;
            while( e == null && i < nb )
            {
                if (UpperRelations(i, input) >0)
                {
                    e = input[i];
                }
                else
                {
                    output.Add(input[i]);
                    
                }
                
                i++;
            }

            if (e != null)
            {
                // put found element at bottom
                
                while ( i < nb)
                {
                    output.Add(input[i]);
                    i++;
                }

                output.Add(e);

            }

            
            return output;
           
        }



        //----------------------------------------------------------------

        static IList<Element> MatrixToList(int[,] m)
        {
            IList<Element> al = new List<Element>();

            for (int i = 0; i < nb; i++)
            {
                al.Add(new Element(i, i));
            }

            for (int i = 0; i < nb; i++)
            {
                Element e1 = al[i];
                for( int j = 0; j<nb;j++)
                {
                    Relation r = new Relation(al[j], m[i,j] );
                    e1.Relations.Add(al[j],r);
                }
            }

            return al;
        }

        static int SumD(int i, IList<Element> m)
        {
            int d = 0;
            for (int j = 0; j < nb; j++)
            {
                if ( i != j  && m[i].Relations[m[j]].w > 0 ) d += (i - j);
            }

            return d;
        }

        static double AvgD(int i, IList<Element> m)
        {
            int b = UpperRelations(i, m) + LowerRelations(i,m);

            if (b != 0) return (double)SumD(i,m) / (double)b;
            else return 0;
        }

        static int UpperRelations(int i, IList<Element> m)
        {
            int nbRel = 0;
            for (int j = 0; j < nb; j++)
            {
                if (i < j && m[i].Relations[m[j]].w > 0) nbRel++;
            }

            return nbRel;
        }

        static int LowerRelations(int i, IList<Element> m)
        {
            int nbRel = 0;
            for (int j = 0; j < nb; j++)
            {
                if (i > j && m[i].Relations[m[j]].w > 0) nbRel++;
            }

            return nbRel;
        }
        static void Print( IList<Element> m)
        {
            Console.Write("    ");
            for (int i = 0; i < nb; i++)
            {
                Console.Write("{0}", m[i].id);
            }
            Console.WriteLine("       L   U   S   A");

            for (int i = 0; i < nb; i++)
            {
                Console.Write("{0}:  ", m[i].id);

                for (int j = 0; j < nb; j++)
                {
                    if (i == j)
                        Console.Write('*');
                    else if (m[i].Relations[m[j]].w > 0)
                        Console.Write('X');
                    else
                        Console.Write('.');
                }

                Console.WriteLine("   {0}   {1}   {2}   {3}", 
                    LowerRelations(i, m), UpperRelations( i,m), SumD(i, m), AvgD(i, m));
                //Console.WriteLine();
            }
        }
    }
}
