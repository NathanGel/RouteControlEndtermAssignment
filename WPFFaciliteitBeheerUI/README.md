# Documentatie Faciliteitbeheer Module

## Overzicht

De Faciliteitbeheer Module is een onderdeel van het netwerkbeheersysteem dat gebruikers in staat stelt om faciliteiten toe te voegen, bij te werken en te verwijderen. De module biedt een gebruiksvriendelijke interface voor het beheren van faciliteitsgegevens die in een SQL Server database worden opgeslagen.

## Functionaliteiten

### Faciliteit Beheer
- **Faciliteiten Bekijken**: Alle beschikbare faciliteiten worden weergegeven in een datagridsysteem.
- **Faciliteiten Toevoegen**: Nieuwe faciliteiten kunnen worden toegevoegd met een naam.
- **Faciliteiten Bijwerken**: Bestaande faciliteiten kunnen worden geselecteerd en bijgewerkt.
- **Faciliteiten Verwijderen**: Geselecteerde faciliteiten kunnen worden verwijderd indien ze niet gekoppeld zijn aan punten.

### Gebruikersinterface
- **DataGrid Weergave**: Faciliteiten worden in een overzichtelijke tabel weergegeven.
- **Dialoogvensters**: Voor het toevoegen en bijwerken van faciliteiten worden specifieke dialoogvensters gebruikt.
- **Aangepaste Venster Bedieningselementen**: De applicatie bevat aangepaste venster bedieningselementen voor minimaliseren, maximaliseren/herstellen en sluiten.

## Systeemarchitectuur

### Gebruikersinterface Componenten
- **MainWindow.xaml**: Het hoofdvenster van de applicatie met de datagridsysteem en bedieningselementen.
- **FacilityWindow.xaml**: Een dialoogvenster voor het toevoegen en bewerken van faciliteitsgegevens.

### Gegevensbeheer
- **NetworkManager**: Behandelt bedrijfslogica voor faciliteitsoperaties.
- **NetworkRepository**: Beheert database-interacties.
- **Mappers**: Converteren tussen UI-modellen (FacilityUI) en domeinmodellen.

### Gegevensopslag
- De applicatie gebruikt een SQL Server database voor permanente opslag.
- Database-verbinding wordt geconfigureerd via de verbindingsreeks in de MainWindow constructor.

## Foutafhandeling

De applicatie implementeert uitgebreide foutafhandeling met specifieke foutmeldingen voor:
- Validatiefouten (bijv. ongeldige faciliteitgegevens)
- Database-verbindingsfouten
- Beperkingsschendingen (bijv. poging tot verwijderen van een faciliteit met koppelingen)
- Onverwachte systeemfouten

## Technische Details

### ObservableCollection
De applicatie gebruikt een ObservableCollection<FacilityUI> om de gebruikersinterface automatisch bij te werken wanneer faciliteitsgegevens veranderen.

### Dialoogvenster Functionaliteit
- **Toevoegen Modus**: Wanneer isUpdate = false, wordt het dialoogvenster gebruikt om nieuwe faciliteiten toe te voegen.
- **Bijwerken Modus**: Wanneer isUpdate = true, wordt het dialoogvenster gebruikt om bestaande faciliteiten bij te werken.
- **Dynamische Titelbalk**: De titel van het dialoogvenster verandert op basis van de geselecteerde modus.

### Venster Interacties
- **Venster Verplaatsen**: De titelbalk kan worden gebruikt om vensters te verplaatsen.
- **Venster Bedieningselementen**: Knoppen voor het minimaliseren, maximaliseren/herstellen en sluiten van vensters.

## Bekende Beperkingen

- Faciliteiten met bestaande koppelingen kunnen niet worden verwijderd vanwege foreign key-beperkingen.
- Bij het verwijderen van faciliteiten wordt geen bevestigingsdialoog getoond.

## Gebruikstips

1. Zorg ervoor dat faciliteiten unieke namen hebben voor betere identificatie.
2. Bij fouten tijdens het toevoegen, bijwerken of verwijderen, controleer de foutmelding voor meer informatie.
3. Selecteer altijd eerst een faciliteit in de datagridsysteem voordat u de knoppen voor bijwerken of verwijderen gebruikt.
4. Controleer of een faciliteit nog in gebruik is voordat u probeert deze te verwijderen.

## Integratie met Netwerkbeheer

De Faciliteitbeheer Module is geïntegreerd met het bredere netwerkbeheersysteem en maakt gebruik van dezelfde onderliggende database en bedrijfslogica. Faciliteiten kunnen worden toegewezen aan netwerkpunten om hun functionaliteit en doel binnen het netwerk aan te duiden.