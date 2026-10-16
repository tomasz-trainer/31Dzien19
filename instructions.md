# Instrukcje dla agentów – 31Dzien19

Solucja .NET 10 (`31Dzien19.slnx`) – system sklepu internetowego (produkty, kategorie, logowanie JWT) z jednym API i wieloma klientami (WPF, Blazor WASM, MAUI). Projekt dydaktyczny; kod i komentarze są po polsku/angielsku, odpowiadaj po polsku.

## Struktura solucji

| Projekt | SDK / TFM | Rola |
|---|---|---|
| `P03WeatherForecastWPF.Client` | `net10.0-windows`, WPF | Klient desktop (MVVM, CommunityToolkit.Mvvm, DI). Pogoda (Open-Meteo / lokalne API / fake), produkty, logowanie. |
| `P05Shop.API` | `Microsoft.NET.Sdk.Web` | ASP.NET Core Web API (kontrolery), EF Core + SQL Server, JWT. |
| `P06Shop.Shared` | `net10.0` | Biblioteka wspólna: modele (`Product`, `Category`, `User`, DTO auth), `ServiceResponse<T>`, `PagedResult<T>`, interfejsy serwisów i klienckie implementacje HTTP, `IMeesageDialogService`. |
| `P07Shop.DataSeeder` | `net10.0` | Generator danych testowych produktów (Bogus, stały seed `1234`, 100 rekordów), używany w `HasData` w `DataContext`. |
| `P11BlazorWebAssembly.Client` | Blazor WebAssembly | Klient webowy: lista/edycja/tworzenie produktów, logowanie, rejestracja, `CustomAuthStateProvider`, Blazor.LocalStorage. |
| `P12MAUI.Client` | MAUI (android, ios, maccatalyst, windows) | Klient mobilny/desktop (MVVM, CommunityToolkit.Mvvm). Lista i szczegóły produktu. |
| `P13Shop.AppHost` | `Aspire.AppHost.Sdk` | .NET Aspire AppHost: uruchamia `api` oraz `blazor` (z `WaitFor(api)`). |

Zależności: wszystkie klienty, API i seeder -> `P06Shop.Shared`; API -> `P07Shop.DataSeeder`; AppHost -> API + Blazor.

## Architektura

- **Serwisy współdzielone**: interfejsy w `P06Shop.Shared/Services/*` (`IProductService`, `ICategoryService`, `IAuthService`/`IAuthApiClientService`, `IMeteoService`). Klienci używają implementacji HTTP z `P06Shop.Shared`; API ma własne implementacje w `P05Shop.API/Services` (dostęp do bazy przez `DataContext`).
- **Odpowiedzi serwisów** opakowuje `ServiceResponse<T>` (`Success`, `Data`, `Message`); kontrolery mapują `Success` na `Ok`/`NotFound`/`500`.
- **API** (`P05Shop.API`):
  - Kontrolery: `ProductController`, `CategoryController`, `AuthController`, `WeatherMeteoController`, `WeatherForecastController`, `EnvInfoController`. Trasy `api/[controller]`.
  - `DataContext` (`Models/DataContext.cs`): `Products`, `Categories`, `Users`; konfiguracja przez Fluent API (np. `Title` max 100, `Barcode` max 12, `Price` `decimal(8,2)`, unikalna `Category.Name`, FK Product->Category z `SetNull`).
  - DI: `AddScoped` dla serwisów; uwierzytelnianie `JwtBearer` z kluczem `AppSettings:Token`; CORS otwarty (`AllowAnyOrigin`); OpenAPI w Development.
  - Migracje w `P05Shop.API/Migrations` (nie edytować ręcznie, poza koniecznością).
- **Klienci** rejestrują serwisy i `HttpClient` w DI (`App.xaml.cs` w WPF, `MauiProgram.cs` w MAUI, `Program.cs` w Blazor) i używają wzorca MVVM (WPF/MAUI) lub komponentów Razor (Blazor).
- `IMeesageDialogService` (literówka w nazwie jest istniejąca – nie zmieniać bez potrzeby) abstrahuje komunikaty UI; implementacje: `WpfMessageDialogService`, `MauiMessageDialogService`.

## Adresy i konfiguracja

- API: `https://localhost:7127` / `http://localhost:5008` (`P05Shop.API/Properties/launchSettings.json`).
- Adres API jest zahardkodowany w WPF (`App.xaml.cs`) i MAUI (`MauiProgram.cs`); w Blazor pochodzi z `ApiBaseUrl` w `wwwroot/appsettings.Development.json`. Przy zmianie portu zaktualizować wszystkie miejsca.
- Blazor: `https://localhost:7178` / `http://localhost:5048`. AppHost (dashboard): `https://localhost:17002`.
- Baza: SQL Server, connection string `DefaultConnection` w `appsettings.Development.json` (LocalDB `MySuperShop`) i `appsettings.Production.json` (Azure SQL).
- **Bezpieczeństwo**: `appsettings.json` i `appsettings.Production.json` zawierają klucz JWT i dane logowania do bazy. Nie kopiować ich do kodu, logów ani odpowiedzi; nowe sekrety dodawać przez User Secrets / zmienne środowiskowe.

## Komendy

```powershell
# Budowanie (bez MAUI, jeśli brak workloadów)
dotnet build P05Shop.API
dotnet build P11BlazorWebAssembly.Client
dotnet build P03WeatherForecastWPF.Client

# Uruchomienie API + Blazor przez Aspire
dotnet run --project P13Shop.AppHost

# Migracje EF Core (uruchamiać z katalogu solucji)
dotnet ef migrations add <Nazwa> --project P05Shop.API
dotnet ef database update --project P05Shop.API
```

Nie ma projektów testowych.

## Konwencje

- Nowy model/DTO współdzielony przez API i klientów -> `P06Shop.Shared`. Model tylko dla API (np. `CityDto`) -> `P05Shop.API/DTO`.
- Nowy endpoint: kontroler w `P05Shop.API/Controllers`, logika w serwisie (interfejs w `P06Shop.Shared/Services`, implementacja w `P05Shop.API/Services`), rejestracja w `Program.cs`, zwracanie `ServiceResponse<T>`.
- Zmiana modelu EF -> dodać migrację; `ProductDataSeeder` używa stałego seeda, więc zmiana generatora zmienia dane w migracjach.
- Nowa funkcja klienta: dodać metodę do klienckiego serwisu w `P06Shop.Shared` i użyć jej we wszystkich klientach, gdzie ma sens (WPF/Blazor/MAUI).
- Zachowaj istniejący styl (przestrzenie nazw blokowe, `async`/`await`, wstrzykiwanie przez konstruktor). Nie wprowadzaj zmian poza zakresem zadania.
- Znane niedoskonałości (nie naprawiać bez prośby): `Task.Delay(1000).Wait()` w `ProductController.GetProducts`, literówki w nazwach (`IMeesageDialogService`, `WeatherSeervice`), Blazor odwołuje się do przestrzeni `P03WeatherForecastWPF.Client.Services`.
