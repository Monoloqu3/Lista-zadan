using Newtonsoft.Json;
using System.Text.RegularExpressions;


public class Zadanie
{
    public int Id { get; set; }
    public string Nazwa { get; set; }
    public string Opis { get; set; }
    public DateTime DataZakonczenia { get; set; }
    public bool CzyWykonane { get; set; }

    public Zadanie(int id, string nazwa, string opis, DateTime dataZakonczenia, bool czyWykonane)
    {
        Id = id;
        Nazwa = nazwa;
        Opis = opis;
        DataZakonczenia = dataZakonczenia;
        CzyWykonane = czyWykonane;
    }

    public override string ToString()
    {
        return $"[{Id}] {Nazwa} - {Opis} (Zakończone: {CzyWykonane}, Termin: {DataZakonczenia.ToShortDateString()})";
    }
}


public class ManagerZadan
{
    private List<Zadanie> zadania = new List<Zadanie>();

    public bool CzyIdIstnieje(int id)
    {
        return zadania.Any(z => z.Id == id);
    }

    public void DodajZadanie(Zadanie zadanie)
    {
        zadania.Add(zadanie);
    }

    public void UsunZadanie(int id)
    {
        zadania.RemoveAll(z => z.Id == id);
    }

    public void WyswietlZadania()
    {
        foreach (var zadanie in zadania)
        {
            Console.WriteLine(zadanie);
        }
    }

    public void ZapiszDoPliku(string sciezka)
    {
        string json = JsonConvert.SerializeObject(zadania, Formatting.Indented);

        try
        {
            File.WriteAllText(sciezka, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Błąd: " + ex.Message);
        }
    }

    public void WczytajZPliku(string sciezka)
    {
        string json = File.ReadAllText(sciezka);
        zadania = JsonConvert.DeserializeObject<List<Zadanie>>(json);
    }

    public bool WalidujSciezke(string input, Regex regex)
    {
        while (true)
        {
            if (regex.IsMatch(input))
            {
                return true;
            }
            else
            {
                return false;

            }
        }
    }

}

class Program
{

    static void Main()
    {
        ManagerZadan manager = new ManagerZadan();
        Regex regex = new Regex(@"^([a-zA-Z]:\\)?(?:[\w\s-]+\\)+[\w\s-]*$");
        bool koniec = false;
        string sciezkaZapisu = "";

        while (!koniec)
        {
            Console.WriteLine("\n1. Dodaj zadanie\n2. Usuń zadanie\n3. Wyświetl zadania\n4. Dodaj scieżkę zapisu/odczytu pliku\n5. Zapisz do pliku\n6. Wczytaj z pliku\n7. Wyjdź");
            Console.Write("Wybierz opcję: ");
            int wybor;
            int.TryParse(Console.ReadLine(), out wybor);
            switch (wybor)
            {
                case 1:
                    int id;
                    while (true)
                        {
                        Console.Write("Podaj ID: ");
                        if (!int.TryParse(Console.ReadLine(), out id))
                        {
                            Console.Write("Id musi być liczbą całkowitą!\n");
                            continue;
                        }
                        if (!manager.CzyIdIstnieje(id))
                        {
                            break;
                        }
                        else
                        {
                            Console.Write("Podane ID zadania juz istnieje, czy chcesz je nadpisać? (tak/nie): ");
                            string czy_nadpisac = Console.ReadLine().ToLower();
                            if (czy_nadpisac == "tak")
                            {
                                manager.UsunZadanie(id);
                                break;
                            }
                            else if (czy_nadpisac == "nie")
                            {
                                continue;
                            }
                        }
                    }
                    string nazwa;
                    Console.Write("Nazwa zadania: ");
                    while (true)
                    {
                        nazwa = Console.ReadLine();
                        if (nazwa != "")
                        {
                            break;
                        }
                        else
                        {
                            Console.Write("Nazwa zadania nie może być pusta!\n");
                            Console.Write("Nazwa zadania: ");
                            continue;
                        }
                    }
                    Console.Write("Opis zadania: ");
                    string opis = Console.ReadLine();
                    Console.Write("Data zakończenia (dd/mm/yyyy): ");
                    DateTime dataZakonczenia;
                    while (!DateTime.TryParse(Console.ReadLine(), out dataZakonczenia))
                    {
                        Console.Write("Niepoprawny format daty\n");
                        Console.Write("Data zakończenia (dd/mm/yyyy): ");
                    }
                    Console.Write("Czy wykonane (true/false): ");
                    bool czyWykonane;
                    while (!bool.TryParse(Console.ReadLine(), out czyWykonane))
                    {
                        Console.Write("Niepoprawne dane wejsciowe, Wpisz \"true\" lub \"false\"\n");
                        Console.Write("Czy wykonane (true/false): ");
                    }
                    manager.DodajZadanie(new Zadanie(id, nazwa, opis, dataZakonczenia, czyWykonane));
                    Console.Write("Poprawnie dodano zadanie");
                    break;
                case 2:
                    Console.Write("Podaj ID zadania do usunięcia: ");
                    int idUsuwanie;
                    while (!int.TryParse(Console.ReadLine(), out idUsuwanie))
                    {
                        Console.Write("Id musi być liczbą całkowitą!\n");
                        Console.Write("Podaj ID: ");
                    }
                    manager.UsunZadanie(idUsuwanie);
                    Console.Write($"Poprawnie usunięto zadanie");
                    break;
                case 3:
                    manager.WyswietlZadania();
                    break;
                case 4:
                    Console.Write("Podaj nazwę pliku:\n");
                    string plik = Console.ReadLine();
                    if (plik == "")
                    {
                        Console.Write("Nazwa pliku nie może być pusta!");
                        break;
                    }
                    Console.Write("Dodaj ścieżkę do folderu zapisu/odczytu pliku:\n");
                    string sciezka = Console.ReadLine();
                    if (!manager.WalidujSciezke(sciezka, regex))
                    {
                        Console.WriteLine("Niepoprawna scieżka.");
                        break;
                    }
                    else
                    {
                        sciezkaZapisu = $"{sciezka}\\{plik}.json";
                        Console.Write($"Scieżka do zapisu/odczytu pliku została dodana: {sciezkaZapisu}");
                    }
                    break;
                case 5:
                    if (sciezkaZapisu != "")
                    {
                        manager.ZapiszDoPliku(sciezkaZapisu);
                        Console.Write($"Plik został zapisany w: {sciezkaZapisu}");
                    }
                    else
                    {
                        Console.Write("Sciezka do zapisu pliku nie została zdefiniowana");
                    }
                    break;
                case 6:
                    if (sciezkaZapisu != "")
                    {
                        manager.WczytajZPliku(sciezkaZapisu);
                        Console.Write($"Plik został wczytany z: {sciezkaZapisu}");
                    }
                    else
                    {
                        Console.Write("Sciezka do odczytu pliku nie została zdefiniowana");
                    }
                    break;
                case 7:
                    koniec = true;
                    Console.Write("Zamykam!");
                    break;
                default:
                    Console.WriteLine("Nieznana opcja, spróbuj ponownie.");
                    break;
            }
        }
    }
    
}
