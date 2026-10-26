# Vagtplan

En webapplikation til at administrere grundlaget for en vagtplan på et hospital: perioder, afsnit, personalegrupper, brugere med roller og ansættelser. Ud fra det kan man se, hvilket personale der indgår i en valgt periode.

Løsningen består af en React-frontend i TypeScript, et REST API i C# og ASP.NET Core og en SQLite-database. Forretningsreglerne håndhæves i backend, og fejl vises tydeligt for brugeren ved det felt, de handler om.

## Kom i gang

**Krav:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) og [Node.js](https://nodejs.org/) 20 eller nyere. Der skal ikke installeres en database. SQLite-databasen oprettes automatisk med testdata ved første opstart.

**1. Backend:** Åbn `backend/backend.sln` i Visual Studio, vælg **WebAPI** som startup-projekt og start med profilen **https** (F5). Swagger åbner på `https://localhost:7170/swagger`. Får du en fejl om udviklingscertifikatet, så kør `dotnet dev-certs https --trust` én gang.

**2. Frontend:**

```bash
cd frontend
npm install
npm run dev
```

Åbn `http://localhost:5173`. Blokerer PowerShell `npm`, så brug Command Prompt.

**3. Tests:**

```bash
cd backend
dotnet test
```

**Nulstil data:** Stop backend og slet `vagtplan.db` i `backend/src/WebAPI`. Databasen oprettes igen ved næste opstart.

## Arkitektur

Backend er delt i lag (Onion/Clean Architecture), hvor afhængighederne peger ind mod domænet:

```
backend/
├── src/
│   ├── Core/            Entiteter og rene regler (DateRange, HexColor, EmailAddress)
│   ├── Application/     DTO'er, services, validatorer og exceptions
│   ├── Infrastructure/  EF Core, migrationer og testdata
│   └── WebAPI/          Controllere og central fejlhåndtering
└── Tests/
    ├── Unit/            Regler uden database
    └── Integration/     Services mod en SQLite-database i hukommelsen
```

Frontend har én fil pr. side i `frontend/app/routes` og genbrugelige komponenter i `frontend/app/components`.

## Tekniske valg

- **Backend:** C#, ASP.NET Core 8, Entity Framework Core 8 og SQLite. Tests med xUnit
- **Frontend:** React 19, TypeScript, React Router 7 (SPA), Tailwind CSS 4 og Vite
- **SQLite**, så løsningen kan køres uden at installere en database. Al dataadgang går gennem Entity Framework Core, så et skift til SQL Server er én linje og en ny migration
- **API:** Alle ressourcer følger samme REST-mønster, og fejl returneres som [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457)
- **Forretningsregler** håndhæves i backend. Rene regler ligger i `Core/Rules`, fælles feltregler i `NameValidation` og `DateValidation`, og hver entitet har en validator, der også tjekker regler mod databasen. Databasen har constraints som ekstra sikkerhedsnet
- **Fejlbeskeder** er på dansk og vises ved det felt, de handler om
- **Ingen repository-lag.** Testene kører mod en rigtig database, så både regler og forespørgsler testes
- **Vite-proxy** sender kald til `/api` videre til backend under udvikling, så CORS ikke er nødvendigt

## Forretningsregler og tests

| Regel | Håndhæves i | Testet i |
|---|---|---|
| Perioder må ikke overlappe, men må ligge lige efter hinanden | `PeriodValidator` | `PeriodServiceTests` |
| Start skal være før slut | `DateValidation` og CHECK-constraint | `PeriodServiceTests` |
| Overlap med inklusive datoer | `DateRange` | `DateRangeTests` |
| Gyldig HEX-farve | `HexColor` | `HexColorTests`, `DepartmentServiceTests` |
| Afsnit og personalegrupper med ansættelser kan ikke slettes | Services og fremmednøgler | `DepartmentServiceTests`, `StaffGroupServiceTests` |
| Unik e-mail | `UserValidator` og unikt indeks | `UserServiceTests`, `EmailAddressTests` |
| Brugere med ansættelser kan ikke slettes | `UserService` og fremmednøgle | `UserServiceTests` |
| Mindst én rolle | `UserValidator` | `UserServiceTests` |
| Kun personale kan ansættes, timer fra 0 til 37 | `EmploymentValidator` og CHECK-constraint | `EmploymentServiceTests` |
| En persons ansættelser må ikke overlappe | `EmploymentValidator` | `EmploymentServiceTests` |
| Personale vises, hvis ansættelsen overlapper perioden | `StaffService` | `StaffServiceTests` |
