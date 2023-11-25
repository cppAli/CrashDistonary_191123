using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;


class Program
{
    static List<string> bannedWords = new List<string>();
    static List<string> textSearch = new List<string>();
    static readonly string bannedWordsFilePath = "CrashWord.txt"; // встроенный словарь

    static void LoadApp()
    {
        Thread.Sleep(3000);
       
    }
    static async Task LoadBannerApp()
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tЗавантаження інтерфейсу");
        await Task.Run(() => LoadApp());

        for (int i = 0; i < 50; i++)
        {
            Console.Write("/");
            await Task.Delay(100); // Задержка 100 миллисекунд
        }
        Console.WriteLine("\n\t\t\t\t\tВІТАЮ!!!");
        await Task.Delay(300); // Задержка 100 миллисекунд
    }

    static async Task Main()
    {
        await LoadBannerApp();
        Console.Clear();

        Console.OutputEncoding = System.Text.Encoding.Unicode;
        Console.WriteLine("їі");
        Console.WriteLine("Перед запуском замены текста открыть все файлы для записи - открыть словарь или добавить текст, \nвторое - открыть текст введите название, \nследующий шаг - запустить редактор");

        while (true)
        {
            Console.WriteLine("\n1. Додати нові слова");
            Console.WriteLine("2. Завантажити заборонені слова з файлу");
            Console.WriteLine("3. Завантажити текст для огляду з файлу");
            Console.WriteLine("4. Замінити заборонені слова та зберегти у новий файл");
            Console.WriteLine("5. Зупинити роботу");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await AddBannedWordsAsync(bannedWordsFilePath);
                    break;
                case "2":
                    await LoadBannedWordsFromFileAsync();
                    break;
                case "3":
                    await LoadTextFromFile();
                    break;
                case "4":
                    await ReplaceBannedWordsAsync();
                    break;
                case "5":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Невірний вибір. Спробуйте знову.\n");
                    break;
            }
        }
    }

    static async Task AddBannedWordsAsync(string bannedWordsFilePath)
    {
        Console.WriteLine("Додати нові слова: Так - 1, Ні - 2");
        string response = Console.ReadLine();

        if (response == "1")
        {
            Console.WriteLine("Введіть заборонені слова, розділені пробілами:");
            string[] wordsInput = Console.ReadLine().Split(' ');
            try
            {
                await OpenBannedWordsAsync(bannedWordsFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла с запрещенными словами: {ex.Message}");
                return;
            }

            // Добавить новые слова к существующему списку
            bannedWords.AddRange(wordsInput);
            // Сохранить обновленный список обратно в файл
            try
            {
                using (StreamWriter sw = new StreamWriter(bannedWordsFilePath))
                {
                    foreach (string word in bannedWords)
                    {
                        await sw.WriteLineAsync(word);
                    }
                }

                Console.WriteLine("Список слів успішно оновлено в файлі.");
                // Вывести обновленный список слов
                Console.WriteLine("\nОновлений список слів:");
                await LoadBannedWordsFromFileAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при збереженні файлу з забороненими словами: {ex.Message}");
            }
            Console.WriteLine("Список слів успішно оновлено в файлі.");
        }
        else
        {
            await LoadBannedWordsFromFileAsync();
        }
    }

    static async Task<List<string>> OpenBannedWordsAsync(string bannedWordsFilePath)
    {
        try
        {
            //bannedWords;
            using (FileStream fs = new FileStream(bannedWordsFilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                bannedWords = new List<string>();
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    bannedWords.Add(line);
                }
            }
            return bannedWords;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла с запрещенными словами: {ex.Message}");
            return null; // Можно также использовать Task.FromResult<List<string>>(null);
        }
    }

    static async Task LoadBannedWordsFromFileAsync()
    {
        ConsoleSpinner spinner = new ConsoleSpinner();

        Console.WriteLine("\nЗавантаження словника <Crash Word>:");

        bannedWords = await Task.Run(() => OpenBannedWordsAsync(bannedWordsFilePath));

        if (bannedWords != null)
        {
            spinner.Turn();
            await Task.Delay(5000);
        }
        Console.WriteLine("\n");
        foreach (string line in bannedWords)
        {
            // Обработка каждой строки файла
            Console.WriteLine(line);
        }
        spinner.Stop();
    }

    static Task LoadTextFromFile()
    {
        Console.WriteLine("Введіть назву файлу, що підлягає редагуванню:\n");
        string filePath = Console.ReadLine();
        try
        {
            textSearch = new List<string>(File.ReadAllLines(filePath));
            foreach (string line in textSearch)
            {
                // Обработка каждой строки файла
                Console.WriteLine(line);
            }
            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при завантаженні: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    static async Task<Task> ReplaceBannedWordsAsync()
    {
        ConsoleSpinner spinner = new ConsoleSpinner();
        await OpenBannedWordsAsync(bannedWordsFilePath);

        if (textSearch == null || textSearch.Count == 0)
        {
            Console.WriteLine("Нет текста для обработки.");
            return Task.CompletedTask;
        }
        else
        {
            spinner.Turn();
            await Task.Delay(5000);
        }
        Console.WriteLine("\n");
        List<string> modifiedText = new List<string>();

        foreach (string line in textSearch)
        {
            string modifiedLine = line;

            foreach (string bannedWord in bannedWords)
            {
                int index = modifiedLine.IndexOf(bannedWord, StringComparison.OrdinalIgnoreCase);

                while (index != -1)
                {
                    modifiedLine = modifiedLine.Remove(index, bannedWord.Length).Insert(index, "****");

                    index = modifiedLine.IndexOf(bannedWord, index + 4, StringComparison.OrdinalIgnoreCase);
                }
            }

            modifiedText.Add(modifiedLine);
        }
       
        // Save modified text to a new file
        try
        {
            File.WriteAllLines("ModifiedText.txt", modifiedText);
            Console.WriteLine("Заборонені слова замінено та збережено у новий файл 'ModifiedText.txt'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при збереженні файлу: {ex.Message}");
        }

        modifiedText = new List<string>(File.ReadAllLines("ModifiedText.txt").ToList());
        foreach (string line in modifiedText)
        {
            // Обработка каждой строки файла
            Console.WriteLine(line);
        }
        Console.WriteLine("\n");
        spinner.Stop();
        return Task.CompletedTask;
    }
}

