# Documentatie Netwerkbeheer Applicatie

## Overzicht

De Netwerkbeheer Applicatie is een op WPF gebaseerde desktop applicatie ontworpen voor het visualiseren, creëren en beheren van netwerkinfrastructuur. De applicatie stelt gebruikers in staat om netwerkpunten en verbindingen tussen deze punten op een visueel canvas te creëren, waarbij deze gegevens worden opgeslagen in een SQL Server database.

## Functies

### Netwerkpunt Beheer
- **Netwerkpunten Aanmaken**: Rechtermuisklik op een lege ruimte op het canvas om het contextmenu te openen. Selecteer "Netwerkpunt Toevoegen" om een nieuw punt op de geklikte locatie te creëren.
- **Netwerkpunten Bijwerken**: Selecteer een bestaand punt met een rechtermuisklik en kies "Netwerkpunt Bijwerken" om de eigenschappen te wijzigen. Dit opent een nieuw venster waarin x-coordinaat, y-coordinaat en faciliteiten kunnen worden aangepast.
- **Netwerkpunten Verwijderen**: Selecteer een bestaand punt met rechtermuisklik en kies in het contextmenu "Netwerkpunt Verwijderen". Let op: punten met bestaande verbindingen kunnen niet worden verwijderd.
- **Coördinaten Bekijken**: Klik op een punt om de X- en Y-coördinaten op het canvas te bekijken.

### Verbinding Beheer
- **Verbindingen Aanmaken**: Klik op de knop "Verbinding Toevoegen", selecteer vervolgens achtereenvolgens twee netwerkpunten om een verbinding tussen hen te creëren.
- **Verbindingen Verwijderen**: Klik op de knop "Verbinding Verwijderen", selecteer een beginpunt en kies uit de gemarkeerde punten om de verbinding te verwijderen. Let op: verbindingen die in routes worden gebruikt, kunnen niet worden verwijderd.
- **Verbindingen Bekijken**: Bij het verwijderen van een verbinding worden alle mogelijke verbindingseindpunten vanaf het geselecteerde punt gemarkeerd.

### Visuele Interface
- **Canvas Interactie**: De applicatie biedt een canvas voor het visualiseren van interacties met het netwerk.
- **Punt Markering**: Geselecteerde punten worden rood gemarkeerd.
- **Aangepaste Venster Bedieningselementen**: De applicatie bevat aangepaste venster bedieningselementen voor minimaliseren, maximaliseren/herstellen en sluiten.

## Systeem Architectuur

### Gebruikersinterface Componenten
- **MainWindow.xaml**: Het hoofdvenster van de applicatie met het canvas en de bedieningselementen.
- **NetworkPointWindow**: Een dialoogvenster voor het bewerken van netwerkpunt eigenschappen.

### Gegevensvisualisatie
- **Punten**: Weergegeven als turkooizen cirkels op het canvas.
- **Verbindingen**: Weergegeven als oranjerode lijnen tussen punten.

### Gegevensbeheer
- **NetworkManager**: Behandelt bedrijfslogica voor netwerkoperaties.
- **NetworkRepository**: Beheert database-interacties.
- **Mappers**: Converteren tussen UI-modellen en domeinmodellen.

### Gegevensopslag
- De applicatie gebruikt een SQL Server database voor permanente opslag.
- Database-verbinding wordt geconfigureerd via de verbindingsreeks in de MainWindow constructor.

## Foutafhandeling

De applicatie implementeert uitgebreide foutafhandeling met specifieke foutmeldingen voor:
- Netwerkvalidatiefouten (bijv. ongeldige puntcoördinaten)
- Database-verbindingsfouten
- Beperkingsschendingen (bijv. poging tot verwijderen van een punt met verbindingen)
- Onverwachte systeemfouten

## Technische Details

### Observable Collections
De applicatie gebruikt ObservableCollection<T> voor punten en segmenten om de gebruikersinterface automatisch bij te werken wanneer gegevens veranderen.

### Gebeurtenisafhandeling
- **Points_CollectionChanged**: Behandelt het toevoegen/verwijderen van puntvisualisaties wanneer de puntencollectie verandert.
- **Segments_CollectionChanged**: Behandelt het toevoegen/verwijderen van verbindingsvisualisaties wanneer de segmentencollectie verandert.
- **Muisgebeurtenissen**: Het canvas verwerkt verschillende muisgebeurtenissen om puntselectie, markering en contextmenu-operaties af te handelen.

### Visueel Element Beheer
- **Dictionary Mappings**: De applicatie onderhoudt dictionaries om UI-elementen en gegevensobjecten te koppelen.
- **Canvas Beheer**: Elementen worden dynamisch toegevoegd aan en verwijderd van het canvas op basis van gebruikersinteracties.

## Bekende Beperkingen

- Lijnsegmenten kunnen soms overlappen met puntvisualisaties.
- Bij het bijwerken van de locatie van een punt worden alle verbonden segmenten opnieuw getekend.
- Punten met bestaande verbindingen kunnen niet worden verwijderd vanwege foreign key-beperkingen.

## Gebruikstips

1. Bevestig altijd wijzigingen wanneer daarom wordt gevraagd om gegevensverlies te voorkomen.
2. Voor betere zichtbaarheid, probeer te vermijden punten te dicht bij elkaar te plaatsen.
3. Bij het maken van verbindingen, zorg ervoor dat beide punten worden geselecteerd zoals gevraagd.
4. Als een operatie mislukt, controleer de foutmelding voor specifieke details over het probleem.
5. Gebruik de coördinatenweergave om netwerkpunten nauwkeurig te positioneren.