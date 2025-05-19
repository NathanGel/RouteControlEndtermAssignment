# Route Beheer Applicatie

## Overzicht

De Route Beheer Applicatie is een op WPF gebaseerde desktop applicatie ontworpen voor het visualiseren, creëren en beheren van routes binnen een netwerkinfrastructuur. De applicatie stelt gebruikers in staat om routes te creëren door netwerkpunten te selecteren, deze routes te bewerken en te exporteren naar JSON-bestanden.

## Functies

### Route Visualisatie
- **Netwerkweergave**: Het hoofdcanvas toont alle netwerkpunten (als turkooizen cirkels) en segmenten (als oranje-rode lijnen) uit de database.
- **Route Markering**: Geselecteerde routes worden gevisualiseerd door punten in verschillende kleuren te markeren:
  - **Groen**: Punten die fungeren als stopplaatsen
  - **Rood**: Punten die deel uitmaken van de route maar geen stopplaatsen zijn

### Route Creatie
- **Interactieve Selectie**: Klik op "Route Toevoegen" om de interactieve modus te activeren.
- **Punt Selectie**:
  - **Linkermuisklik**: Selecteert een punt als onderdeel van de route (geen stopplaats)
  - **Rechtermuisklik**: Selecteert een punt als stopplaats
  - Het eerste punt wordt automatisch als stopplaats gemarkeerd
- **Segment Validatie**: De applicatie controleert automatisch of geselecteerde punten verbonden zijn door bestaande segmenten.
- **Bevestiging**: Na selectie verschijnt een "Confirm Selection" knop om de route op te slaan.
- **Naamgeving**: Een dialoogvenster vraagt om een naam voor de nieuwe route.

### Route Beheer
- **Route Overzicht**: Klik op "Routes Beheren" om een lijst van alle beschikbare routes te bekijken.
- **Route Selectie**: Klik op "Route Selecteren" om een specifieke route te kiezen en op het canvas te visualiseren.
- **Route Bewerking**: Vanuit het route beheer venster kunnen individuele routes worden bewerkt in een apart venster.

### Route Detail Venster
Het route detail venster biedt uitgebreide bewerkingsmogelijkheden:

#### Basis Informatie
- **Route ID**: Alleen-lezen identificatie van de route
- **Route Naam**: Bewerkbare naam van de route
- **Totale Afstand**: Automatisch berekende totale afstand van de route

#### Stop Beheer
- **Stops Grid**: Overzicht van alle punten in de route met hun stopplaats-status
- **Stop Toggle**: Klik op de stop-indicator om de stopplaats-status van een punt te wijzigen
- **Punt Verwijdering**: Alleen het eerste of laatste punt van een route kan worden verwijderd
- **Punt Toevoeging**: 
  - *Toevoegen Vooraan*: Voegt een punt toe aan het begin van de route
  - *Toevoegen Achteraan*: Voegt een punt toe aan het einde van de route

#### Faciliteiten Weergave
- **Faciliteiten Lijst**: Toont alle faciliteiten van het geselecteerde punt in de stops grid

#### Persistentie
- **Wijzigingen Opslaan**: Slaat alle aanpassingen op in de database
- **JSON Export**: Exporteert de route naar een JSON-bestand met volledige details

## JSON Export Functionaliteit

De applicatie kan routes exporteren naar JSON-bestanden met de volgende structuur:

```json
{
  "Id": 1,
  "Name": "Route Naam",
  "FullDistance": "15.25 km",
  "Segments": [
    {
      "Id": 1,
      "Distance": "5.5 km",
      "StartPoint": {
        "Id": 1,
        "X": 100,
        "Y": 200,
        "Facilities": [
          {"Id": 1, "Name": "Parkeerplaats"}
        ],
        "IsStop": true
      },
      "EndPoint": {
        "Id": 2,
        "X": 300,
        "Y": 400,
        "Facilities": [],
        "IsStop": false
      }
    }
  ]
}
```

## Systeem Architectuur

### Gebruikersinterface Componenten
- **MainWindow.xaml**: Het hoofdvenster met het netwerkcanvas en route beheer knoppen
- **RouteWindow.xaml**: Detailvenster voor route bewerking
- **SelectRouteDialogWindow**: Dialoog voor route selectie en beheer
- **RouteNameDialogWindow**: Dialoog voor het invoeren van route namen
- **AddPointDialogWindow**: Dialoog voor het toevoegen van punten aan routes

### Gegevensmodellen
- **RouteUI**: UI-model voor routes met segmenten en stops
- **NetworkPointStopsUI**: UI-model voor netwerkpunten met stopplaats-informatie
- **NetworkPoint**: Domeinmodel voor netwerkpunten
- **Segment**: Domeinmodel voor verbindingen tussen punten

### Gegevensbeheer
- **RouteManager**: Behandelt bedrijfslogica voor route-operaties
- **NetworkManager**: Behandelt netwerkgerelateerde operaties
- **RouteRepository**: Beheert database-interacties voor routes
- **NetworkRepository**: Beheert database-interacties voor netwerkstructuur

### Mappers
- **RouteMapper**: Converteert tussen UI-modellen en domeinmodellen voor routes
- **NetworkPointStopsMapper**: Converteert tussen route stops en UI-representatie

## Foutafhandeling

De applicatie implementeert uitgebreide foutafhandeling met specifieke uitzonderingen:

- **RouteException**: Voor route-specifieke validatiefouten
- **ApplicationException**: Voor database en systeem gerelateerde fouten
- **Exception**: Voor onverwachte fouten

Foutmeldingen worden getoond via MessageBox-dialogen met verschillende iconen:
- **Warning**: Voor validatiefouten
- **Error**: Voor systeem- en databasefouten
- **Information**: Voor succesmeldingen

## Gebruikerservaring

### Visuele Feedback
- **Punt Markering**: Verschillende kleuren voor verschillende punt-types
- **Interactieve Elementen**: Hover-effecten en visuele feedback bij muisinteracties
- **Aangepaste Venster Controls**: Geminimaliseer, maximaliseer en sluit knoppen

### Gebruiksflow
1. **Netwerkweergave**: Start met de visualisatie van het volledige netwerk
2. **Route Creatie**: Gebruik interactieve selectie om nieuwe routes te maken
3. **Route Beheer**: Bekijk, bewerk en exporteer bestaande routes
4. **Validatie**: Automatische controle op route-integriteit bij alle operaties

## Technische Details

### Database Integratie
- **SQL Server**: Gebruikt SQL Server database voor permanente opslag
- **Connection String**: Geconfigureerd in MainWindow constructor
- **Entity Framework**: Gebruikt voor database-operaties via repositories

### Observable Collections
- **Real-time Updates**: ObservableCollection zorgt voor automatische UI-updates
- **Data Binding**: WPF data binding voor naadloze UI-synchronisatie

### Bestandsoperaties
- **SaveFileDialog**: Voor JSON export functionaliteit
- **JsonSerializer**: Voor het serialiseren van route-gegevens

## Bekende Beperkingen

> **⚠️ Belangrijk**: Let op de volgende beperkingen bij het gebruik van de applicatie:

- Routes moeten uit aaneengesloten segmenten bestaan
- Alleen het eerste en laatste punt van een route kunnen worden verwijderd
- Segmenten kunnen niet individueel worden verwijderd uit routes
- Route naam moet uniek zijn (validatie in business layer)

## Installatie en Configuratie

### Vereisten
- **.NET Framework/Core** (versie afhankelijk van project configuratie)
- **SQL Server** instance
- **Visual Studio 2019** of hoger voor ontwikkeling

### Database Setup
1. Configureer de connection string in `MainWindow.cs`
2. Zorg ervoor dat de database "NetworkControlTesting" bestaat
3. Voer eventuele migraties uit om de tabellen aan te maken

### Eerste Gebruik
1. Start de applicatie
2. Controleer of netwerkpunten en segmenten correct worden geladen
3. Test de route creatie functionaliteit
4. Verifieer database connectie via route opslaan

---

## Gebruikstips

> **💡 Pro Tips**: Voor optimaal gebruik van de applicatie

1. **Route Planning**: Plan routes vooraf door het netwerk te bekijken
2. **Stopplaats Strategie**: Denk na over welke punten stopplaatsen moeten zijn
3. **Validatie**: Let op foutmeldingen tijdens route creatie
4. **Backup**: Exporteer belangrijke routes naar JSON voor backup
5. **Performance**: Bij grote netwerken kan het laden even duren

## Onderhoud en Updates

### Loggen
- Foutafhandeling via `try-catch` blocks met gebruiksvriendelijke meldingen
- Database operaties zijn gewrapped in transacties waar nodig

### Extensibiliteit
- **Modulaire architectuur** maakt uitbreiding van functionaliteit mogelijk
- **Mapper-pattern** maakt toevoegen van nieuwe UI-modellen eenvoudig
- **Repository-pattern** abstraheert database-operaties

---

## Support

Voor technische problemen:

1. ✅ Controleer database connectie
2. ✅ Verifieer route integriteit in de database
3. ✅ Restart de applicatie bij onverwacht gedrag
4. ✅ Bekijk error logs voor gedetailleerde foutinformatie

---

*Laatste update: [Datum]*
*Versie: 1.0*