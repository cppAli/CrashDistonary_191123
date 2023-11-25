using System;
    class ConsoleSpinner
    {
        private int counter;
        private readonly string[] sequence;

        public ConsoleSpinner()
        {
            sequence = new[] { "/", "-", "\\", "|" };
            counter = 0;
        }

        public void Turn()
        {
            counter++;
            int currentIndex = counter % sequence.Length;
            Console.Write($"\r{sequence[currentIndex]} Відкриваю файл...");
        }

        public void Stop()
        {
            Console.Write("\rЗавантаження завершено.     ");
            Console.WriteLine();
        }
    }

