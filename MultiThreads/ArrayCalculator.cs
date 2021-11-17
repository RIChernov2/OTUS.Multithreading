using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreads
{
    public class ArrayCalculator
    {
        private readonly int[] _array;
        private bool _calculated = false;
        private Stopwatch  _stopwatch;
        private const int _quantityOfMeasures = 5;
        private const int _numberOfCalculationTypes = 3;
        private const int _coresCount = 4;
        private double[] _resultValues = new double[_numberOfCalculationTypes];
        private TimeSpan[][] _resultTimes = new TimeSpan[_numberOfCalculationTypes][];


        public ArrayCalculator(int[] array)
        {
            _array = array;
            _stopwatch = new Stopwatch();
        }

        public void Calulate()
        {

            TimeSpan[] span;
            int calculationNumber = 1;

            // последовательное вычисление          
            int calculationNumber1 = calculationNumber++;
            double result1 = 0;
            span = new TimeSpan[_quantityOfMeasures + 2];

            for ( int i = 0 ; i < span.Length - 1 ; i++ )
            {
                _stopwatch.Start();
                result1 = GetSum();
                _stopwatch.Stop();
                span[i] = _stopwatch.Elapsed;

                // игнорируе первый "прогревочный расчет", из остальных ищем среднее значение
                if( i != 0) span[span.Length - 1] += span[i];

                _stopwatch.Reset();
            }

            span[span.Length - 1] /= _quantityOfMeasures;
            _resultTimes[calculationNumber1 - 1] = span;
            _resultValues[calculationNumber1 - 1] = result1;



            // параллельное вычисление
            int calculationNumber2 = calculationNumber++;
            double result2 = 0;
            span = new TimeSpan[_quantityOfMeasures + 2];

            for ( int i = 0 ; i < span.Length - 1 ; i++ )
            {
                _stopwatch.Start();
                result2 = GetParallelSum(_coresCount*2);
                _stopwatch.Stop();
                span[i] = _stopwatch.Elapsed;

                // игнорируе первый "прогревочный расчет", из остальных ищем среднее значение
                if ( i != 0 ) span[span.Length - 1] += span[i];

                _stopwatch.Reset();
            }

            span[span.Length - 1] /= _quantityOfMeasures;
            _resultTimes[calculationNumber2 - 1] = span;
            _resultValues[calculationNumber2 - 1] = result2;


            // LINQP вычисление
            int calculationNumber3 = calculationNumber++;
            double result3 = 0;
            span = new TimeSpan[_quantityOfMeasures + 2];

            for ( int i = 0 ; i < span.Length - 1 ; i++ )
            {
                _stopwatch.Start();
                result3 = GetLINQPSum();
                _stopwatch.Stop();
                span[i] = _stopwatch.Elapsed;

                // игнорируе первый "прогревочный расчет", из остальных ищем среднее значение
                if ( i != 0 ) span[span.Length - 1] += span[i];

                _stopwatch.Reset();
            }

            span[span.Length - 1] /= _quantityOfMeasures;
            _resultTimes[calculationNumber3 - 1] = span;
            _resultValues[calculationNumber3 - 1] = result3;


            _calculated = true;
        }



        public void Print()
        {

            if ( !_calculated )
            {
                Console.WriteLine("Данные по работе с массивом не были посчитаны.\n" +
                                                    "Необходимо вызвать метод Calulate().");
                return;
            }

            if( _resultValues.Sum() / _resultValues.Length != _resultValues[0] )
            {
                Console.WriteLine("Ошибка, значения вычислений разными методами не равны.");
                for ( int i = 0 ; i < _resultValues.Length ; i++ )
                {
                    Console.WriteLine($"{i}. => {_resultValues[i]}");
                }
                return;
            }
            Console.WriteLine("///////////////////////////////////////////////////////////////////////");
            Console.WriteLine("=======================================================================");
            Console.WriteLine($"     Массив содержит {_array.Length} элементов.");
            Console.WriteLine("=======================================================================");
            Console.WriteLine("///////////////////////////////////////////////////////////////////////");
            Console.WriteLine(Environment.NewLine);

            int calculationNumber;


            // последователбное
            calculationNumber = 0;

            Console.WriteLine($"Последовательное вычисление суммы дало следующие результаты:");
            for ( int i = 1 ; i < _resultTimes[calculationNumber].Length - 1 ; i++ )
            {
                Console.WriteLine($"  {i}. {_resultTimes[calculationNumber][i].TotalMilliseconds} мс.");
            }
            Console.WriteLine($"Среднее значение {_resultTimes[calculationNumber].Last().TotalMilliseconds} мс" + 
                $" ({_resultTimes[calculationNumber].Last().Ticks} тиков)");

            Console.WriteLine(Environment.NewLine);



            // Параллельное
            calculationNumber = 1;

            Console.WriteLine($"Параллельное вычисление суммы дало следующие результаты:");
            for ( int i = 1 ; i < _resultTimes[calculationNumber].Length - 1 ; i++ )
            {
                Console.WriteLine($"  {i}. {_resultTimes[calculationNumber][i].TotalMilliseconds} мс.");
            }
            Console.WriteLine($"Среднее значение {_resultTimes[calculationNumber].Last().TotalMilliseconds} мс" +
                $" ({_resultTimes[calculationNumber].Last().Ticks} тиков)");

            Console.WriteLine(Environment.NewLine);


            // LINQP
            calculationNumber = 2;

            Console.WriteLine($"Вычисление суммы с помошью LINQP дало следующие результаты:");
            for ( int i = 1 ; i < _resultTimes[calculationNumber].Length - 1 ; i++ )
            {
                Console.WriteLine($"  {i}. {_resultTimes[calculationNumber][i].TotalMilliseconds} мс.");
            }
            Console.WriteLine($"Среднее значение {_resultTimes[calculationNumber].Last().TotalMilliseconds} мс" +
                $" ({_resultTimes[calculationNumber].Last().Ticks} тиков)");

            Console.WriteLine(Environment.NewLine);
        }

        private double GetSum() => GetSum(_array, 0, _array.Length);
        private double GetSum(int[] array, int startIndex, int endIndex)
        {
            double result = 0;
            for ( int i = startIndex ; i < endIndex ; i++ )
            {
                result += array[i];
            }
            return result;
        }
        private double GetParallelSum(int threadsNumber)
        {
            int partsSize = _array.Length / threadsNumber;
            Task[] resultTastks = new Task[threadsNumber];
            double[] results = new double[threadsNumber];
            for ( int i = 0 ; i < threadsNumber ; i++ )
            {
                int j = i;
                int startIndex = j * partsSize;
                int endIndex = j == threadsNumber - 1 ? _array.Length : (j + 1) * partsSize;
                resultTastks[j] = Task.Run(() => results[j] = GetSum(_array, startIndex, endIndex));
            }
            Task.WaitAll(resultTastks);
            return results.Sum();
        }

        //private double GetLINQPSum() => _array.AsParallel().Select(x => (double)x).Sum();
        //private double GetLINQPSum() => _array.AsParallel().Sum(x => x / 1.0);
        private double GetLINQPSum() => _array.AsParallel().Select(Convert.ToDouble).Sum();

        //private double GetLINQPSum() => _array.Select(x => (double)x).AsParallel().Sum();
        //private double GetLINQPSum() =>_array.AsParallel().Select(Convert.ToDouble).Sum();
        //private double GetLINQPSum() => _array.ToArray().AsParallel().Sum<double>();
        //private double GetLINQPSum() => _array.ToArray().AsParallel().Cast<double>().Sum<double>();
        //private double GetLINQPSum() => _array.AsParallel().Sum(x => x / 1.0);
        //private double GetLINQPSum() => _array.Select(x => (double)x).AsParallel().Sum();
        //private double GetLINQPSum() =>_array.AsParallel().Select(x => (double)x).Sum();
        //private double GetLINQPSum() =>_array.AsParallel().Sum();

    }
}
