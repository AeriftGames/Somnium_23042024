# Logging
Jedná se o logiku, která zlepšuje práci s logováním ať už do souboru, tak i formou printu. Logging je zároveň i **Autoload** nacházející se v `/autoload/logging.gd`. Lze tedy kdykoliv zavolat funkce loggingu pomocí `Logging.funkce()`
Logging si načítá informace ze souboru
`/autoload/settings.cfg`. 
Základním principem je rozdělení logiky podle úrovně logu a kdo loguje.


## Logging level
Logging level lze nastavit pomocí hodnoty `logging_level` v configu.
- **INFO**
Tato úroveň znamená, že se bude logovat vše. Tzn. úroveň **INFO**, **WARNING** i **ERROR**.
Je určeno primárně pro logování událostí, které se staly a jsou pouze **informačního** charakteru
- **WARNING**
Tato úroveň znamená, že se budou logovat úrovně **WARNING** a **ERROR**.
Je určeno pro zalogování něčeho, co se ideálně němělo stát, a nebo nějakého divného chování
- **ERROR**
V této úrovni se bude logovat pouze úroveň **ERROR**.
Je určeno pro zalogování nějakého erroru, který nastal přímo během spuštěného projektu.

## Config
Config se nachází v `/autoload/settings.cfg`. Config je součástí *.gitignore*. 
Config je načítán pomocí Autoloadu `custom_settings.gd`. Tzn. všechny hodnoty z configu jsou dostupné přes zavolání CustomSettings.
```python
if CustomSettings.debug_oalar == true:
    Logging.info(self, "Keypad used")
```

Současná podoba configu je je:
```ini
[debug]
debug_oalar = true
debug_kaen = true

[logging]
; INFO, WARNING, ERROR
logging_level = "INFO"
include_instances = true
include_datetime = false
file_log = false
clear_file = true
```
Vysvětlení jednotlivých hodnot:

| Klíč          | Hodnota        |  Typ          |
| ------------- | ------------- | ------------- |
| debug_oalar | Určeno pro rozdělení logování mezi Oalar a Kaen | bool |
| debug_kaen l  | Určeno pro rozdělení logování mezi Oalar a Kaen  | bool |
| logging_level   | Úroveň logování  | String (INFO, WARNING, ERROR) |
| include_instances  | Zavedení unikátních jmen instancí, které log vytvořily  | bool |
| include_datetime   | Přidání datumu a času k jednotlivým záznamům  | bool |
| file_log   | Logování do souboru  | bool |
| clear_file   | Vyčištění logovacího souboru při spuštění projektu  | bool |

## Logování do souboru
Log do souboru se ukládá v `log/default_log.txt`. Tento soubor je součástí *.gitignore*. Oproti klasickému logu do printu sbírá soubor i základní informace o zařízení, které projekt spustilo. Jde například o věci typu: Operační systém, unikátní ID, CPU, ...

## Volatelné funkce
Logging obsahuje celkem 3 volatelné funkce. Každá z funkcí vyžaduje 2 parametry:
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje

### info(node: Object, text: String) -> void
Vytvoří log úrovně **INFO**
Požaduje:
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

### warning(node: Object, text: String) -> void
Vytvoří log úrovně **WARNING**
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

### error(node: Object, text: String) -> void
Vytvoří log úrovně **ERROR**
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

## Ukázka vytvoření logu
V ukázce vytvoříme log typu **INFO**.
```python
Logging.info(self, "Keypad used")
```
Logging lze zavolat pouze pomocí *logging* jelikož jde o autoload.
Funkce info() slouží k vytvoření záznamu info.
Pošleme v rámci argumentu node hodnotu self. V ukázce log zavolal script keypad.gd
A pošleme informaci o tom, že byl keypad použit.
Výsledkem je vytvoření tohoto logu:
```
INFO: keypad (keypad:<Node3D#38151390688>): Keypad used
```

## Ukázka logu
Ukázka logu ze souboru:
```
----- HARDWARE INFORMATION -----
Running on Windows - 10.0.18363
UID - {da54fe64-1073-11eb-aca2-806e6f6e6963}
CPU name: Intel(R) Core(TM) i7-8700K CPU @ 3.70GHz, CPU count: 12
--------------------------------
INFO: Logging (Logging:<Node#27128758699>): Debugger loaded
INFO: CustomSettings (CustomSettings:<Node#27548189082>): Settings loaded
INFO: CustomSettings (CustomSettings:<Node#27548189082>): Successfully loaded settings.cfg
INFO: keypad (keypad:<Node3D#38285608416>): Keypad used
INFO: 5 (5:<MeshInstance3D#38688261624>): Key 5 pressed
INFO: 6 (6:<MeshInstance3D#38755370492>): Key 6 pressed
INFO: 6 (6:<MeshInstance3D#38755370492>): Key 6 pressed
INFO: 8 (8:<MeshInstance3D#38889588228>): Key 8 pressed
INFO: enter (enter:<MeshInstance3D#39158023700>): Key enter pressed
INFO: keypad (keypad:<Node3D#38285608416>): Incorrect combination entered
```
