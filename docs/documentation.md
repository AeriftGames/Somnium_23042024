# Documentation
Technická dokumentace k projektu **Somnium Prototype**

## Seznam kapitol

- [Documentation](#documentation)
  - [Seznam kapitol](#seznam-kapitol)
- [Standardy projektu](#standardy-projektu)
  - [Souborová struktura projektu](#souborov-struktura-projektu)
  - [Jmenová konvence](#jmenov-konvence)
    - [Pojmenování složek](#pojmenovn-sloek)
    - [Pojmenování scén a scriptů](#pojmenovn-scn-a-script)
    - [Pojmenování složek](#pojmenovn-sloek)
    - [Pojmenování nodes v rámci scén](#pojmenovn-nodes-v-rmci-scn)
- [Herní mechanismy a jejich detailní popisy](#hern-mechanismy-a-jejich-detailn-popisy)
  - [Ukázková herní mechanika](#ukzkov-hern-mechanika)
    - [Zprovoznění logiky](#zprovoznn-logiky)
    - [Volané funkce](#volan-funkce)
    - [Volatelné funkce](#volateln-funkce)
    - [Příklady](#pklady)
  - [Logging](#logging)
    - [Logging level](#logging-level)
    - [Config](#config)
    - [Logování do souboru](#logovn-do-souboru)
    - [Volatelné funkce](#volateln-funkce)
    - [Ukázka vytvoření logu](#ukzka-vytvoen-logu)
    - [Ukázka logu](#ukzka-logu)
  - [item_pickup](#item_pickup)
    - [Popis](#popis)
    - [Jak použít](#jak-pout)
- [Groups](#groups)
  - [template_group](#template_group)
- [Collision layers](#collision-layers)
  - [1. vrstva](#1-vrstva)
- [Autoload](#autoload)
  - [logging](#logging)
  - [CustomSettings](#customsettings)
  - [LastSingleton](#lastsingleton)
- [Databases](#databases)
  - [items](#items)

*Pro rychlé vygenerování kapitol lze použít [https://ecotrust-canada.github.io/markdown-toc/](https://ecotrust-canada.github.io/markdown-toc/)*
*Ještě další generator, kde je nastavit úroveň. Současné kapitoly jsou udělané na úroveň **3**. [https://luciopaiva.com/markdown-toc/](https://luciopaiva.com/markdown-toc/)

# Standardy projektu
Tato kapitola obsahuje dohodnoté standardy, které zajišťují jednoduchou přehlednost pro všechny členy projektu

## Souborová struktura projektu
**TODO** Zde bude popsána složková struktura projektu.

## Jmenová konvence
**TODO** Tato kapitola obsahuje menší kapitoly jednotlivých jmennových konvencí.

### Pojmenování složek
**TODO**

### Pojmenování scén a scriptů
**TODO**

### Pojmenování složek
**TODO**

### Pojmenování nodes v rámci scén
**TODO**

# Herní mechanismy a jejich detailní popisy
Tato kapitola obsahuje všechny důležité herní mechanismy

## Ukázková herní mechanika
Toto je ukázková herní mechanika. Díky této mechanice může hráč sebrat předmět do svého inventáře. Díky této mechanice lze provést: Tato kapitola by měla obsahovat detailní popis logiky.
- toto
- tohle taky
  - dokonce i toto
- a ještě tohle

Obrázek ukazující tuto logiku v akci
![Ukázka](https://github.com/AeriftGames/Somnium/blob/develop/docs/img/example_icon.png)

### Zprovoznění logiky
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

### Volané funkce
Tato kapitola obsahuje veškeré volané funkce této mechaniky.

#### playerEntered(node: Object, text: String)
Tato funkce volá do každé **Area3D**, která zkoliduje s hráčem. Posílá informace o sobě formou **Node** a také posílá nějaký text typu **String**

### Volatelné funkce
Tato kapitola obsahuje veškeré volatelné (přijimatlné) funkce této mechaniky.

#### myFunction(node: Object, text: String) -> void:
Tato funkce dělá to, že něco dělá. Pro její zavolání je potřeba použít 2 argumenty. Prvním je **node**, který funkci zavolá. A druhý je **string**, který slouží k udělaní *nejaké funkce.*

#### returnString(text: String) -> String:
Tato funkce po zavolání vrátí string, který byl použit při jejím zavolání jako argument.
Pro úspěšné zavolání funkce je potřeba poslat i jeden povinná argument typu **string**. Funkce následně tento string také vrátí.

### Příklady
Tato kapitola ukazuje nějaký příklad z praxe, jak je funkce využita.

## Logging
Jedná se o logiku, která zlepšuje práci s logováním ať už do souboru, tak i formou printu. Logging je zároveň i **Autoload** nacházející se v `/autoload/logging.gd`. Lze tedy kdykoliv zavolat funkce loggingu pomocí `Logging.funkce()`
Logging si načítá informace ze souboru
`/autoload/settings.cfg`. 
Základním principem je rozdělení logiky podle úrovně logu a kdo loguje.


### Logging level
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

### Config
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

### Logování do souboru
Log do souboru se ukládá v `log/default_log.txt`. Tento soubor je součástí *.gitignore*. Oproti klasickému logu do printu sbírá soubor i základní informace o zařízení, které projekt spustilo. Jde například o věci typu: Operační systém, unikátní ID, CPU, ...

### Volatelné funkce
Logging obsahuje celkem 3 volatelné funkce. Každá z funkcí vyžaduje 2 parametry:
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje

#### info(node: Object, text: String) -> void
Vytvoří log úrovně **INFO**
Požaduje:
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

#### warning(node: Object, text: String) -> void
Vytvoří log úrovně **WARNING**
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

#### error(node: Object, text: String) -> void
Vytvoří log úrovně **ERROR**
`Object` - object, který log zavolal. Tzn. ve většině případů jde o *self*
`String` - text, který se pod poslaným objectem zaloguje
Funkce nic nevrací.

### Ukázka vytvoření logu
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

### Ukázka logu
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

## item_pickup
Item_pickup je **core system** nacházející se v `core_systems\item_pickup`. Item_pickup slouží jako základ pro všechny
itemy, které mají být zvednutelné (pick up) hráčem.

### Popis
Systém je dělán tak, aby jej stačilo jen připojit k itemu, u kterého chceme povolit pick up. A následně nastavit
proměnné pro doladění potřebného chování (popsáno v kapitole) [Jak použít](#Jak-použít). Většina proměnných má
defaultní hodnotu, ale některé no. Pro správné fungování by mělo být všech not null.

### Jak použít
Použití item_pickup je jednoduché. Stačí provést následující kroky:
1. Vytvořit scénu, v které chceme logiku picku provést. Může jít například o scénu vytvořenou rovnou z *.glb* souboru
pomocí `New Inherited scene` (pravé tlačítko myši na *.glb* soubor)
2. K parenté této scény přiřadíme script nacházející se v `core_systems\item_pickup\item_pickup.gd`
3. Po připojení odoznačíme parent a znovu označíme. Tím, se aktualizuje inspector, který bude obsahovat nové proměnné.
Viz. obrázek:
![Ukázka](https://github.com/AeriftGames/Somnium/blob/develop/docs/img/item_pickup_01.PNG)
4. Zde **musíme** nastavit všechny proměnné. Bez nich nebude logika fungovat správně.

| Variable              | Popis                                                                                                        | Typ         | Default value |
|-----------------------|--------------------------------------------------------------------------------------------------------------|-------------|---------------|
| item_interaction_name | Jméno typu akce, které se zobrazí hráči po najetí na item. Mělo by začínat velkým písmenem.                  | String      | Pickup        |
| item_name             | Jméno itemu, které se zobrazí hráči po najetí na item. Mělo by začínat malým písmenem.                       | String      | item          |
| use_node              | Node s připojeným scriptem, který **musí** obsahovat `use()` funkci. Tato funkce je zavolána po použití itemu. | Node        | *null*        |
| pickup_speed          | Rychlost itemu, kterou letí směrem k hráči.                                                                  | float       | 0.2           |
| pickup_height         | Výška, do které letí item (Vypočítáno z pozice playera). 0 je zem, na které hráč stojí.                      | float       | 0.8           |
| hide_speed            | Rychlost, při které se item schová (aby simuloval osebrání objektu)                                          | float       | 0.1           |
| sfx                   | Zvukový efekt, který se přehraje při sebrání itemu. Po dohrání zvuku dojde k `queue_free()`.                 | AudioStream | *null*        |

5. Následně vytvoříme nový node, který bude obsahovat kód s logikou, která se provede po sebrání itemu. Tento node je 
je potřeba vybrat v proměnné **use_node**
6. Use node musí obsahovat funkcí use(), která je zavoláná po sebrání itemu.


# Groups
Tato kapitola popisuje veškeré groups využité v projektu

## template_group
Toto je ukázková group. Tuto skupinu obsahují všechny objekty, které budou být moci interaktovány s hráčem.

# Collision layers
Tato kapitola popisuje veškeré kolizní vrstvy v projektu.

## 1. vrstva
Tato vrstva je defaultní a řeší většinu kolizí.

# Autoload
Tato kapitola popisuje všechny autoload.

## logging
Logging by měl být vždy na prvním místě v pořadí Autoloadů. Je to z důvodu toho, aby byl dostupný pro případné autoloady, které by chtěli logovat.
Detailní popis funkčnosti loggingu je popsán [v této kapitole](#logging).

## CustomSettings
Jedná se o autoload, který do svých proměnných ukládá veškeré hodnoty z [configu](#config). Lze si tedy šáhnout na jakoukoliv hodnotu z configu například pomocí:
```python
if CustomSettings.debug_oalar == true:
    do_something()
```

## LastSingleton
Jedná se o pomocný Autoload pro Logging. Jediné co tento autoload dělá je, že nastavít hodnotu 
```
Logging.autoload_complete = true
```
Pomocí této hodnoty logging pozná, že již proběhl veškerý loading. Po načtení se LastSingleton sám uvolní.

# Databases
Tato kapitola popisuje jednotlivé databáze využíté v projektu

## items
items.json je databáze itemů určená k TODO