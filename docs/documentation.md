# Documentation
Technická dokumentace k projektu **Somnium Prototype**

## Seznam kapitol
- [Documentation](#documentation)
  * [Seznam kapitol](#seznam-kapitol)
  * [Standardy projektu](#standardy-projektu)
    + [Souborová struktura projektu](#souborov--struktura-projektu)
    + [Jmenová konvence](#jmenov--konvence)
      - [Pojmenování složek](#pojmenov-n--slo-ek)
      - [Pojmenování scén a scriptů](#pojmenov-n--sc-n-a-script-)
      - [Pojmenování složek](#pojmenov-n--slo-ek-1)
      - [Pojmenování nodes v rámci scén](#pojmenov-n--nodes-v-r-mci-sc-n)
  * [Herní mechanismy a jejich detailní popisy](#hern--mechanismy-a-jejich-detailn--popisy)
    + [Ukázková herní mechanika](#uk-zkov--hern--mechanika)
      - [Zprovoznění logiky](#zprovozn-n--logiky)
      - [Volané funkce](#volan--funkce)
        * [playerEntered(node: Object, text: String)](#playerentered-node--object--text--string-)
      - [Volatelné funkce](#volateln--funkce)
        * [myFunction(node: Object, text: String) -> void:](#myfunction-node--object--text--string-----void-)
        * [returnString(text: String) -> String:](#returnstring-text--string-----string-)
      - [Příklady](#p--klady)
    + [Logging](#logging)
      - [Logging level](#logging-level)
      - [Config](#config)
      - [Logování do souboru](#logov-n--do-souboru)
      - [Volatelné funkce](#volateln--funkce-1)
        * [info(node: Object, text: String) -> void](#info-node--object--text--string-----void)
        * [warning(node: Object, text: String) -> void](#warning-node--object--text--string-----void)
        * [error(node: Object, text: String) -> void](#error-node--object--text--string-----void)
      - [Ukázka vytvoření logu](#uk-zka-vytvo-en--logu)
      - [Ukázka logu](#uk-zka-logu)
  * [Groups](#groups)
    + [template_group](#template-group)
  * [Collision layers](#collision-layers)
    + [1. vrstva](#1-vrstva)
  * [Autoload](#autoload)
    + [logging](#logging)
    + [CustomSettings](#customsettings)
    + [LastSingleton](#lastsingleton)


*Pro rychlé vygenerování kapitol lze použít [https://ecotrust-canada.github.io/markdown-toc/](https://ecotrust-canada.github.io/markdown-toc/)*

## Standardy projektu
Tato kapitola obsahuje dohodnoté standardy, které zajišťují jednoduchou přehlednost pro všechny členy projektu

### Souborová struktura projektu
**TODO** Zde bude popsána složková struktura projektu.

### Jmenová konvence
**TODO** Tato kapitola obsahuje menší kapitoly jednotlivých jmennových konvencí.

#### Pojmenování složek
**TODO**

#### Pojmenování scén a scriptů
**TODO**

#### Pojmenování složek
**TODO**

#### Pojmenování nodes v rámci scén
**TODO**

## Herní mechanismy a jejich detailní popisy
Tato kapitola obsahuje všechny důležité herní mechanismy

### Ukázková herní mechanika
Toto je ukázková herní mechanika. Díky této mechanice může hráč sebrat předmět do svého inventáře. Díky této mechanice lze provést: Tato kapitola by měla obsahovat detailní popis logiky.
- toto
- tohle taky
  - dokonce i toto
- a ještě tohle

Obrázek ukazující tuto logiku v akci
![Ukázka](https://github.com/AeriftGames/Somnium/blob/develop/docs/img/example_icon.png)

#### Zprovoznění logiky
Toto je ukázková kapitola popisující jak zprovoznit tuto logiku na novém objektu. Pro její použití, je potřeba provést tyto kroky:
1. Nainstacovat core scénu nacházející se v */core_systems/interaction/interaction.tscn*
2. Tato scéna musí být child hlavního objektu, s kterým chceme interaktovat
3. Hlavní objekt musí obsahovat script, který bude mít:

Zde je ukázka snippetu kódu, který je důležité v dokumentaci ukázat. Blok kódu může obsahovat i zvýraznění syntaxe na základě určeného kódu.
```python
import AeriftGames

def my_function(arg1, arg2):
    result = arg1 + " " + arg2
    return result

x = "Hello"
y = "World"
hello_world = my_function(x, y)
print(hello_world)
```
Výsledkem tohoto snippetu je:
```
Hello World
```

#### Volané funkce
Tato kapitola obsahuje veškeré volané funkce této mechaniky.

##### playerEntered(node: Object, text: String)
Tato funkce volá do každé **Area3D**, která zkoliduje s hráčem. Posílá informace o sobě formou **Node** a také posílá nějaký text typu **String**

#### Volatelné funkce
Tato kapitola obsahuje veškeré volatelné (přijimatlné) funkce této mechaniky.

##### myFunction(node: Object, text: String) -> void:
Tato funkce dělá to, že něco dělá. Pro její zavolání je potřeba použít 2 argumenty. Prvním je **node**, který funkci zavolá. A druhý je **string**, který slouží k udělaní *nejaké funkce.*

##### returnString(text: String) -> String:
Tato funkce po zavolání vrátí string, který byl použit při jejím zavolání jako argument.
Pro úspěšné zavolání funkce je potřeba poslat i jeden povinná argument typu **string**. Funkce následně tento string také vrátí.

#### Příklady
Tato kapitola ukazuje nějaký příklad z praxe, jak je funkce využita.

### Logging
Jedná se o logiku, která zlepšuje práci s logováním ať už do souboru, tak i formou printu. Logging je zároveň i **Autoload** nacházející se v `/autoload/logging.gd`. Lze tedy kdykoliv zavolat funkce loggingu pomocí `Logging.funkce()`
Logging si načítá informace ze souboru
`/autoload/settings.cfg`. 
Základním principem je rozdělení logiky podle úrovně logu a kdo loguje.


#### Logging level
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

#### Config
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

#### Logování do souboru
Log do souboru se ukládá v `log/default_log.txt`. Tento soubor je součástí *.gitignore*. Oproti klasickému logu do printu sbírá soubor i základní informace o zařízení, které projekt spustilo. Jde například o věci typu: Operační systém, unikátní ID, CPU, ...

#### Volatelné funkce
Logging obsahuje celkem 3 volatelné funkce. Každá z funkcí vyžaduje 2 parametry:
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje

##### info(node: Object, text: String) -> void
Vytvoří log úrovně **INFO**
Požaduje:
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

##### warning(node: Object, text: String) -> void
Vytvoří log úrovně **WARNING**
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

##### error(node: Object, text: String) -> void
Vytvoří log úrovně **ERROR**
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

#### Ukázka vytvoření logu
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

#### Ukázka logu
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

## Groups
Tato kapitola popisuje veškeré groups využité v projektu

### template_group
Toto je ukázková group. Tuto skupinu obsahují všechny objekty, které budou být moci interaktovány s hráčem.

## Collision layers
Tato kapitola popisuje veškeré kolizní vrstvy v projektu.

### 1. vrstva
Tato vrstva je defaultní a řeší většinu kolizí.

## Autoload
Tato kapitola popisuje všechny autoload.

### logging
Logging by měl být vždy na prvním místě v pořadí Autoloadů. Je to z důvodu toho, aby byl dostupný pro případné autoloady, které by chtěli logovat.
Detailní popis funkčnosti loggingu je popsán [v této kapitole](#logging).

### CustomSettings
Jedná se o autoload, který do svých proměnných ukládá veškeré hodnoty z [configu](#config). Lze si tedy šáhnout na jakoukoliv hodnotu z configu například pomocí:
```python
if CustomSettings.debug_oalar == true:
    do_something()
```

### LastSingleton
Jedná se o pomocný Autoload pro Logging. Jediné co tento autoload dělá je, že nastavít hodnotu 
```
Logging.autoload_complete = true
```
Pomocí této hodnoty logging pozná, že již proběhl veškerý loading. Po načtení se LastSingleton sám uvolní.

### Databases
Tato kapitola popisuje jednotlivé databáze využíté v projektu

#### items
items.json je databáze itemů určená k TODO