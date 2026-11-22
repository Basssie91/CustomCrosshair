# Custom Crosshair

Een eigen crosshair over je games heen, voor Windows 10 en 11.

## Downloaden

Ga naar **[Releases](https://github.com/Basssie91/CustomCrosshair/releases/latest)** en download **CustomCrosshair.exe**.
Je hoeft niets te installeren: dubbelklik en hij draait.

Krijg je "Windows heeft uw pc beschermd"? Klik op **Meer info** en daarna op **Toch uitvoeren**.

## Wat kan hij

- 13 crosshair-stijlen met voorbeelden
- Eigen kleur of RGB-modus
- Lengte, dikte, afstand, puntgrootte en positie instellen
- Profielen opslaan en wisselen
- Crosshair-code om je crosshair met vrienden te delen
- Streamproof: onzichtbaar in OBS, Discord-streams en opnames
- Kies op welk scherm de crosshair staat
- Werkt zichzelf automatisch bij als er een nieuwe versie is

## Gebruik

- **Home** opent en sluit het menu (je kunt een andere toets kiezen bij Instellingen).
- Zet je game op **Venster zonder rand** (Borderless), anders zie je de crosshair niet.

## Voor de maker

De code staat in `src/CustomCrosshair.cs`. Verhoog `Version` in de klasse `Updater`
en zet de wijziging op `main`: GitHub bouwt dan automatisch een nieuwe
`CustomCrosshair.exe` en publiceert die als release. Iedereen met de app krijgt
daarna een update-melding.
