public class Gen
{
    public int task { get; init; }
    public int gen { get; set; }

    public Gen(int t, int g)
    {
        task = t;
        gen = g;
    }
}

public class Matrix
{
    public int N { get; set; }
    public int M { get; set; }
    public int T1 { get; set; }
    public int T2 { get; set; }
    public List<List<int>> matrix {get; set;}
    public Matrix(int n, int m, int t1 = 10, int t2 = 20)
    {
        N = n;
        M = m;
        T1 = t1;
        T2 = t2;
        generate_matrix();
    }

    private void generate_matrix()
    {
        var rnd = new Random();
        var matrix_new = new List<List<int>>();
        for(int i = 0; i < M; i++)
        {
            var str_matrix = new List<int>();
            var rnd_number = rnd.Next(T1, T2);
            for (int j = 0; j < N; j++)
            {
                str_matrix.Add(rnd_number);
            }
            matrix_new.Add(str_matrix);
        }
        matrix = matrix_new;
    }

    public void print_matrix()
    {
        foreach(var str in matrix)
        {
            Console.WriteLine(string.Join(",", str));
        }
    }
}

public class Gen_algorith
{
    Random rnd = new Random();
    int N_chr { get; set; }
    int N_lim { get; set; }
    List<List<Gen>> Ch_i { get; set; }
    public List<int> F_Ch_i { get; set; }
    double P_cross { get; set; }
    double P_mutation { get; set; }
    int number_o_generation = 0;


    Matrix matrix { get; init; }


    public Gen_algorith(int n_chr, int n_lim, double p_cross, double p_mutation, Matrix mt)
    {
        //Console.WriteLine("Введите номер числа особей")
        N_chr = n_chr; //Число особей
        N_lim = n_lim;
        P_cross = p_cross;
        P_mutation = p_mutation;
        matrix = mt;
        F_Ch_i = new List<int>();
    }

    private void generate_begin()
    {
        var ch_i = new List<List<Gen>>();
        for (int i = 0; i < N_chr; i++)
        {
            var osob = new List<Gen>();
            for (int j = 0; j < matrix.M; j++)
            {
                osob.Add(new Gen(matrix.matrix[j][0], rnd.Next(0, 256)));
            }
            ch_i.Add(osob);
        }
        Ch_i = ch_i;
        F_Ch_i = get_vector_survive(Ch_i);
    }

    private void print_counting_feno(List<List<int>> feno, List<int> sum_columns)
    {
        Console.WriteLine("Фенотип");
        for (int i = 1; i <= matrix.N; i++)
        {
            Console.Write(i.ToString().PadLeft(3).PadRight(5));
        }
        Console.WriteLine();
        int maxRows = feno.Max(column => column.Count);

        for (int i = 0; i < maxRows; i++)
        {
            foreach (var column in feno)
            {
                if (i < column.Count)
                {
                    string formattedNumber = column[i].ToString().PadLeft(3).PadRight(5);
                    Console.Write(formattedNumber);
                }
                else
                {
                    Console.Write("     ");
                }
            }
            Console.WriteLine();
        }
        //Отделяем строкой
        for (int i = 0; i < matrix.N; i++)
        {
            Console.Write("_____");
            
        }
        Console.WriteLine();
        foreach (var sum in sum_columns) {
            string formattedsum = sum.ToString().PadLeft(3).PadRight(5);
            Console.Write(formattedsum);
        }
        Console.WriteLine();

    }
    private List<int> get_vector_survive(List<List<Gen>> generation)
    {
        List<int> new_F_ch_i = new List<int>(); 
        foreach (var specimen in generation)
        {
            new_F_ch_i.Add(get_feno_one(specimen));
        }
        return new_F_ch_i;
    }

    private int get_feno_one(List<Gen> osob)
    {
        int interval_len = 257 / matrix.N;
        //Для каждой особи генерируем двумерную матрицу фенотипа
        List<List<int>> feno = new List<List<int>>();
        for (int i = 0; i < matrix.N; i++)
        {
            feno.Add(new List<int>());
        }

        foreach (var el in osob)
        {
            int intervalIndex = el.gen / interval_len;
            feno[intervalIndex].Add(el.task);
        }

        List<int> fenotip_results = feno.Select(column => column.Sum()).ToList();
        print_counting_feno(feno, fenotip_results);

        return fenotip_results.Max();
    }

    public void print_feno_all()
    {
        foreach(var osob in Ch_i)
        {
            Console.WriteLine(get_feno_one(osob));
        }
    }

    //При передаче второй родитель можно вычислить как список, из которого удаляем первого родителя, и случайно выбираем второго
    private List<Gen> cross_over(List<Gen> parent_1, List<Gen> parent_2)
    {
        double p_current_cross = rnd.NextDouble();
        List<Gen> outlast_potom = new List<Gen>();
        if (p_current_cross < P_cross)
        {
            Console.WriteLine("Происходит скрещивание");
            //Выбираем точку разделения случайно
            int divide_point = rnd.Next(1, matrix.M);
            //И порождаем два потомка
            List<Gen> potom1 = new List<Gen>();
            List<Gen> potom2 = new List<Gen>();
            for (int i = 0; i < matrix.M; i++)
            {
                if (i < divide_point)
                {
                    potom1.Add(parent_1[i]);
                    potom2.Add(parent_2[i]);
                }
                else
                {
                    potom1.Add(parent_2[i]);
                    potom2.Add(parent_1[i]);
                }
            }
            //Расчитываем какой потомок выживает
            if (get_feno_one(potom1) < get_feno_one(potom2))
            {
                outlast_potom = mutation(potom1);
            }
            else
            {
                outlast_potom = mutation(potom2);
            }
            //И этот потомок перекрывает левого родителя, формируя новое поколение  
        }
        else outlast_potom = parent_1;
        return outlast_potom;
    }

    private List<Gen> mutation(List<Gen> potom)
    {
        List<Gen> new_potom = new List<Gen>(potom);
        double p_current_mutation = rnd.NextDouble();
        if (p_current_mutation < P_mutation)
        {
            Console.WriteLine("Происходит мутация");
            //Выбираем случайный ген
            int gen_idx = rnd.Next(0, matrix.M);
            string binaryString = Convert.ToString(new_potom[gen_idx].gen, 2).PadLeft(8, '0');
            Console.WriteLine(new_potom[gen_idx].gen + " = " + binaryString);
            //ВЫбираем случайную хромосому и инвертируем
            byte number = (byte)new_potom[gen_idx].gen;
            
            int index = rnd.Next(8);
            number ^= (byte)(1 << index);

            Console.WriteLine(number + " = " +Convert.ToString(number, 2).PadLeft(8, '0'));
            new_potom[gen_idx].gen = number;
        }
        return new_potom;
    }

    private List<List<Gen>> generate_new_gen()
    {
        List<List<Gen>> new_generation = new List<List<Gen>>();
        //Условие, что лучший элемент повторился N_lim раз
        for(int i = 0; i < N_chr; i++)
        {
            //Заполнить список, Для выбора родителя для кроссинг овера
            List<List<Gen>> sharp_osob = new List<List<Gen>>();
            for (int j = 0; j < N_chr; j++)
            {
                if (j != i) sharp_osob.Add(Ch_i[j]);
            }

            var idx = rnd.Next(sharp_osob.Count);
            new_generation.Add(cross_over(Ch_i[i], sharp_osob[idx]));
        }
        return new_generation;
    }

    public void main_algorithm()
    {
        generate_begin();
        int counter = 0;
        while(counter < N_lim)
        {
            var new_generation = generate_new_gen();
            var new_F_ch_i = get_vector_survive(new_generation);
            if (F_Ch_i.Min() == new_F_ch_i.Min())
            {
                counter++;
            }
            else
            {
                counter = 0;
            }
            Ch_i = new_generation;
            F_Ch_i = new_F_ch_i;
        }

    }
}
class lab5
{
    public static void Main()
    {
        var test = new Matrix(3, 7);
        test.print_matrix();
        var test1 = new Gen_algorith(5, 3, 0.7, 0.7, test);
        test1.main_algorithm();
    }
}