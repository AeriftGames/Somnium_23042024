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
    + [MessageObject](#messageobject)
  * [Groups](#groups)
    + [template_group](#template-group)
  * [Collision layers](#collision-layers)
    + [1. vrstva](#1-vrstva)
  * [Autoload](#autoload)
    + [logging](#logging)
    + [CustomSettings](#customsettings)
    + [LastSingleton](#lastsingleton)
  * [Databases](#databases)
    + [items](#items)


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

### **MessageObject**
Jedná se o classu která ulehčuje obejít nepříjemné chování Godot 4 (které nejspíš bude časem s novými verzemi Godot enginu vyřešeno) Aktuálně problém je v tom, že nemůžeme předat skrze argumenty ve funci jakákoliv data z **CSharp scriptu** do **GD scriptu** a naopak a to ani vracet funkcí nějaký datový typ. Godot totiž řeší veškeré datové konverze argumentů mezi jednotlivými ```Godot.Object``` přes svůj typ ```Variant```. Typ Variant umí pracovat s velkým počtem používaných typů a dělá tedy jejich konverzi (například int nebo string na Variant) Když se tedy například pokoušíme přenést skrze argumenty funkce data, začne se plnit neznámý buffer v cashe paměti (kterou využívá Variant) a to nejšpíš proto, že se po přenosu neuvolní data z této části paměti (neznámý buffer). Program se po chvílí přehlcuje a i engine přestává správně fungovat, **nastávají úniky paměti** a začíná padat jak náš program, tak samotný engine. Proto je tedy MessageObject, který slouží jako jiná cesta komunikace, než by jsme v tomto případě jinak nativně používali.

#### **Ukázka základního použití MessageObject**
Dejme tomu že máme takovou scénu v Godotu a chceme komunikovat mezi těmito nodes i s možností předávat si různá data:
- WorldScene
  - **NodeA_GD** (Node3D obsahující GD script)
  - **NodeB_CS** (Node3D obsahující CS script)

**NodeA_GD** (GD script)
```python
extends Node3D

# objekt s kterym chceme komunikovat a ma v sobe msgObject
var NodeB

func _ready():
    NodeB = get_node("/root/WorldScene/NodeB_CS")
    # posle zpravu ktera z kontextu chce vratit string nejakeho jmena
    NodeB.msgObject.SendMessageToCSNow("msg_get_yourname")
    print(NodeB.msgObject.GetStringData())

# toto je funkce volana pouze z protejsiho komunikacniho objektu CS (neco jako hej mas zpravu, ted si ji precti)
func message_update():
	# cteme a filtrujeme prichozi povahu message z NodeB_CS
	var msg:String = NodeB.msgObject.GetMessage()
	if(msg == "msg_write_text"):
		print(NodeB.msgObject.GetStringData())
```

**NodeB_CS** (CS script)
```csharp
using Godot;
using System;

public partial class NodeB_CS : Node3D
{
  // nas MessageObject
  public MessageObject msgObject;
  
	public override void _Ready()
	{
      	// vytvoreni noveho MessageObjectu
     	 msgObject = new MessageObject(this,GetNode<Node3D>("/root/WorldScene/NodeA_GD"));
      
      	// nastavime data(jako argument) a posilame NodeA_GD zpravu (msg/kontext)
      	msgObject.SetStringData("hello");
      	msgObject.SendMessageToGDNow("msg_write_text");
      	// SendMessageToGDNow spusti v protejsim objektu s kterym komunikujeme funkci message_update
	}

	// toto je funkce volana pouze z protejsiho komunikacniho objektu GD (neco jako hej mas zpravu, ted si ji precti)
	public void message_update()
	{
		// cteme a filtrujeme prichozi povahu message
		string msg = msgObject.GetMessage();
		switch (msg)
		{
            		case "msg_get_yourname":
                	{
                    	msgObject.SetStringData("Michael");
                    	break;
                	}
        	}
	}
}
```

#### **Vyvětlení kroků v kódu:**
Vytvoření ```msgObject``` musí být vždy na straně CS scriptu a kvůli domluvě by se měl tento objekt vždy jmenovat msgObject

**CS SCRIPT**
```csharp
// Vytvorime novy message object ktery bude komunikovat s jinym objektem s GD scriptem
// Prvni parametr je odkaz na tuhle instanci objektu
// Druhy parametr je instance objektu s kterym chceme komunikovat
msgObject = new MessageObject(this,GetNode<Node3D>("/root/WorldScene/NodeA_GD"));
```
Pošleme zprávu a text jako argument **(první se musi nastavit data co chceme předat a až poté poslat zprávu)**
```csharp
// nastavime data(jako argument) a posilame NodeA_GD zpravu (msg/kontext)
msgObject.SetStringData("hello");
msgObject.SendMessageToGDNow("msg_write_text");
```
v tu chvíli kde končí předchozí kód ```SendMessageToGDNow``` se přesune kód do GD scriptu a zavolá v něm ```message_update()```

**GD SCRIPT**

teď se spustí message_update, kde si objekt přečte zprávy
```python
# toto je funkce volana pouze z protejsiho komunikacniho objektu CS (neco jako hej mas zpravu, ted si ji precti)
func message_update():
	# cteme a filtrujeme prichozi povahu message z NodeB_CS
	var msg:String = NodeB.msgObject.GetMessage()
	if(msg == "msg_write_text"):
		print(NodeB.msgObject.GetStringData())
```
přečteme si zprávu a pokud se shoduje, tak následným kódem zareagujeme (jelikož jsme v **CS Objektu** před odesláním zprávy nastavili ```SetStringData("hello")``` **viz výše**.) můžeme si poslední uložená data lehce převzít, je tam uložen string ```"hello"```, který pomocí printu vypíšeme
```python
if(msg == "msg_write_text"):
    print(NodeB.msgObject.GetStringData())
```
**Veškerá komunikace funguje i obráceně** a je zanesena v ukázkovém kódu výše

**GD SCRIPT** 

Pošle zprávu ```SendMessageToCSNow()``` do **CS Scriptu** a bude chít přijímat data string
```python
func _ready():
    NodeB = get_node("/root/WorldScene/NodeB_CS")
    # posle zpravu druhemu objektu a ktera z kontextu chce vratit string nejakeho jmena
    NodeB.msgObject.SendMessageToCSNow("msg_get_yourname")
    print(NodeB.msgObject.GetStringData())
```
**CS SCRIPT** 

Hned poté co poslal zprávu **GD Script** skrze ```SendMessageToCSNow()``` se spustí v **CS Scriptu** funkci ```message_update()``` kde si opět si přečteme zprávy. Pokud zpráva souhlasí, provedeme reakci na zprávu, naplníme stringData textem nejakeho jmena.
```csharp
// toto je funkce volana pouze z protejsiho komunikacniho objektu GD (neco jako hej mas zpravu, ted si ji precti)
	public void message_update()
	{
		// cteme a filtrujeme prichozi povahu message
		string msg = msgObject.GetMessage();
		switch (msg)
		{
            		case "msg_get_yourname":
                	{
                    		msgObject.SetStringData("Michael");
                    		break;
                	}
        	}
	}
```
V tu chvíli kdy se dokončí funkce message_update se vrátí kód zpátky do **GD Scriptu** hned tam kde skončil po ```SendMessageToCSNow()```, takže následuje:
```python
print(NodeB.msgObject.GetStringData())
```
kde si vezmeme právě uloženou hodnotu string Michael.

### **Volatelné funkce msgObject**
| Funkce      | Argumenty | Popis |
| ----------- | ----------- | -----------|
| ```MessageObject()```        | ```Node``` ownerObject, ```Node``` communicationObject, ```(optional)bool``` multiCommunication          | Konstruktor při vytváření objektu MessageObject,**(optional bool)** jen jestli budeme chtít komunikovat s více GD Scripty naráz|
| ```SendMessageToGDNow()```   | ```string```                                        | Pošle zprávu z CS do GD objektu|
| ```SendMessageToCSNow()```   | ```string```                                        | Pošle zprávu z GD do CS objektu|
| ```GetMessage()```           |                                                                | Vrátí aktualní zprávu          |
| ```ResetAllData()```         |                                                                | Zresetuje **včechna** data pro přenos informací|
| ```GetIsMultiCommunication()```         |                                                                | vrací **true**, pokud je při konstrukci počítáno s komunikací s vícero GDScripty naráz|
| ```SendMessageToGDNow_ToObject()```         | ```string``` message, ```Node``` newCommunicationGDObject        | Pošle zprávu z CS do GD objektu, který nově přiřadíme (jen pokud v konstruktoru MessageObject je argument **multiCommunication** = true)|
| ```SetBoolData()```          | ```bool```                                         | Nastaví nová **bool** data pro přenos informace|
| ```GetBoolData()```          |                                                                | Vrací aktuální **bool** data pro přenos informace|
| ```SetIntData()```           | ```int```                                           | Nastaví nová **int** data pro přenos informace|
| ```GetIntData()```           |                                                                | Vrací aktuální **int** data pro přenos informace|
| ```SetFloatData()```         | ```float```                                       | Nastaví nová **float** data pro přenos informace|
| ```GetFloatData()```         |                                                                | Vrací aktuální **float** data pro přenos informace|
| ```SetStringData()```         | ```string```                                       | Nastaví nová **string** data pro přenos informace|
| ```GetStringData()```         |                                                                | Vrací aktuální **string** data pro přenos informace|
| ```SetVector2Data()```         | ```Vector2```                                       | Nastaví nová **Vector2** data pro přenos informace|
| ```GetVector2Data()```         |                                                                | Vrací aktuální **Vector2** data pro přenos informace|
| ```SetVector3Data()```         | ```Vector3```                                       | Nastaví nová **Vector3** data pro přenos informace|
| ```GetVector3Data()```         |                                                                | Vrací aktuální **Vector3** data pro přenos informace|
| ```SetNodeData()```         | ```Node```                                       | Nastaví nová **Node** data pro přenos informace|
| ```GetNodeData()```         |                                                                | Vrací aktuální **Node** data pro přenos informace|
| ```SetNodeData()```         | ```Node```                                       | Nastaví nová **Node** data pro přenos informace|
| ```LoadBoolDataFromGDNow()```         | ```string``` messge                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **bool** data|
| ```LoadIntDataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **int** data|
| ```LoadFloatDataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **float** data|
| ```LoadStringDataFromGDNow()```         | ```string``` message                             | Stejný jako ```SendMessageToGDNow()``` + ihned vrací očekávaná **string** data|
| ```LoadVector2DataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **Vector2** data|
| ```LoadVector3DataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **Vector3** data|
| ```LoadNodeDataFromGDNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToGDNow()``` + ihned vrací očekávaná **Node** data|
| ```LoadBoolDataFromCSNow()```         | ```string``` messge                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **bool** data|
| ```LoadIntDataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **int** data|
| ```LoadFloatDataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **float** data|
| ```LoadStringDataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **string** data|
| ```LoadVector2DataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **Vector2** data|
| ```LoadVector3DataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **Vector3** data|
| ```LoadNodeDataFromCSNow()```         | ```string``` message                             | Stejný jako  ```SendMessageToCSNow()``` + ihned vrací očekávaná **Node** data|

**tipy k funkcím**
- Pokud použijeme v konstruktoru MessageObject ```(optional)bool``` argument **mutliCommunication** = true, nemůžeme pak na tomhle MessageObjectu využívat zkrácených funkcí pro vracení dat jako je třeba: ```LoadVector3DataFromGDNow()```, můžeme používat pouze ```SendMessageToGDNow_ToObject()```, je to prevence nechtěné komunikaci se špatným communicationObject. Tudíž pokud víme že budeme komunikovat jen s jedním **GD Scriptem**.
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

## Databases
Tato kapitola popisuje jednotlivé databáze využíté v projektu

### items
items.json je databáze itemů určená k TODO
